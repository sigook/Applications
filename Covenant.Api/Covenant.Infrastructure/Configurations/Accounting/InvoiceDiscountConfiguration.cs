using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

public class InvoiceDiscountConfiguration : IEntityTypeConfiguration<InvoiceDiscount>
{
    public void Configure(EntityTypeBuilder<InvoiceDiscount> builder)
    {
        builder.ToTable("InvoiceDiscounts");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Invoice)
            .WithMany(x => x.Discounts)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
