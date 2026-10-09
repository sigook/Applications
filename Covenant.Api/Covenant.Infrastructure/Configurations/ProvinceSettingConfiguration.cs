using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations;

public class ProvinceSettingConfiguration : IEntityTypeConfiguration<ProvinceSetting>
{
    public void Configure(EntityTypeBuilder<ProvinceSetting> builder)
    {
        builder.ToTable("ProvinceSettings");
        builder.HasKey(ps => ps.ProvinceId);
    }
}
