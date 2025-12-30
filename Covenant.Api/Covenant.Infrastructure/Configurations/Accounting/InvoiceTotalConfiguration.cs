using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

public class InvoiceTotalConfiguration : IEntityTypeConfiguration<InvoiceTotal>
{
    public void Configure(EntityTypeBuilder<InvoiceTotal> builder)
    {
        builder.ToTable("InvoiceTotal");
        builder.HasKey(k => k.Id);
    }
}
