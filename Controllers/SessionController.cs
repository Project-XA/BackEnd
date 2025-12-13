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
    }
}
