using Models;
using Project_X.Data.Context;
using Project_X.Data.Repositories;
using Project_X.Data.Repository;
using Project_X.Models;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Project_X.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbConext _context;
        public IRepository<AppUser> Users { get; }
        public IOrganizationRepository Organizations  {get;}
        public IRepository<OrganizationUser> OrganizationUsers { get; }
        public IRepository<AttendanceLog> AttendanceLogs { get; }
        public IAttendanceSessionRepository AttendanceSessions { get; }
        public IRepository<LocationBeacon> Beacons { get; }
        public IRepository<VerificationSession> VerificationSessions { get; }
        public HallRepository Halls { get; }
        public IOtpRepository OTPs { get; }

        public UnitOfWork(AppDbConext context)
        {
            _context = context;
            Users = new Repository<AppUser>(_context);
            Organizations = new OrganizationRepository(_context);
            OrganizationUsers = new Repository<OrganizationUser>(_context);
            AttendanceLogs = new Repository<AttendanceLog>(_context);
            AttendanceSessions = new AttendanceSessionRepository(_context);
            Beacons = new Repository<LocationBeacon>(_context);
            VerificationSessions = new Repository<VerificationSession>(_context);
            Halls = new HallRepository(_context);
            OTPs = new OtpRepository(_context);
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
