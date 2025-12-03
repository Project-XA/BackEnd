using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Models;
using Project_X.Data.UnitOfWork;
using Project_X.Models;
using Project_X.Models.DTOs;
using Project_X.Models.Response;
using Project_X.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        public readonly IUnitOfWork _unitOfWork;
        public readonly IEmailService _emailService;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDTO RegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                var response = ApiResponse.FailureResponse("Inavlid Data", errors);
                return BadRequest(response);
            }
            var user = await _userManager.FindByEmailAsync(RegisterDTO.Email);
            if (user != null)
            {
                var response = ApiResponse.FailureResponse("Email is already registered");
                return BadRequest(response);
            }
            var newUser = _mapper.Map<AppUser>(RegisterDTO);
            var result = await _userManager.CreateAsync(newUser, RegisterDTO.Password);
            var roleResult = await _userManager.AddToRoleAsync(newUser, RegisterDTO.Role.ToString());
            if (!result.Succeeded || !roleResult.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                var response = ApiResponse.FailureResponse("Registration Failed Due to Error in User or Role creation", errors);
                return BadRequest(response);
            }
           
            var responseSuccess = ApiResponse.SuccessResponse("Registration Successful", new
            {
                newUser.Id,
                newUser.FullName,
                newUser.UserName,
                newUser.Email,
                newUser.Role,
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
                var response = ApiResponse.FailureResponse("Invalid Data", errors);
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
                    var response = ApiResponse.SuccessResponse("Login done Successfully", loginToken);
                    return Ok(response);
                }
            }
            var responseFail = ApiResponse.FailureResponse("Invalid Email or Password", new List<string> { "Invalid Email or Password" });
            return BadRequest(responseFail);
        }
        [HttpPost("Forgot-Password")]
        [EnableRateLimiting("OtpPolicy")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                var responseFail = ApiResponse.FailureResponse("Invalid Email", errors);
                return BadRequest(responseFail);
            }
            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);
            if (user != null)
            {
                var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
                var otpEntry = new OTP
                {
                    Email = forgotPasswordDTO.Email,
                    Otp = otp,
                    ExpirationTime = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false,
                };
                await _unitOfWork.OTPs.AddAsync(otpEntry);
                int isSuccess = await _unitOfWork.SaveAsync();
                if (isSuccess == 0)
                {
                    var responseFail = ApiResponse.FailureResponse("UnExpected Error");
                    return BadRequest(responseFail);
                }
                var subject = "Password Reset OTP";
                var body = $@"
                <div style='font-family:Segoe UI,Arial,sans-serif; font-size:14px; color:#1f2937;'>
                    <div style='max-width:560px; margin:0 auto; padding:24px; border:1px solid #e5e7eb; border-radius:8px;'>
                        <h2 style='margin:0 0 16px 0; color:#111827;'>Project X</h2>
                        <p>We received a request to reset the password for your account.</p>
                        <p>Please use the following one-time verification code to continue:</p>
                        <div style='margin:16px 0; padding:16px; background:#f9fafb; border:1px dashed #d1d5db; border-radius:6px; text-align:center;'>
                            <span style='font-size:22px; letter-spacing:4px; font-weight:700; color:#111827;'>{otp}</span>
                        </div>
                        <p>This code will expire in <strong>10 minutes</strong>.</p>
                        <p>If you did not request a password reset, you can safely ignore this email.</p>
                        <p style='margin-top:24px;'>Regards,<br/>Project_X Team</p>
                    </div>
                    <p style='margin-top:12px; font-size:12px; color:#6b7280;'>This is an automated message. Please do not reply.</p>
                </div>";
                await _emailService.SendEmailAsync(forgotPasswordDTO.Email, subject, body);
            }
            var response = ApiResponse.SuccessResponse("If your email exists, an OTP will be sent.");
            return Ok(response);
        }
        [HttpPost("verify-rest-password-otp")]
        [EnableRateLimiting("OtpPolicy")]
        public async Task<IActionResult> VerifyRestPasswordOTP(VerifyOtpResetPasswordDto verifDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                var responseFail = ApiResponse.FailureResponse("Invalid Data", errors);
                return BadRequest(responseFail);
            }
            var otp =await _unitOfWork.OTPs.GetValidOtpByEmailAsync(verifDto.Email, verifDto.Otp);
            if(otp == null)
            {
                var responseFail = ApiResponse.FailureResponse("Inavlid OTP",new List<string> {"Invalid or expired OTP"});
                return BadRequest(responseFail);
            }
            var user = await _userManager.FindByEmailAsync(verifDto.Email);
            if (user == null)
            {
                var responseFail = ApiResponse.FailureResponse("User not found", new List<string> { "User not found" });
                return BadRequest(responseFail);
            }
            var resetToken =await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, verifDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e=>e.Description)
                    .ToList();
                var responseFail = ApiResponse.FailureResponse("Unexpected Error Happend", errors);
                return BadRequest(responseFail);
            }
            otp.IsUsed = true;
            await _unitOfWork.SaveAsync();
            var responseSuccess = ApiResponse.SuccessResponse("Password Reset Successfully");
            return Ok(responseSuccess);
        }
    }

}
