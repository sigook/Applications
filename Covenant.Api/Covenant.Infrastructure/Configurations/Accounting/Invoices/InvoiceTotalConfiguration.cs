using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Invoices;

public class InvoiceTotalConfiguration : IEntityTypeConfiguration<InvoiceTotal>
{
    public void Configure(EntityTypeBuilder<InvoiceTotal> builder)
    {
        builder.ToTable("InvoiceTotals");
        builder.HasKey(k => k.Id);

        builder.HasOne(x => x.Invoice)
            .WithMany(x => x.InvoiceTotals)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TimeSheetTotal)
            .WithMany()
            .HasForeignKey(x => x.TimeSheetTotalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
