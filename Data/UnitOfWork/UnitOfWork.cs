using Models;
using Project_X.Data.Context;
using Project_X.Data.Repository;
using Project_X.Models;

namespace Project_X.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbConext _context;
        public IRepository<AppUser> Users { get; }
        public IRepository<Organization> Organizations  {get;}
        public IRepository<AttendanceLog> AttendanceLogs { get; }
        public IRepository<AttendanceSession> AttendanceSessions { get; }
        public IRepository<LocationBeacon> Beacons { get; }
        public IRepository<VerificationSession> VerificationSessions { get; }
        public IRepository<Hall> Halls { get; }

        public UnitOfWork(AppDbConext context)
        {
            _context = context;
            Users = new Repository<AppUser>(_context);
            Organizations = new Repository<Organization>(_context);
            AttendanceLogs = new Repository<AttendanceLog>(_context);
            AttendanceSessions = new Repository<AttendanceSession>(_context);
            Beacons = new Repository<LocationBeacon>(_context);
            VerificationSessions = new Repository<VerificationSession>(_context);
            Halls = new Repository<Hall>(_context);
        }
        public void Dispose()
        {
          _context.Dispose();
        }

        public async Task<int> SaveAsync()
        {
           return await _context.SaveChangesAsync();
        }
    }
}
