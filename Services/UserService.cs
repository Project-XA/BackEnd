using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Models;
using Project_X.Data.UnitOfWork;
using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ApiResponse> GetUserAsync(GetUserDTO userDTO)
        {
            var organization = await _unitOfWork.Organizations.GetByCodeAsync(userDTO.OrgainzatinCode);
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
                        return ApiResponse.SuccessResponse("User is retrieved successfully", userResponse);
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
    }
}
