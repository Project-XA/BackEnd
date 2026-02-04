using Microsoft.EntityFrameworkCore;
using Project_X.Models;
using Project_X.Data.Context;
using Project_X.Data.Repository;

namespace Project_X.Data.Repositories
{
    public class OrganizationUserRepository : Repository<OrganizationUser>, IOrganizationUserRepository
    {
        public OrganizationUserRepository(AppDbConext context) : base(context) { }

        public async Task<List<OrganizationUser>> GetByOrganizationAndUsersAsync(int organizationId, List<string> userIds)
        {
            return await _context.OrganizationUser
                .Where(ou => ou.OrganizationId == organizationId && userIds.Contains(ou.UserId))
                .ToListAsync();
        }
    }
}
