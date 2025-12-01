using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using Project_X.Models.DTOs;
using Project_X.Models.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                var response = ApiRepsonse.FailureResponse("Invalid Data", errors);
                return BadRequest(response);
            }
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user != null)
            {
                var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (result)
                {
                    var Claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Email, loginDTO.Email),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier,user.Id)
                    };
                    var securitkey = Environment.GetEnvironmentVariable("SecurityKey");
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securitkey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    // var issuer = Environment.GetEnvironmentVariable("Issuer");
                    // var audience = Environment.GetEnvironmentVariable("audience");
                    var token = new JwtSecurityToken(
                        // issuer: string.IsNullOrWhiteSpace(issuer) ? null : issuer,
                        // audience: string.IsNullOrWhiteSpace(audience) ? null : audience,
                        claims: Claims,
                        notBefore: DateTime.UtcNow,
                        expires: DateTime.UtcNow.AddMinutes(90),
                        signingCredentials: creds
                    );
                    var loginToken = new JwtSecurityTokenHandler().
                        WriteToken(token);
                    var response = ApiRepsonse.SuccessResponse("Login done Successfully",loginToken);
                    return Ok(response);
                }
            }
            var responseFail = ApiRepsonse.FailureResponse("Invalid Email or Password", new List<string> { "Invalid Email or Password" });
            return BadRequest(responseFail);
        }
    }

}
