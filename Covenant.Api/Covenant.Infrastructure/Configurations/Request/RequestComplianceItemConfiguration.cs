using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Covenant.Infrastructure.Configurations.Request;

public class RequestComplianceItemConfiguration : IEntityTypeConfiguration<RequestComplianceItem>
{
    private const int MaximumLengthName = 200;
    private const int MaximumLengthDocumentTarget = 30;

    public void Configure(EntityTypeBuilder<RequestComplianceItem> builder)
    {
        builder.ToTable("RequestComplianceItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(MaximumLengthName)
            .IsRequired();

        builder.Property(x => x.DocumentTarget)
            .HasConversion(new EnumToStringConverter<ComplianceDocumentTarget>())
            .HasMaxLength(MaximumLengthDocumentTarget);

        builder.HasOne(x => x.Request)
            .WithMany(r => r.ComplianceItems)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
