namespace Project_X.Models.DTOs
{
    public class VerifyOtpResetPasswordDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }
}
