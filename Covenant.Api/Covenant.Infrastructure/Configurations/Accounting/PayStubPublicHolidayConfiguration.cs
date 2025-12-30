using Covenant.Common.Entities.Accounting.PayStub;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting
{
    public class PayStubPublicHolidayConfiguration : IEntityTypeConfiguration<PayStubPublicHoliday>
    {
        public void Configure(EntityTypeBuilder<PayStubPublicHoliday> builder)
        {
            builder.ToTable("PayStubPublicHoliday");
            builder.HasKey(k => k.Id);
        }
    }
} 