
using Project_X.Models;

namespace Project_X.Data.Repositories
{
    public interface IAttendanceSessionRepository : IRepository<AttendanceSession>
    {
        Task<List<AttendanceSession>> GetSessionsByHallIdAsync(int hallId);
        Task<int> CountByOrganizationsAsync(List<int> organizationIds);
    }
}

