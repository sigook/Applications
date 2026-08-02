using Covenant.Common.Entities.Accounting.Deductions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

public class CppDeductionConfiguration : IEntityTypeConfiguration<CppDeduction>
{
    public void Configure(EntityTypeBuilder<CppDeduction> builder)
    {
        builder.ToTable("CppDeductions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Year, x.PayPeriod, x.From, x.To });
    }
}
