using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting
{
    public class InvoiceUSAConfiguration : IEntityTypeConfiguration<InvoiceUSA>
    {
        public void Configure(EntityTypeBuilder<InvoiceUSA> builder)
        {
            builder
                .HasMany(e => e.Discounts)
                .WithOne(t => t.InvoiceUSA)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Items)
                .WithOne(t => t.InvoiceUSA)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(i => i.NumberId).ValueGeneratedOnAdd();
            builder.HasIndex(i => i.InvoiceNumber).IsUnique();
            builder.Property(i => i.InvoiceNumber).IsRequired();
            builder.HasIndex(i => i.InvoiceNumberId).IsUnique();
        }
    }
}