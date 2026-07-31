using Covenant.Common.Entities.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Deductions;

public class ProvincialTaxMonthlyConfiguration : IEntityTypeConfiguration<ProvincialTaxMonthly>
{
    public void Configure(EntityTypeBuilder<ProvincialTaxMonthly> builder)
    {
        builder.ToTable("ProvincialTaxMonthly");
        builder.HasKey(x => x.Id);
    }
}
