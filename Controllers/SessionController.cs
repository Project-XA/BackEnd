using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_X.Models.DTOs;
using Project_X.Models.Response;
using Project_X.Services;
using System.Security.Claims;
using Models;

namespace Project_X.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpPost("Create-Session")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateSession(CreateSessionDTO createSessionDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v=>v.Errors).Select(e=>e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse.FailureResponse("Invalid Data", errors));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _sessionService.CreateSessionAsync(createSessionDTO, userId);

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
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetSessionById(int id)
        {
            var result = await _sessionService.GetSessionByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpGet("hall/{hallId}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetSessionsByHallId(int hallId)
        {
            var result = await _sessionService.GetSessionsByHallIdAsync(hallId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{sessionId}/attendance")]
        public async Task<IActionResult> GetSessionAttendance(int sessionId, [FromServices] IOrganizationService organizationService)
        {
            if (!Request.Headers.TryGetValue("X-Api-Key", out var apiKey))
            {
                return Unauthorized(ApiResponse.FailureResponse("API Key is required."));
            }

            var orgResult = await organizationService.GetOrganizationByApiKeyAsync(apiKey!);
            if (!orgResult.Success)
            {
                return Unauthorized(ApiResponse.FailureResponse("Invalid API Key."));
            }

            var organization = (Organization)orgResult.Data;
            var sessionResult = await _sessionService.GetSessionByIdAsync(sessionId);
            
            if (!sessionResult.Success)
            {
                 return NotFound(ApiResponse.FailureResponse("Session not found."));
            }

            var session = (SessionResponseDTO)sessionResult.Data;
            if (session.OrganizationId != organization.OrganizationId)
            {
                 return Unauthorized(ApiResponse.FailureResponse("Access Denied: Session does not belong to this organization."));
            }

            var result = await _sessionService.GetSessionAttendanceAsync(sessionId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateSession(int id, UpdateSessionDTO updateSessionDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse.FailureResponse("Invalid Data", errors));
            }

            var result = await _sessionService.UpdateSessionAsync(id, updateSessionDTO);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var result = await _sessionService.DeleteSessionAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        
        [HttpPost("save-attend")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> SaveAttend(SaveAttendDTO saveAttendDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse.FailureResponse("Invalid Data", errors));
            }

            var result = await _sessionService.SaveAttendAsync(saveAttendDTO);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
