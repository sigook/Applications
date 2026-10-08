using Covenant.Common.Entities.Accounting.PayStub;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.PayStubs;

public class PayStubPublicHolidayConfiguration : IEntityTypeConfiguration<PayStubPublicHoliday>
{
    public void Configure(EntityTypeBuilder<PayStubPublicHoliday> builder)
    {
        builder.ToTable("PayStubPublicHolidays");
        builder.HasKey(k => k.Id);

        builder.HasOne(x => x.PayStub)
            .WithMany(x => x.Holidays)
            .HasForeignKey(x => x.PayStubId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
