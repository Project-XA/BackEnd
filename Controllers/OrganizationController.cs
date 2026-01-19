using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_X.Models.DTOs;
using Project_X.Models.Response;
using Project_X.Services;
using System.Security.Claims;

namespace Project_X.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpPost("create-organization")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOrganization(CreateOrganizationDTO createOrganizationDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var responseFail = ApiResponse.FailureResponse("Invalid Data", errors);
                return BadRequest(responseFail);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _organizationService.CreateOrganizationAsync(createOrganizationDTO, userId);

            if (result.Success)
            {
                return Ok(result);
            }
            if (result.Message == "Unauthorized")
            {
                return Unauthorized(result);
            }
            return BadRequest(result);
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

            var result = await _organizationService.AddMemberAsync(addMemberDTO);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrganizationById(int id)
        {
            var result = await _organizationService.GetOrganizationByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrganization(int id, UpdateOrganizationDTO updateOrganizationDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var responseFail = ApiResponse.FailureResponse("Invalid Data", errors);
                return BadRequest(responseFail);
            }

            var result = await _organizationService.UpdateOrganizationAsync(id, updateOrganizationDTO);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrganization(int id)
        {
            var result = await _organizationService.DeleteOrganizationAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("user-orgs")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserOrganizations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _organizationService.GetUserOrganizationsAsync(userId!);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrganizationUsers(int id)
        {
            var result = await _organizationService.GetOrganizationUsersAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
    }
}