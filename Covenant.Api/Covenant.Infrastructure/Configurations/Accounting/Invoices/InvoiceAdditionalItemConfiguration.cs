using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Invoices;

public class InvoiceAdditionalItemConfiguration : IEntityTypeConfiguration<InvoiceAdditionalItem>
{
    public void Configure(EntityTypeBuilder<InvoiceAdditionalItem> builder)
    {
        builder.ToTable("InvoiceAdditionalItems");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Invoice)
            .WithMany(x => x.AdditionalItems)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
