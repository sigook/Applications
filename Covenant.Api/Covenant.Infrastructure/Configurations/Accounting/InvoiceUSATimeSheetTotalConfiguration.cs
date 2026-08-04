using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting
{
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
} 