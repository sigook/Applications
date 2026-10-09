using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Invoices;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    private const int MaximumLengthStatus = 20;

    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.Property(i => i.NumberId).ValueGeneratedOnAdd();

        builder.Property(i => i.Status)
            .HasConversion(new EnumToStringConverter<InvoiceStatus>())
            .HasMaxLength(MaximumLengthStatus)
            .HasDefaultValue(InvoiceStatus.Pending);

        builder.HasOne(i => i.UpdatedByUser)
            .WithMany()
            .HasForeignKey(i => i.UpdatedBy)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
