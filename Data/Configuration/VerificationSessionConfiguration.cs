using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project_X.Models;

namespace Project_X.Data.Configuration
{
    public class VerificationSessionConfiguration : IEntityTypeConfiguration<VerificationSession>
    {
        public void Configure(EntityTypeBuilder<VerificationSession> builder)
        {
            builder.HasKey(v => v.VerificationId);
            builder.Property(v => v.VerificationType)
                .HasConversion<string>();
            builder.HasOne(v => v.User)
                .WithMany(u => u.VerificationSessions)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(v => v.Log)
                .WithOne(l => l.VerificationSession)
                .HasForeignKey<AttendanceLog>(l => l.VerificationId)
                .OnDelete(DeleteBehavior.Cascade);
           
        }
    }
}
