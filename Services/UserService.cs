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

        public UserService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<ApiResponse> GetUserRoleAsync(GetRoleDTO roleDTO)
        {
            var organization = await _unitOfWork.Organizations.GetByCodeAsync(roleDTO.OrgainzatinCode);
            if (organization == null)
            {
                return ApiResponse.FailureResponse("Organization not found", new List<string> { "Invalid Organization Code" });
            }

            var user = await _userManager.FindByEmailAsync(roleDTO.Email);
            if (user != null)
            {
                var isMember = await _unitOfWork.Organizations.ValidateUser(organization.OrganizationId, user.Id);
                if (isMember)
                {
                    var isPass = await _userManager.CheckPasswordAsync(user, roleDTO.Password);
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
    }
}
