using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Project_X.Data.UnitOfWork;
using Project_X.Models.DTOs;
using Project_X.Models.Enums;
using Project_X.Models.Response;
using System.Security.Claims;

namespace Project_X.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet("get-user-role")]
        public async Task<IActionResult> GetUserRole(GetRoleDTO roleDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                var responseFail = ApiResponse.FailureResponse("Invalid data", errors);
                return BadRequest(responseFail);
            }
            var organization = await _unitOfWork.Organizations
                .GetByCodeAsync(roleDTO.OrgainzatinCode);
            if (organization == null)
            {
                var responseOrgNotFound = ApiResponse.FailureResponse("Organization not found", new List<string> { "Invalid Organization Code" });
                return NotFound(responseOrgNotFound);
            }
            var user = await _userManager.FindByEmailAsync(roleDTO.Email);
            if (user != null)
            {
                var IsMemeber = await _unitOfWork.Organizations.ValidateUser(organization.OrganizationId,user.Id);
                if (IsMemeber)
                {
                    var isPaas = await _userManager.CheckPasswordAsync(user, roleDTO.Password);
                    if (isPaas)
                    {
                        var responseSuccess = ApiResponse.SuccessResponse("User is retrived successfully", user.Role);
                        return Ok(responseSuccess);
                    }
                    else
                    {
                        var responsePassWrong = ApiResponse.FailureResponse("Invalid User Data", new List<string> { "Wrong Email Or password" });
                        return BadRequest(responsePassWrong);
                    }
                }
                var responseUserNotMemeber = ApiResponse.FailureResponse("User not member in the organization", new List<string> { "the userId not found in the organization" });
                return BadRequest(responseUserNotMemeber);
            }
            var responseEmailWrong = ApiResponse.FailureResponse("Invalid User Data", new List<string> { "Wrong Email Or password" });
            return BadRequest(responseEmailWrong);
        } 
    }
}