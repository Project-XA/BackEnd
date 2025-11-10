using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project_X.Models;

namespace Project_X.Data.Configuration
{
    public class LocationBeaconConfiguration : IEntityTypeConfiguration<LocationBeacon>
    {
        public void Configure(EntityTypeBuilder<LocationBeacon> builder)
        {
            builder.HasKey(b => b.BeaconId);
            builder.Property(b => b.BeaconType)
                .HasConversion<string>();
            builder.HasOne(b => b.Session)
                .WithMany(s => s.Beacons)
                .HasForeignKey(s => s.BeaconId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
