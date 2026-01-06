using Covenant.Common.Entities.Agency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Agency
{
    public class AgencyLocationConfiguration : IEntityTypeConfiguration<AgencyLocation>
    {
        public void Configure(EntityTypeBuilder<AgencyLocation> builder)
        {
            builder.ToTable("AgencyLocation");
            builder.HasKey(a => new { a.AgencyId, a.LocationId });
        }
    }
}
