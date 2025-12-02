using Models;
using Project_X.Data.Repository;
using Project_X.Models;

namespace Project_X.Data.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        public IRepository<AppUser> Users { get; }
        public IRepository<Organization> Organizations { get; }
        public IRepository<AttendanceLog> AttendanceLogs { get; }
        public IRepository<AttendanceSession> AttendanceSessions { get; }
        public IRepository<LocationBeacon> Beacons { get; }
        public IRepository<VerificationSession> VerificationSessions { get; }
        public IRepository<Hall> Halls { get; }
        public IRepository<OTP> OTPs { get; }
        Task<int> SaveAsync();
    }
}
