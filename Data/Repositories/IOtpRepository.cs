using Project_X.Models;

namespace Project_X.Data.Repositories
{
    public interface IOtpRepository:IRepository<OTP>
    {
        Task<OTP?> GetValidOtpByEmailAsync(string email, string otpCode);
    }
}
