using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

public class InvoiceUSAItemConfiguration : IEntityTypeConfiguration<InvoiceUSAItem>
{
    public void Configure(EntityTypeBuilder<InvoiceUSAItem> builder)
    {
        builder.ToTable("InvoiceUSAItem");
        builder.HasKey(i => i.Id);
    }
}
