using Covenant.Common.Entities.Accounting.Deductions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Deductions;

public class TaxDeductionConfiguration : IEntityTypeConfiguration<TaxDeduction>
{
    public void Configure(EntityTypeBuilder<TaxDeduction> builder)
    {
        builder.ToTable("TaxDeductions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Year, x.PayPeriod, x.TaxType, x.From, x.To });
    }
}
