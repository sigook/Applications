using Covenant.Common.Entities.Accounting.Deductions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Deductions;

public class CppDeductionConfiguration : IEntityTypeConfiguration<CppDeduction>
{
    public void Configure(EntityTypeBuilder<CppDeduction> builder)
    {
        builder.ToTable("CppDeductions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Year, x.PayPeriod, x.From, x.To });
    }
}
