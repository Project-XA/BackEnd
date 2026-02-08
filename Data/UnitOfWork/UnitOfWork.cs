using Models;
using Project_X.Data.Context;
using Project_X.Data.Repositories;
using Project_X.Data.Repository;
using Project_X.Models;

namespace Project_X.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbConext _context;
        public IRepository<AppUser> Users { get; }
        public IOrganizationRepository Organizations  {get;}
        public IOrganizationUserRepository OrganizationUsers { get; }
        public IAttendanceLogRepository AttendanceLogs { get; }
        public IAttendanceSessionRepository AttendanceSessions { get; }
        public IRepository<LocationBeacon> Beacons { get; }
        public IRepository<VerificationSession> VerificationSessions { get; }
        public IHallRepository Halls { get; }
        public IOtpRepository OTPs { get; }
        public IRepository<OrganizationEvent> OrganizationEvents { get; }

        public UnitOfWork(AppDbConext context)
        {
            _context = context;
            Users = new Repository<AppUser>(_context);
            Organizations = new OrganizationRepository(_context);
            OrganizationUsers = new OrganizationUserRepository(_context);
            AttendanceLogs = new AttendanceLogRepository(_context);
            AttendanceSessions = new AttendanceSessionRepository(_context);
            Beacons = new Repository<LocationBeacon>(_context);
            VerificationSessions = new Repository<VerificationSession>(_context);
            Halls = new HallRepository(_context);
            OTPs = new OtpRepository(_context);
            OrganizationEvents = new Repository<OrganizationEvent>(_context);
        }
    
        public async Task<int> SaveAsync()
        {
           return await _context.SaveChangesAsync();
        }

        void IDisposable.Dispose()
        {
            _context.Dispose();
        }
    }
}

