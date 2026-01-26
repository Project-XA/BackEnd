using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Project_X.Models.DTOs;
using Project_X.Models.Response;
using Project_X.Services;

namespace Project_X.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("get-user")]
        public async Task<IActionResult> GetUser(GetUserDTO roleDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse.FailureResponse("Invalid data", errors));
            }

            var result = await _userService.GetUserAsync(roleDTO);
            
            if (result.Success)
            {
                return Ok(result);
            }

            if (result.Message == "Organization not found")
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        [HttpGet("statistics")]
        [Authorize]
        public async Task<IActionResult> GetStatistics()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse.FailureResponse("Unauthorized"));
            }

            var result = await _userService.GetUserStatisticsAsync(userId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("get-user-role")]
        public async Task<IActionResult> GetUserRole(GetUserDTO roleDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse.FailureResponse("Invalid data", errors));
            }

            var result = await _userService.GetUserRoleAsync(roleDTO);
            
            if (result.Success)
            {
                return Ok(result);
            }

            if (result.Message == "Organization not found")
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }
    }
}