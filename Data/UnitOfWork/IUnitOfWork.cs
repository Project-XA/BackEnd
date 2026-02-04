using Models;
using Project_X.Data.Repositories;
using Project_X.Models;

namespace Project_X.Data.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        public IRepository<AppUser> Users { get; }
        public IOrganizationRepository Organizations { get; }
        public IOrganizationUserRepository OrganizationUsers { get; }
        public IAttendanceLogRepository AttendanceLogs { get; }
        public IAttendanceSessionRepository AttendanceSessions { get; }
        public IRepository<LocationBeacon> Beacons { get; }
        public IRepository<VerificationSession> VerificationSessions { get; }
        public IHallRepository Halls { get; }
        public IOtpRepository OTPs { get; }
        Task<int> SaveAsync();
    }
}
