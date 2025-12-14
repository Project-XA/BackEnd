using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Models;
using Project_X.Data.UnitOfWork;
using Project_X.Models;
using Project_X.Models.DTOs;
using Project_X.Models.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Project_X.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<ApiResponse> RegisterAsync(UserRegisterDTO registerDTO)
        {
            var user = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (user != null)
            {
                return ApiResponse.FailureResponse("Email is already registered");
            }

            var newUser = _mapper.Map<AppUser>(registerDTO);
            var result = await _userManager.CreateAsync(newUser, registerDTO.Password);
            
            if (!result.Succeeded) 
            {
                 var errors = result.Errors.Select(e => e.Description).ToList();
                 return ApiResponse.FailureResponse("Registration Failed", errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(newUser, registerDTO.Role.ToString());
            
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(newUser);
                var errors = roleResult.Errors.Select(e => e.Description).ToList();
                return ApiResponse.FailureResponse("Registration Failed Due to Error in Role creation", errors);
            }

            return ApiResponse.SuccessResponse("Registration Successful", new
            {
                newUser.Id,
                newUser.FullName,
                newUser.UserName,
                newUser.Email,
                newUser.Role,
            });
        }

        public async Task<ApiResponse> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user != null)
            {
                var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (result)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Email, loginDTO.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    };
                    var roles = await _userManager.GetRolesAsync(user);
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    var securityKey = Environment.GetEnvironmentVariable("SecurityKey");
                    if (string.IsNullOrEmpty(securityKey))
                    {
                        throw new InvalidOperationException("SecurityKey is not configured.");
                    }
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        claims: claims,
                        notBefore: DateTime.UtcNow,
                        expires: DateTime.UtcNow.AddMinutes(90),
                        signingCredentials: creds
                    );
                    var loginToken = new JwtSecurityTokenHandler().WriteToken(token);
                    return ApiResponse.SuccessResponse("Login done Successfully", loginToken);
                }
            }
            return ApiResponse.FailureResponse("Invalid Email or Password", new List<string> { "Invalid Email or Password" });
        }

        public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
        {
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
                    return ApiResponse.FailureResponse("Unexpected Error");
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
           
            return ApiResponse.SuccessResponse("If your email exists, an OTP will be sent.");
        }

        public async Task<ApiResponse> VerifyResetPasswordOTPAsync(VerifyOtpResetPasswordDto verifyDto)
        {
            var otp = await _unitOfWork.OTPs.GetValidOtpByEmailAsync(verifyDto.Email, verifyDto.Otp);
            if (otp == null)
            {
                return ApiResponse.FailureResponse("Invalid OTP", new List<string> { "Invalid or expired OTP" });
            }

            var user = await _userManager.FindByEmailAsync(verifyDto.Email);
            if (user == null)
            {
                return ApiResponse.FailureResponse("User not found", new List<string> { "User not found" });
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, verifyDto.NewPassword);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse.FailureResponse("Unexpected Error Happened", errors);
            }

            otp.IsUsed = true;
            await _unitOfWork.SaveAsync();

            return ApiResponse.SuccessResponse("Password Reset Successfully");
        }
    }
}
