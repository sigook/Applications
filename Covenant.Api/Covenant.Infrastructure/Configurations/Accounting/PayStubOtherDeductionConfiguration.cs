using Covenant.Common.Entities.Accounting.PayStub;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

public class PayStubOtherDeductionConfiguration : IEntityTypeConfiguration<PayStubOtherDeduction>
{
    public void Configure(EntityTypeBuilder<PayStubOtherDeduction> builder)
    {
        builder.ToTable("PayStubOtherDeductions");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.PayStub)
            .WithMany(x => x.OtherDeductions)
            .HasForeignKey(x => x.PayStubId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
