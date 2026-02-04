using Microsoft.EntityFrameworkCore;
using Project_X.Data.Context;
using Project_X.Data.Repository;
using Project_X.Models;

namespace Project_X.Data.Repositories
{
    public class AttendanceLogRepository : Repository<AttendanceLog>, IAttendanceLogRepository
    {
        public AttendanceLogRepository(AppDbConext context) : base(context) { }

        public async Task<List<AttendanceLog>> GetLogsBySessionAndUsersAsync(int sessionId, List<string> userIds)
        {
            return await _context.AttendanceLogs
                .Where(log => log.SessionId == sessionId && userIds.Contains(log.UserId))
                .ToListAsync();
        }

        public async Task<List<AttendanceLog>> GetLogsWithDetailsAsync(int sessionId)
        {
            return await _context.AttendanceLogs
                .Include(l => l.User)
                .Include(l => l.VerificationSession)
                .Where(l => l.SessionId == sessionId)
                .ToListAsync();
        }

        public async Task<int> CountByUserIdAsync(string userId)
        {
            return await _context.AttendanceLogs.CountAsync(l => l.UserId == userId);
        }

        public async Task AddRangeAsync(IEnumerable<AttendanceLog> logs)
        {
            await _context.AttendanceLogs.AddRangeAsync(logs);
        }
    }
}
