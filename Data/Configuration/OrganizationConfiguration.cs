
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace Project_X.Data.Configuration
{
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasKey(o => o.OrganizationId);
            builder.HasMany(o => o.Sessions)
                .WithOne(s => s.Organization)
                .HasForeignKey(s => s.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
