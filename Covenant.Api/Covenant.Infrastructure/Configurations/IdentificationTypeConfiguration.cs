using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Covenant.Infrastructure.Configurations;

public class IdentificationTypeConfiguration : IEntityTypeConfiguration<IdentificationType>
{
    private const int MaximumLengthCode = 50;

    public void Configure(EntityTypeBuilder<IdentificationType> builder)
    {
        builder.ToTable("IdentificationTypes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasConversion(new EnumToStringConverter<IdentificationTypeCode>())
            .HasMaxLength(MaximumLengthCode)
            .HasDefaultValue(IdentificationTypeCode.None)
            .IsRequired();
    }
}
