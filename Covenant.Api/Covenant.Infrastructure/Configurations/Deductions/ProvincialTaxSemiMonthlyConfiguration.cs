using Covenant.Common.Entities.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Deductions;

public class ProvincialTaxSemiMonthlyConfiguration : IEntityTypeConfiguration<ProvincialTaxSemiMonthly>
{
    public void Configure(EntityTypeBuilder<ProvincialTaxSemiMonthly> builder)
    {
        builder.ToTable("ProvincialTaxSemiMonthly");
        builder.HasKey(x => x.Id);
    }
}
