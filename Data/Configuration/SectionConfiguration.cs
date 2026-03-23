using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project_X.Models;

namespace Project_X.Data.Configuration
{
    public class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            builder.HasKey(s => s.SectionId);
            builder.HasIndex(s => s.SectionCode).IsUnique();
            builder.HasOne(s => s.Organization)
                .WithMany(o => o.Sections)
                .HasForeignKey(s => s.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
