using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
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
    public class SessionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public SessionController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
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
                var responseFail = ApiResponse.FailureResponse("Invlaid Data", errors);
                return BadRequest(responseFail);
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var user  = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    if (user.Role == UserRole.Admin)
                    {
                        var newSession = _mapper.Map<AttendanceSession>(createSessionDTO);
                        await _unitOfWork.AttendanceSessions.AddAsync(newSession);
                        var IsSucced = await _unitOfWork.SaveAsync();
                        if (IsSucced < 1)
                        {
                            var responseFail = ApiResponse.FailureResponse("UnSccessful Save change", new List<string> { "Unexcepected Error Happend while saving Changes" });
                            return BadRequest(responseFail);
                        }
                        var responseSuccess = ApiResponse.SuccessResponse("Session Created Successfully");
                        return Ok(responseSuccess);
                    }
                }
            }
            var response = ApiResponse.FailureResponse("UnAuthorized", new List<string> { "UnAuthorized Access" }); 
            return Unauthorized(response);
        }

    }
}
