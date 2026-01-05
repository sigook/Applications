using Covenant.Common.Entities.Accounting.Invoice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting
{
    public class InvoiceUSATimeSheetTotalConfiguration : IEntityTypeConfiguration<InvoiceUSATimeSheetTotal>
    {
        public void Configure(EntityTypeBuilder<InvoiceUSATimeSheetTotal> builder)
        {
            builder.ToTable("InvoiceUSATimeSheetTotal");
            builder.HasKey(k => new { k.InvoiceUSAId, k.TimeSheetTotalId });
        }
    }
} 