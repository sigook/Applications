using Covenant.Common.Entities.Agency;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Agencies;

public class AgencyLocationConfiguration : IEntityTypeConfiguration<AgencyLocation>
{
    public void Configure(EntityTypeBuilder<AgencyLocation> builder)
    {
        builder.ToTable("AgencyLocations");
        builder.HasKey(a => new { a.AgencyId, a.LocationId });
    }
}
