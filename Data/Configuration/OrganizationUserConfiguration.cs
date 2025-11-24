using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using Project_X.Models;
using System.Reflection.Emit;

namespace Project_X.Data.Configuration
{
    public class OrganizationUserConfiguration : IEntityTypeConfiguration<OrganizationUser>
    {
        public void Configure(EntityTypeBuilder<OrganizationUser> builder)
        {
            builder.HasKey(ou => new { ou.OrganizationId, ou.UserId });

            builder.HasOne(ou => ou.Organization)
                .WithMany(org => org.OrganizationUsers)
                .HasForeignKey(ou => ou.OrganizationId);

            builder.HasOne(ou => ou.User)
                .WithMany(user => user.OrganizationUsers)
                .HasForeignKey(ou => ou.UserId);
        }
    }
}
