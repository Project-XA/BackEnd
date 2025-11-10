using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using Project_X.Models;

namespace Project_X.Data.Configuration
{
    public class AttendanceSeesionConfiguration : IEntityTypeConfiguration<AttendanceSession>
    {
        public void Configure(EntityTypeBuilder<AttendanceSession> builder)
        {
            builder.HasKey(s => s.SessionId);
            builder.HasMany(s => s.Verifications)
                .WithOne(v => v.Session)
                .HasForeignKey(v => v.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(s => s.ConnectionType)
                .HasConversion<string>();
            builder.HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
