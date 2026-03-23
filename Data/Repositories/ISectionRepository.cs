using Project_X.Models;

namespace Project_X.Data.Repositories
{
    public interface ISectionRepository : IRepository<Section>
    {
        Task<List<Section>> GetSectionsByOrganizationAsync(int organizationId);
        Task<Section?> GetSectionWithUsersAsync(int sectionId);
        Task<Section?> GetByCodeAsync(int sectionCode);
        Task<bool> VerifySectionCodeAsync(int sectionCode);
    }
}
