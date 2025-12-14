using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Project_X.Data.Configuration;
using Project_X.Models;

namespace Project_X.Data.Context
{
    public class AppDbConext:IdentityDbContext<AppUser>
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<AttendanceLog> AttendanceLogs { get; set; }
        public DbSet<AttendanceSession> AttendanceSessions { get; set; }
        public DbSet<LocationBeacon> Beacons { get; set; }
        public DbSet<VerificationSession> VerificationSessions { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<OrganizationUser> OrganizationUser { get; set; }
        public AppDbConext(DbContextOptions dbContextOptions)
            : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(OrganizationConfiguration).Assembly);
            builder.Entity<AppUser>().Property(u=>u.Role)
                .HasConversion<string>();
            builder.Entity<Hall>().HasKey(hall => new { hall.Id});
        }
    }
}
