using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using Project_X.Models;

namespace Project_X.Data.Configuration
{
    public class SectionUserConfiguration : IEntityTypeConfiguration<SectionUser>
    {
        public void Configure(EntityTypeBuilder<SectionUser> builder)
        {
            builder.HasKey(su => new { su.SectionId, su.UserId });

            builder.HasOne(su => su.Section)
                .WithMany(s => s.SectionUsers)
                .HasForeignKey(su => su.SectionId);

            builder.HasOne(su => su.User)
                .WithMany(u => u.SectionUsers)
                .HasForeignKey(su => su.UserId);
        }
    }
}
