using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

public class SkipPayrollNumberConfiguration : IEntityTypeConfiguration<SkipPayrollNumber>
{
    public void Configure(EntityTypeBuilder<SkipPayrollNumber> builder)
    {
        builder.ToTable("SkipPayrollNumber");
        builder.HasKey(e => e.Id);
    }
}
