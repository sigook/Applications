using Covenant.Common.Entities.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Deductions;

public class FederalTaxMonthlyConfiguration : IEntityTypeConfiguration<FederalTaxMonthly>
{
    public void Configure(EntityTypeBuilder<FederalTaxMonthly> builder)
    {
        builder.ToTable("FederalTaxMonthly");
        builder.HasKey(x => x.Id);
    }
}
