using Models;
using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterAsync(UserRegisterDTO registerDTO);
        Task<ApiResponse> LoginAsync(LoginDTO loginDTO);
        Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO);
        Task<ApiResponse> VerifyResetPasswordOTPAsync(VerifyOtpResetPasswordDto verifyDto);
        string GenerateJwtToken(AppUser user);
    }
}
