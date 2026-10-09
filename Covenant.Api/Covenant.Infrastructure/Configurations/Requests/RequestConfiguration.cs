using Covenant.Common.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Requests;

public class RequestConfiguration : IEntityTypeConfiguration<Covenant.Common.Entities.Request.Request>
{
    public void Configure(EntityTypeBuilder<Covenant.Common.Entities.Request.Request> builder)
    {
        builder.ToTable("Requests");
        builder.Property(e => e.NumberId).ValueGeneratedOnAdd();
        builder.Property(e => e.CreatedAt)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Status).HasConversion(new EnumToStringConverter<RequestStatus>());
        builder.Property(e => e.DurationTerm).HasConversion(new EnumToStringConverter<DurationTerm>());

        builder.HasOne(e => e.CompanyProfile)
            .WithMany()
            .HasForeignKey(e => e.CompanyProfileId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.Recruiters)
            .WithOne(o => o.Request)
            .HasForeignKey(f => f.RequestId).IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(m => m.Workers)
            .WithOne(o => o.Request)
            .HasForeignKey(f => f.RequestId).IsRequired()
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(m => m.Sources)
            .WithOne(o => o.Request)
            .HasForeignKey(f => f.RequestId).IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
