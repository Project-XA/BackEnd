using Models;

namespace Project_X.Data.Repositories
{
    public interface IOrganizationRepository:IRepository<Organization>
    {
        public Task<bool> VerifyOrganizationCodeAsync(int orgCode);
        public Task<Organization?> GetByCodeAsync(int orgCode);
        public Task<bool> ValidateUser(int organizationId,string userId);
        public Task<List<Organization>> GetUserOrganizationsAsync(string userId);
        public Task<List<AppUser>> GetOrganizationUsersAsync(int organizationId);
    }
}
