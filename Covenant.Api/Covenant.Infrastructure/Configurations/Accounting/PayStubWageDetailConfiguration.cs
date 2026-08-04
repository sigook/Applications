using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

public class PayStubWageDetailConfiguration : IEntityTypeConfiguration<PayStubWageDetail>
{
    public void Configure(EntityTypeBuilder<PayStubWageDetail> builder)
    {
        builder.ToTable("PayStubWageDetails");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.PayStub)
            .WithMany(x => x.WageDetails)
            .HasForeignKey(x => x.PayStubId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TimeSheetTotal)
            .WithMany()
            .HasForeignKey(x => x.TimeSheetTotalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
