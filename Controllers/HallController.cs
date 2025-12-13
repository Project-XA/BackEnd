using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_X.Models.DTOs;
using Project_X.Models.Response;
using Project_X.Services;

namespace Project_X.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class HallController : ControllerBase
    {
        private readonly IHallService _hallService;

        public HallController(IHallService hallService)
        {
            _hallService = hallService;
        }

        [HttpPost("Create-hall")]
        public async Task<IActionResult> CreateHall(HallDTO hallDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse.FailureResponse("Invalid Data", errors));
            }

            var result = await _hallService.CreateHallAsync(hallDTO);
            
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.Message == "Organization not found.")
            {
                // Preserve original behavior
                return NotFound(result);
            }
            return BadRequest(result);
        }
    }
}
