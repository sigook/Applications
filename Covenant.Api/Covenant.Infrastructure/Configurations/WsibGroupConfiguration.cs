using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations;

public class WsibGroupConfiguration : IEntityTypeConfiguration<WsibGroup>
{
    public void Configure(EntityTypeBuilder<WsibGroup> builder)
    {
        builder.ToTable("WsibGroups");
        builder.HasKey(x => x.Id);
    }
}
