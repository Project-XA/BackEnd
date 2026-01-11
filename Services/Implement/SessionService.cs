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
    }
}
