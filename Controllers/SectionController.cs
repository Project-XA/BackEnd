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
    public class SectionController : ControllerBase
    {
        private readonly ISectionService _sectionService;

        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> CreateSection(CreateSectionDTO createSectionDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse.FailureResponse("Invalid Data", errors));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _sectionService.CreateSectionAsync(createSectionDTO, userId!);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("organization/{orgId}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetSectionsByOrganization(int orgId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _sectionService.GetSectionsByOrganizationAsync(orgId, userId!);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetSectionById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _sectionService.GetSectionByIdAsync(id, userId!);

            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateSection(int id, UpdateSectionDTO updateSectionDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse.FailureResponse("Invalid Data", errors));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _sectionService.UpdateSectionAsync(id, updateSectionDTO, userId!);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteSection(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _sectionService.DeleteSectionAsync(id, userId!);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("{id}/students")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AddStudentToSection(int id, AddStudentToSectionDTO addStudentDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse.FailureResponse("Invalid Data", errors));
            }

            addStudentDTO.SectionId = id;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _sectionService.AddStudentToSectionAsync(addStudentDTO, userId!);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}/students/{studentUserId}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RemoveStudentFromSection(int id, string studentUserId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _sectionService.RemoveStudentFromSectionAsync(id, studentUserId, userId!);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}/students")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetSectionMembers(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _sectionService.GetSectionMembersAsync(id, userId!);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("join")]
        [Authorize]
        public async Task<IActionResult> JoinSection(JoinSectionDTO joinSectionDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse.FailureResponse("Invalid Data", errors));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _sectionService.JoinSectionByCodeAsync(joinSectionDTO, userId!);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
