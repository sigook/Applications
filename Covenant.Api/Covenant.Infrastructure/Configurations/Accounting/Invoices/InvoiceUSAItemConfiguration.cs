using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Invoices;

public class InvoiceUSAItemConfiguration : IEntityTypeConfiguration<InvoiceUSAItem>
{
    public void Configure(EntityTypeBuilder<InvoiceUSAItem> builder)
    {
        builder.ToTable("InvoiceUSAItems");
        builder.HasKey(i => i.Id);

        builder.HasOne(x => x.InvoiceUSA)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.InvoiceUSAId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TimeSheetTotal)
            .WithMany()
            .HasForeignKey(x => x.TimeSheetTotalId)
            .IsRequired(false);
    }
}
