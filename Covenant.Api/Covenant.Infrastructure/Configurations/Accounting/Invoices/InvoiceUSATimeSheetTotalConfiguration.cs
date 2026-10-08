using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Invoices;

public class InvoiceUSATimeSheetTotalConfiguration : IEntityTypeConfiguration<InvoiceUSATimeSheetTotal>
{
    public void Configure(EntityTypeBuilder<InvoiceUSATimeSheetTotal> builder)
    {
        builder.ToTable("InvoiceUSATimeSheetTotals");
        builder.HasKey(k => new { k.InvoiceUSAId, k.TimeSheetTotalId });

        builder.HasOne(x => x.InvoiceUSA)
            .WithMany(x => x.TimeSheetTotals)
            .HasForeignKey(x => x.InvoiceUSAId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TimeSheetTotal)
            .WithMany()
            .HasForeignKey(x => x.TimeSheetTotalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
