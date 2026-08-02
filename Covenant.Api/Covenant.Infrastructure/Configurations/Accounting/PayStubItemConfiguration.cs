using Covenant.Common.Entities.Accounting.PayStub;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

public class PayStubItemConfiguration : IEntityTypeConfiguration<PayStubItem>
{
    public void Configure(EntityTypeBuilder<PayStubItem> builder)
    {
        builder.ToTable("PayStubItems");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.PayStub)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.PayStubId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
