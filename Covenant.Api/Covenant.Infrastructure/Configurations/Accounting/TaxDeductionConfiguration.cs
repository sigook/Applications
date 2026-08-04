using Covenant.Common.Entities.Accounting.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

public class TaxDeductionConfiguration : IEntityTypeConfiguration<TaxDeduction>
{
    public void Configure(EntityTypeBuilder<TaxDeduction> builder)
    {
        builder.ToTable("TaxDeductions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Year, x.PayPeriod, x.TaxType, x.From, x.To });
    }
}
