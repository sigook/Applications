using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

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
