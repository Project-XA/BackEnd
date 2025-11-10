using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project_X.Models;

namespace Project_X.Data.Configuration
{
    public class AttendanceLogConfiguration : IEntityTypeConfiguration<AttendanceLog>
    {
        public void Configure(EntityTypeBuilder<AttendanceLog> builder)
        {
            builder.HasKey(l => l.LogId);
            builder.Property(l => l.Result)
                .HasConversion<string>();
            builder.HasOne(l => l.Session)
                .WithMany(s => s.Logs)
                .HasForeignKey(l => l.SessionId);
            builder.HasOne(l => l.User)
                .WithMany(u => u.Logs)
                .HasForeignKey(l => l.UserId);
            
        }
    }
}
