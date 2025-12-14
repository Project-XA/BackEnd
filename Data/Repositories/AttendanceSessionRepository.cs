
using Project_X.Data.Context;
using Project_X.Models;
using Microsoft.EntityFrameworkCore;
using Project_X.Data.Repository;

namespace Project_X.Data.Repositories
{
    public class AttendanceSessionRepository : Repository<AttendanceSession>, IAttendanceSessionRepository
    {
        private readonly AppDbConext _context;
        public AttendanceSessionRepository(AppDbConext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<AttendanceSession>> GetSessionsByHallIdAsync(int hallId)
        {
            return await _context.AttendanceSessions.Where(s => s.HallId == hallId).ToListAsync();
        }
    }
}
