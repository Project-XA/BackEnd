using Models;

namespace Project_X.Data.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student?> GetByAppUserIdAsync(string appUserId);
        Task<Student?> GetByOrganizationAndRollNumberAsync(int organizationId, string rollNumber);
        Task<Student?> GetByOrganizationAndEmailAsync(int organizationId, string email);
        Task<List<Student>> GetByOrganizationIdAsync(int organizationId);
    }
}
