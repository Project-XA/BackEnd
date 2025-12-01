using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDTO RegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v=>v.Errors)
                    .Select(e=>e.ErrorMessage)
                    .ToList();
                var response = ApiRepsonse.FailureResponse("Inavlid Data", errors);
                return BadRequest(response);
            }
            var user  = await _userManager.FindByEmailAsync(RegisterDTO.Email);
            if(user != null)
            {
                var response = ApiRepsonse.FailureResponse("Email is already registered");
                return BadRequest(response);
            }
            var newUser = _mapper.Map<AppUser>(RegisterDTO);
            var result = await _userManager.CreateAsync(newUser, RegisterDTO.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                var response = ApiRepsonse.FailureResponse("Registration Failed", errors);
                return BadRequest(response);
            }
           var responseSuccess = ApiRepsonse.SuccessResponse("Registration Successful",new
           {
                newUser.Id,
                newUser.UserName,
                newUser.Email
           });
            return Ok(responseSuccess);
        }
    }
}
