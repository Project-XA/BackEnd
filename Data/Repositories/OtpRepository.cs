using Project_X.Data.Repositories;
using Project_X.Models;
using Microsoft.EntityFrameworkCore;
using Project_X.Data.Context;

namespace Project_X.Data.Repository
{
    public class OtpRepository : Repository<OTP>, IOtpRepository
    {
        public OtpRepository(AppDbConext context) 
            : base(context)
        {
        }

        public async Task<OTP?> GetValidOtpByEmailAsync(string email, string otpCode)
        {
            return await _context.OTPs.FirstOrDefaultAsync(o =>
                o.Email == email &&
                o.Otp == otpCode &&
                !o.IsUsed &&
                o.ExpirationTime > DateTime.UtcNow);
        }
    }
}
