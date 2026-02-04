using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Models;
using Project_X.Data.UnitOfWork;
using Project_X.Models.DTOs;
using Project_X.Models.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Project_X.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public UserService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ApiResponse> GetUserAsync(GetUserDTO userDTO)
        {
            var organization = await _unitOfWork.Organizations.GetByCodeAsync(userDTO.OrganizationCode);
            if (organization == null)
            {
                return ApiResponse.FailureResponse("Organization not found", new List<string> { "Invalid Organization Code" });
            }

            var user = await _userManager.FindByEmailAsync(userDTO.Email);
            if (user != null)
            {
                var isMember = await _unitOfWork.Organizations.ValidateUser(organization.OrganizationId, user.Id);
                if (isMember)
                {
                    var isPass = await _userManager.CheckPasswordAsync(user, userDTO.Password);
                    if (isPass)
                    {
                        var userResponse = _mapper.Map<UserResponseDTO>(user);
                        userResponse.OrganizationId = organization.OrganizationId;
                        userResponse.OrganizationName = organization.OrganizationName;
                        var loginToken = await _authService.GenerateJwtTokenAsync(user);
                        return ApiResponse.SuccessResponse("User is retrieved successfully", new { userResponse, loginToken });
                    }
                    else
                    {
                         return ApiResponse.FailureResponse("Invalid User Data", new List<string> { "Wrong Email Or password" });
                    }
                }
                return ApiResponse.FailureResponse("User not member in the organization", new List<string> { "The userId not found in the organization" });
            }
            return ApiResponse.FailureResponse("Invalid User Data", new List<string> { "Wrong Email Or password" });
        }
        public async Task<ApiResponse> GetUserRoleAsync(GetUserDTO userDTO)
        {
            var organization = await _unitOfWork.Organizations.GetByCodeAsync(userDTO.OrganizationCode);
            if (organization == null)
            {
                return ApiResponse.FailureResponse("Organization not found", new List<string> { "Invalid Organization Code" });
            }

            var user = await _userManager.FindByEmailAsync(userDTO.Email);
            if (user != null)
            {
                var isMember = await _unitOfWork.Organizations.ValidateUser(organization.OrganizationId, user.Id);
                if (isMember)
                {
                    var isPass = await _userManager.CheckPasswordAsync(user, userDTO.Password);
                    if (isPass)
                    {
                        return ApiResponse.SuccessResponse("User is retrieved successfully", user.Role);
                    }
                    else
                    {
                         return ApiResponse.FailureResponse("Invalid User Data", new List<string> { "Wrong Email Or password" });
                    }
                }
                return ApiResponse.FailureResponse("User not member in the organization", new List<string> { "The userId not found in the organization" });
            }
            return ApiResponse.FailureResponse("Invalid User Data", new List<string> { "Wrong Email Or password" });
        }
        public async Task<ApiResponse> GetUserStatisticsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse.FailureResponse("User not found", new List<string> { "Invalid User ID" });
            }

            var organizations = await _unitOfWork.Organizations.GetUserOrganizationsAsync(userId);
            var orgIds = organizations.Select(o => o.OrganizationId).ToList();

            if (!orgIds.Any())
            {
                return ApiResponse.SuccessResponse("User statistics retrieved successfully", new UserStatisticsDTO());
            }
            var totalSessions = await _unitOfWork.AttendanceSessions.CountByOrganizationsAsync(orgIds);
            var attendedSessions = await _unitOfWork.AttendanceLogs.CountByUserIdAsync(userId);
            var missedSessions = totalSessions - attendedSessions;
            if (missedSessions < 0) missedSessions = 0;

            double attendancePercentage = totalSessions > 0 ? ((double)attendedSessions / totalSessions) * 100 : 0;

            var stats = new UserStatisticsDTO
            {
                TotalSessions = totalSessions,
                AttendedSessions = attendedSessions,
                MissedSessions = missedSessions,
                AttendancePercentage = Math.Round(attendancePercentage, 2)
            };

            return ApiResponse.SuccessResponse("User statistics retrieved successfully", stats);
        }
    }
}
