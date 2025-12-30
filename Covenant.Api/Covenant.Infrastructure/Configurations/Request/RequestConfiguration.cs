using Covenant.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class RequestConfiguration : IEntityTypeConfiguration<Covenant.Common.Entities.Request.Request>
    {
        public void Configure(EntityTypeBuilder<Covenant.Common.Entities.Request.Request> builder)
        {
            builder.Property(e => e.NumberId).ValueGeneratedOnAdd();
            builder.Property(e => e.CreatedAt)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Status).HasConversion(new EnumToStringConverter<RequestStatus>());
            builder.Property(e => e.DurationTerm).HasConversion(new EnumToStringConverter<DurationTerm>());

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
        }
    }
}