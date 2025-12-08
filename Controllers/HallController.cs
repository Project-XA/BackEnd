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
using Project_X.Models.Response;
using System.Security.Claims;

namespace Project_X.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class HallController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public HallController(UserManager<AppUser> userManager, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("Create-hall")]
        public async Task<IActionResult> CreateHall(HallDTO hallDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                var responseFail = ApiResponse.FailureResponse("Invalid Data", errors);
                return BadRequest(responseFail);
            }
            var hall = _unitOfWork.Halls.GetHallByNameAsync(hallDTO.HallName);
            if(hall != null)
            {
                var responseFail = ApiResponse.FailureResponse("Hall with this name already exists.",new List<string> { "Duplicate Hall Name"});
                return BadRequest(responseFail);
            }
            var organization = await _unitOfWork.Organizations.GetByIdAsync(hallDTO.OrganizationId);
            if(organization != null) {
                var hallEntity = _mapper.Map<Hall>(hallDTO);
                hallEntity.Organization = organization;
                await _unitOfWork.Halls.AddAsync(hallEntity);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                var responseFail = ApiResponse.FailureResponse("Organization not found.", new List<string> { "Invalid Organization ID" });
                return NotFound(responseFail);
            }
            var responseSuccess = ApiResponse.SuccessResponse("Hall created successfully.");
            return Ok(responseSuccess);
        }
    }
}
