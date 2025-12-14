using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("get-user")]
        public async Task<IActionResult> GetUser([FromQuery] GetUserDTO roleDTO)
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
    }
}