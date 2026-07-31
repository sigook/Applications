using Covenant.Common.Entities.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Deductions;

public class ProvincialTaxWeeklyConfiguration : IEntityTypeConfiguration<ProvincialTaxWeekly>
{
    public void Configure(EntityTypeBuilder<ProvincialTaxWeekly> builder)
    {
        builder.ToTable("ProvincialTaxWeekly");
        builder.HasKey(x => x.Id);
    }
}
