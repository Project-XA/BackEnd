
using Project_X.Data.Context;
using Project_X.Models;
using Microsoft.EntityFrameworkCore;
using Project_X.Data.Repository;

namespace Project_X.Data.Repositories
{
    public class AttendanceSessionRepository : Repository<AttendanceSession>, IAttendanceSessionRepository
    {
        public AttendanceSessionRepository(AppDbConext context) : base(context) { }

        public async Task<List<AttendanceSession>> GetSessionsByHallIdAsync(int hallId)
        {
            return await _context.AttendanceSessions.Where(s => s.HallId == hallId).ToListAsync();
        }

        public async Task<int> CountByOrganizationsAsync(List<int> organizationIds)
        {
            return await _context.AttendanceSessions
                .CountAsync(s => organizationIds.Contains(s.OrganizationId));
        }
    }
}

