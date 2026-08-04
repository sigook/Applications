using Covenant.Common.Entities.Accounting.PayStub;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting
{
    internal class PayStubConfiguration : IEntityTypeConfiguration<PayStub>
    {
        public void Configure(EntityTypeBuilder<PayStub> builder)
        {
            builder.ToTable("PayStubs");
            builder.Property(i => i.NumberId).ValueGeneratedOnAdd();
        }
    }
}