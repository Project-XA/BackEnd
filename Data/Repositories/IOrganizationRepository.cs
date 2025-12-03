using Models;

namespace Project_X.Data.Repositories
{
    public interface IOrganizationRepository:IRepository<Organization>
    {
        public Task<bool> VerfiyOrganiztionCOde(int orgCode);
        public Task<Organization?> GetByCodeAsync(int orgCode);
    }
}
