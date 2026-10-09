using Covenant.Common.Entities.Accounting.PayStub;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.PayStubs;

internal class PayStubConfiguration : IEntityTypeConfiguration<PayStub>
{
    public void Configure(EntityTypeBuilder<PayStub> builder)
    {
        builder.ToTable("PayStubs");
        builder.Property(i => i.NumberId).ValueGeneratedOnAdd();
    }
}
