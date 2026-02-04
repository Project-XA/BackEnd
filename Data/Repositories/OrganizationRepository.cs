using Microsoft.EntityFrameworkCore;
using Models;
using Project_X.Data.Context;
using Project_X.Data.Repository;

namespace Project_X.Data.Repositories
{
    public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(AppDbConext conext)
            : base(conext){}
        public async Task<bool> VerifyOrganizationCodeAsync(int orgCode)
        {
            return await _context.Organizations
                .AnyAsync(o => o.OrganizationCode == orgCode);
        }
        public async Task<Organization?> GetByCodeAsync(int orgCode)
        {
            return await _context.Organizations
                .FirstOrDefaultAsync(o => o.OrganizationCode == orgCode);
        }

        public async Task<bool> ValidateUser(int organizationId, string userId)
        {
            var result = await _context.OrganizationUser.
                AnyAsync(ou=>ou.OrganizationId==organizationId&& ou.UserId==userId);
            return result;
        }

        public async Task<List<Organization>> GetUserOrganizationsAsync(string userId)
        {
            var result  = await _context.OrganizationUser
                .Where(ou => ou.UserId == userId)
                .Include(ou => ou.Organization)
                .Select(ou => ou.Organization)
                .ToListAsync();
            return result;
        }

        public async Task<List<AppUser>> GetOrganizationUsersAsync(int organizationId)
        {
            var users = await _context.OrganizationUser
                .Where(ou => ou.OrganizationId == organizationId)
                .Include(ou => ou.User)
                .Select(ou => ou.User)
                .ToListAsync();
            return users;
        }
    }
}
