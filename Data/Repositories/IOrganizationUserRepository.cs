using Project_X.Models;

namespace Project_X.Data.Repositories
{
    public interface IOrganizationUserRepository : IRepository<OrganizationUser>
    {
        Task<List<OrganizationUser>> GetByOrganizationAndUsersAsync(int organizationId, List<string> userIds);
    }
}
