using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Invoices;

public class SkipPayrollNumberConfiguration : IEntityTypeConfiguration<SkipPayrollNumber>
{
    public void Configure(EntityTypeBuilder<SkipPayrollNumber> builder)
    {
        builder.ToTable("SkipPayrollNumbers");
        builder.HasKey(e => e.Id);
    }
}
