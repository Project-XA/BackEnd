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
                return NotFound(result);
            }
            return BadRequest(result);
        }

        [HttpGet("get-all-halls/{organizationId}")]
        public async Task<IActionResult> GetAllHalls(int organizationId)
        {
            var result = await _hallService.GetAllHallsAsync(organizationId);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.Message == "Organization not found.")
            {
                 return NotFound(result);
            }
            return BadRequest(result);
        }

        [HttpGet("get-hall/{hallId}")]
        public async Task<IActionResult> GetHallById(int hallId)
        {
            var result = await _hallService.GetHallByIdAsync(hallId);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpPut("update-hall/{hallId}")]
        public async Task<IActionResult> UpdateHall(int hallId, UpdateHallDTO updateHallDTO)
        {
             if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse.FailureResponse("Invalid Data", errors));
            }

            var result = await _hallService.UpdateHallAsync(hallId, updateHallDTO);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.Message == "Hall not found")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("delete-hall/{hallId}")]
        public async Task<IActionResult> DeleteHall(int hallId)
        {
            var result = await _hallService.DeleteHallAsync(hallId);
            if (result.Success)
            {
                return Ok(result);
            }
             if (result.Message == "Hall not found")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }
    }
}
