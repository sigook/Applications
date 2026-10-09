using Covenant.Common.Entities.Agency;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Agencies;

public class AgencyWsibGroupConfiguration : IEntityTypeConfiguration<AgencyWsibGroup>
{
    public void Configure(EntityTypeBuilder<AgencyWsibGroup> builder)
    {
        builder.ToTable("AgencyWsibGroups");
        builder.HasKey(c => new
        {
            c.AgencyId,
            c.WsibGroupId
        });
    }
}
