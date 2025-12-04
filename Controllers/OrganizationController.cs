using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Models;
using Models.Enums;
using Project_X.Data.Repositories;
using Project_X.Data.UnitOfWork;
using Project_X.Models;
using Project_X.Models.DTOs;
using Project_X.Models.Enums;
using Project_X.Models.Response;
using System.Security.Claims;

namespace Project_X.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Random _random = new Random();
        public OrganizationController(
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpPost("create-organization")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateOrganization(CreateOrganizationDTO createOrganizationDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v=>v.Errors).Select(e=>e.ErrorMessage).ToList();
                var responseFail = ApiResponse.FailureResponse("Invalid Data",errors);
                return BadRequest(responseFail);
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if(userId!= null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if(user != null)
                {
                    if(user.Role == UserRole.Admin)
                    {
                        var organization = _mapper.Map<Organization>(createOrganizationDTO);
                        organization.CreatedAt = DateTime.UtcNow;
                        var code = _random.Next(1000, 9999);
                        var org = await _unitOfWork.Organizations.GetByCodeAsync(code);
                        while(org != null)
                        {
                            code = _random.Next(1000, 9999);
                        }
                        organization.OrganizationCode = code;
                        await _unitOfWork.Organizations.AddAsync(organization);
                        await _unitOfWork.SaveAsync();
                        var organizationUser = new OrganizationUser
                        {
                            OrganizationId =organization.OrganizationId,
                            UserId = user.Id,
                            Role = user.Role
                        };
                        await _unitOfWork.OrganizationUsers.AddAsync(organizationUser);
                        await _unitOfWork.SaveAsync();
                        
                        var organizationResponse = _mapper.Map<OrganizationResponseDTO>(organization);
                        
                        var responseSuccess = ApiResponse.SuccessResponse("Organization Created Successfully", organizationResponse);
                        return Ok(responseSuccess);
                    }
                }
            }
            var response = ApiResponse.FailureResponse("Unauthorized");
            return Unauthorized(response);
        }
        [HttpPost("add-member")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMember(AddMemberDTO addMemberDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var responseFail = ApiResponse.FailureResponse("Invalid Data", errors);
                return BadRequest(responseFail);
            }

            var existingUser = await _userManager.FindByEmailAsync(addMemberDTO.Email);
            if (existingUser != null)
            {
                var responseFail = ApiResponse.FailureResponse("User with this email already exists.");
                return BadRequest(responseFail);
            }

            var organization = await _unitOfWork.Organizations.GetByIdAsync(addMemberDTO.OrganizationId);
            if (organization == null)
            {
                var responseFail = ApiResponse.FailureResponse("Organization not found.",new List<string> {"Invalid Organizztion Id"});
                return NotFound(responseFail);
            }
            var newUser = _mapper.Map<AppUser>(addMemberDTO); 
            var result = await _userManager.CreateAsync(newUser, addMemberDTO.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(ApiResponse.FailureResponse("Failed to create user", errors));
            }

            await _userManager.AddToRoleAsync(newUser, addMemberDTO.Role.ToString());

            var organizationUser = new OrganizationUser
            {
                OrganizationId = addMemberDTO.OrganizationId,
                UserId = newUser.Id,
                Role = addMemberDTO.Role
            };

            await _unitOfWork.OrganizationUsers.AddAsync(organizationUser);
            await _unitOfWork.SaveAsync();
            var responseSuccess = ApiResponse.SuccessResponse("Member added successfully",
                new { newUser.Id, newUser.Email, organization.OrganizationId });
            return Ok(responseSuccess);
        }
    }
}
