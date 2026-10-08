using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Invoices;

public class InvoiceUSADiscountConfiguration : IEntityTypeConfiguration<InvoiceUSADiscount>
{
    public void Configure(EntityTypeBuilder<InvoiceUSADiscount> builder)
    {
        builder.ToTable("InvoiceUSADiscounts");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.InvoiceUSA)
            .WithMany(x => x.Discounts)
            .HasForeignKey(x => x.InvoiceUSAId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
