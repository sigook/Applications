using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting
{
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
}
