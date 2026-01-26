using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Models;
using Project_X.Data.UnitOfWork;
using Project_X.Models;
using Project_X.Models.DTOs;
using Project_X.Models.Enums;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public class SessionService : ISessionService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse> CreateSessionAsync(CreateSessionDTO createSessionDTO, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return ApiResponse.FailureResponse("Unauthorized", new List<string> { "Unauthorized Access" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.Role != UserRole.Admin)
            {
                return ApiResponse.FailureResponse("Unauthorized", new List<string> { "Unauthorized Access" });
            }
            var hall = await _unitOfWork.Halls.GetByIdAsync(createSessionDTO.HallId);
            if (hall == null)
            {
                return ApiResponse.FailureResponse("Hall not found", new List<string> { "Invalid Hall ID" });
            }

            if (hall.OrganizationId != createSessionDTO.OrganizationId)
            {
                return ApiResponse.FailureResponse("Hall mismatch", new List<string> { "Hall does not belong to the specified organization" });
            }

            var newSession = _mapper.Map<AttendanceSession>(createSessionDTO);
            await _unitOfWork.AttendanceSessions.AddAsync(newSession);
            var isSuccess = await _unitOfWork.SaveAsync();

            if (isSuccess < 1)
            {
                return ApiResponse.FailureResponse("Unsuccessful Save change", new List<string> { "Unexpected Error Happened while saving Changes" });
            }

            return ApiResponse.SuccessResponse("Session Created Successfully");
        }
        public async Task<ApiResponse> GetSessionByIdAsync(int sessionId)
        {
            var session = await _unitOfWork.AttendanceSessions.GetByIdAsync(sessionId);
            if (session == null)
            {
                return ApiResponse.FailureResponse("Session not found", new List<string> { "Invalid Session ID" });
            }
            var sessionResponse = _mapper.Map<SessionResponseDTO>(session);
            return ApiResponse.SuccessResponse("Session retrieved successfully", sessionResponse);
        }

        public async Task<ApiResponse> GetSessionsByHallIdAsync(int hallId)
        {
             var hall = await _unitOfWork.Halls.GetByIdAsync(hallId);
             if (hall == null)
             {
                 return ApiResponse.FailureResponse("Hall not found", new List<string> { "Invalid Hall ID" });
             }

             var sessions = await _unitOfWork.AttendanceSessions.GetSessionsByHallIdAsync(hallId);
             var sessionResponses = _mapper.Map<List<SessionResponseDTO>>(sessions);
             return ApiResponse.SuccessResponse("Sessions retrieved successfully", sessionResponses);
        }

        public async Task<ApiResponse> UpdateSessionAsync(int sessionId, UpdateSessionDTO updateSessionDTO)
        {
            var session = await _unitOfWork.AttendanceSessions.GetByIdAsync(sessionId);
            if (session == null)
            {
                return ApiResponse.FailureResponse("Session not found", new List<string> { "Invalid Session ID" });
            }

            _mapper.Map(updateSessionDTO, session);
            // Updating StartAt and EndAt might require validation logic similar to creation if needed, 
            // but relying on DTO validation attributes for now.
            
            _unitOfWork.AttendanceSessions.Update(session);
            await _unitOfWork.SaveAsync();

            var sessionResponse = _mapper.Map<SessionResponseDTO>(session);
            return ApiResponse.SuccessResponse("Session updated successfully", sessionResponse);
        }

        public async Task<ApiResponse> DeleteSessionAsync(int sessionId)
        {
            var session = await _unitOfWork.AttendanceSessions.GetByIdAsync(sessionId);
            if (session == null)
            {
                return ApiResponse.FailureResponse("Session not found", new List<string> { "Invalid Session ID" });
            }

            _unitOfWork.AttendanceSessions.Delete(session);
            await _unitOfWork.SaveAsync();

            return ApiResponse.SuccessResponse("Session deleted successfully");
        }

        public async Task<ApiResponse> CreateVerificationSessionAsync(CreateVerificationSessionDTO verificationDTO)
        {
            var user = await _userManager.FindByIdAsync(verificationDTO.UserId);
            if (user == null)
            {
                return ApiResponse.FailureResponse("User not found", new List<string> { "Invalid User ID" });
            }

            var session = await _unitOfWork.AttendanceSessions.GetByIdAsync(verificationDTO.SessionId);
            if (session == null)
            {
                return ApiResponse.FailureResponse("Session not found", new List<string> { "Invalid Session ID" });
            }

            var verificationSession = _mapper.Map<VerificationSession>(verificationDTO);
            await _unitOfWork.VerificationSessions.AddAsync(verificationSession);
            var isSuccess = await _unitOfWork.SaveAsync();

            if (isSuccess < 1)
            {
                return ApiResponse.FailureResponse("Unsuccessful Save change", new List<string> { "Unexpected Error Happened while saving Changes" });
            }

            return ApiResponse.SuccessResponse("Verification Session Created Successfully", new { VerificationId = verificationSession.VerificationId });
        }

        public async Task<ApiResponse> SaveAttendAsync(SaveAttendDTO attendDTO)
        {
            var session = await _unitOfWork.AttendanceSessions.GetByIdAsync(attendDTO.SessionId);
            if (session == null)
            {
                return ApiResponse.FailureResponse("Session not found", new List<string> { "Invalid Session ID" });
            }

            if (session.OrganizationId == 0)
            {
                return ApiResponse.FailureResponse("Session is not associated with an organization", new List<string> { "Invalid Session Configuration" });
            }

            var uniqueVerificationIds = attendDTO.VerificationIds.Distinct().ToList();
            var verifications = await _unitOfWork.VerificationSessions.FindAllAsync(
                v => uniqueVerificationIds.Contains(v.VerificationId));

            var errors = new List<string>();
            var attendanceLogs = new List<AttendanceLog>();
            
            var foundVerificationIds = verifications.Select(v => v.VerificationId).ToHashSet();
            foreach (var vId in uniqueVerificationIds)
            {
                if (!foundVerificationIds.Contains(vId))
                {
                    errors.Add($"Verification with ID '{vId}' not found");
                }
            }

            var usersToVerify = new List<string>();
            var validVerifications = new List<VerificationSession>();

            foreach (var verification in verifications)
            {
                if (verification.SessionId != attendDTO.SessionId)
                {
                    errors.Add($"Verification '{verification.VerificationId}' does not belong to the specified session");
                    continue;
                }
                
                if (verification.IsSuccessed == false)
                {
                    errors.Add($"Verification '{verification.VerificationId}' was not successful");
                    continue;
                }
                usersToVerify.Add(verification.UserId);
                validVerifications.Add(verification);
            }
            
            if (!usersToVerify.Any())
            {
                 if (errors.Any()) return ApiResponse.FailureResponse("Failed to save attendance", errors);
                 return ApiResponse.FailureResponse("No valid verifications found", new List<string> { "No valid verifications to process" });
            }

            var orgUsers = await _unitOfWork.OrganizationUsers.FindAllAsync(
                ou => ou.OrganizationId == session.OrganizationId && usersToVerify.Contains(ou.UserId));
            
            var validOrgUserIds = orgUsers.Select(ou => ou.UserId).ToHashSet();
            var existingLogs = await _unitOfWork.AttendanceLogs.FindAllAsync(
                log => log.SessionId == attendDTO.SessionId && usersToVerify.Contains(log.UserId));
            
            var existingLogUserIds = existingLogs.Select(l => l.UserId).ToHashSet();
            foreach (var verification in validVerifications)
            {
                var userId = verification.UserId;

                if (!validOrgUserIds.Contains(userId))
                {
                    errors.Add($"User associated with verification '{verification.VerificationId}' is not a member of the organization");
                    continue;
                }

                if (existingLogUserIds.Contains(userId))
                {
                    errors.Add($"Attendance already recorded for user associated with verification '{verification.VerificationId}'");
                    continue;
                }

                var attendanceLog = new AttendanceLog
                {
                    SessionId = attendDTO.SessionId,
                    UserId = userId,
                    TimeStamp = DateTime.UtcNow,
                    Result = AttendanceResult.Present,
                    VerificationId = verification.VerificationId,
                    ProofSignature = verification.ProofSignature
                };

                attendanceLogs.Add(attendanceLog);
            }

            if (errors.Any())
            {
                return ApiResponse.FailureResponse("Failed to save attendance", errors);
            }

            if (!attendanceLogs.Any())
            {
                return ApiResponse.FailureResponse("No valid attendance records to save", new List<string> { "All verifications failed validation" });
            }

            try
            {
                foreach (var log in attendanceLogs)
                {
                    await _unitOfWork.AttendanceLogs.AddAsync(log);
                }

                var saved = await _unitOfWork.SaveAsync();
                
                if (saved < 1)
                {
                    return ApiResponse.FailureResponse("Failed to save attendance", new List<string> { "Database save operation failed" });
                }

                return ApiResponse.SuccessResponse($"Attendance saved successfully for {attendanceLogs.Count} user(s)", 
                    new { SavedCount = attendanceLogs.Count, SessionId = attendDTO.SessionId });
            }
            catch (Exception ex)
            {
                return ApiResponse.FailureResponse("An error occurred while saving attendance", new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse> GetSessionAttendanceAsync(int sessionId)
        {
            var session = await _unitOfWork.AttendanceSessions.GetByIdAsync(sessionId);
            if (session == null)
            {
                return ApiResponse.FailureResponse("Session not found", new List<string> { "Invalid Session ID" });
            }
            var logs = await _unitOfWork.AttendanceLogs.FindAllAsync(
                l => l.SessionId == sessionId,
                new[] { "User", "VerificationSession" }
            );

            var attendanceRecords = _mapper.Map<List<AttendanceRecordDTO>>(logs);
            return ApiResponse.SuccessResponse("Attendance retrieved successfully", attendanceRecords);
        }
    }
}
