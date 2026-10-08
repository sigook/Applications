using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Invoices;

public class InvoiceAdditionalDetailConfiguration : IEntityTypeConfiguration<InvoiceAdditionalDetail>
{
    public void Configure(EntityTypeBuilder<InvoiceAdditionalDetail> builder)
    {
        builder.ToTable("InvoiceAdditionalDetails");
        builder.HasKey(k => k.Id);

        builder.HasOne(d => d.CanadaInvoice)
            .WithOne(i => i.AdditionalDetail)
            .HasForeignKey<InvoiceAdditionalDetail>(d => d.CanadaInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.UsaInvoice)
            .WithOne(i => i.AdditionalDetail)
            .HasForeignKey<InvoiceAdditionalDetail>(d => d.UsaInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
