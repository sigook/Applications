using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations;

public class LocationTaxConfiguration : IEntityTypeConfiguration<LocationTax>
{
    public void Configure(EntityTypeBuilder<LocationTax> builder)
    {
        builder.ToTable("LocationTaxes");
        builder.HasKey(lt => lt.LocationId);
        builder.HasOne(lt => lt.Location)
            .WithOne(l => l.LocationTax)
            .HasForeignKey<LocationTax>(lt => lt.LocationId);
    }
}
