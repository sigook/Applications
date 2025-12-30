using Covenant.Common.Entities.Agency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Agency
{
    public class AgencyWsibGroupConfiguration : IEntityTypeConfiguration<AgencyWsibGroup>
    {
        public void Configure(EntityTypeBuilder<AgencyWsibGroup> builder)
        {
            builder.HasKey(c => new
            {
                c.AgencyId,
                c.WsibGroupId
            });
        }
    }
}