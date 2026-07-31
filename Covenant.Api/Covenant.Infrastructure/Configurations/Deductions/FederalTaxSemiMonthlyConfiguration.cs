using Covenant.Common.Entities.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Deductions;

public class FederalTaxSemiMonthlyConfiguration : IEntityTypeConfiguration<FederalTaxSemiMonthly>
{
    public void Configure(EntityTypeBuilder<FederalTaxSemiMonthly> builder)
    {
        builder.ToTable("FederalTaxSemiMonthly");
        builder.HasKey(x => x.Id);
    }
}
