using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder
                .HasMany(e => e.InvoiceTotals)
                .WithOne(t => t.Invoice)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(i => i.NumberId).ValueGeneratedOnAdd();
        }
    }
}