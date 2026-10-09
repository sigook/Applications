using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Invoices;

public class InvoiceUSAConfiguration : IEntityTypeConfiguration<InvoiceUSA>
{
    private const int MaximumLengthStatus = 20;

    public void Configure(EntityTypeBuilder<InvoiceUSA> builder)
    {
        builder.ToTable("InvoicesUSA");
        builder.Property(i => i.NumberId).ValueGeneratedOnAdd();
        builder.HasIndex(i => i.InvoiceNumber).IsUnique();
        builder.Property(i => i.InvoiceNumber).IsRequired();
        builder.HasIndex(i => i.InvoiceNumberId).IsUnique();

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
