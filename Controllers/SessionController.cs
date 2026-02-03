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
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetSessionAttendance(int sessionId)
        {
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
