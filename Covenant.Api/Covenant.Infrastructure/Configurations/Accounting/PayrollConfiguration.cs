using Covenant.Common.Entities.Accounting.PayStub;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting
{
    internal class PayStubConfiguration : IEntityTypeConfiguration<PayStub>
    {
        public void Configure(EntityTypeBuilder<PayStub> builder)
        {
            builder.HasMany(s => s.WageDetails)
                .WithOne(t => t.PayStub)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Holidays)
                .WithOne(t => t.PayStub)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Items)
                .WithOne(t => t.PayStub)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(i => i.NumberId).ValueGeneratedOnAdd();
        }
    }
}