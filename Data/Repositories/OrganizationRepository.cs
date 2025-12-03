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
        public async Task<bool> VerfiyOrganiztionCOde(int orgCode)
        {
            return await _context.Organizations
                .AnyAsync(o => o.OrganizationCode == orgCode);
        }
        public async Task<Organization?> GetByCodeAsync(int orgCode)
        {
            return await _context.Organizations
                .FirstOrDefaultAsync(o => o.OrganizationCode == orgCode);
        }
    }
}
