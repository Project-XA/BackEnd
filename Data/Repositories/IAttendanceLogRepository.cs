using Project_X.Models;

namespace Project_X.Data.Repositories
{
    public interface IAttendanceLogRepository : IRepository<AttendanceLog>
    {
        Task<List<AttendanceLog>> GetLogsBySessionAndUsersAsync(int sessionId, List<string> userIds);
        Task<List<AttendanceLog>> GetLogsWithDetailsAsync(int sessionId);
        Task<int> CountByUserIdAsync(string userId);
        Task AddRangeAsync(IEnumerable<AttendanceLog> logs);
    }
}
