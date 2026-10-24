using Covenant.Common.Entities.Accounting.Invoices;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Invoices;

public class InvoiceAdditionalDetailConfiguration : IEntityTypeConfiguration<InvoiceAdditionalDetail>
{
    public void Configure(EntityTypeBuilder<InvoiceAdditionalDetail> builder)
    {
        builder.ToTable("InvoiceAdditionalDetails");
        builder.HasKey(k => k.Id);

        builder.HasOne(d => d.Invoice)
            .WithOne(i => i.AdditionalDetail)
            .HasForeignKey<InvoiceAdditionalDetail>(d => d.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
