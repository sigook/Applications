using Covenant.Common.Entities.Accounting.Subcontractor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting
{
    public class ReportSubcontractorPublicHolidayConfiguration : IEntityTypeConfiguration<ReportSubcontractorPublicHoliday>
    {
        public void Configure(EntityTypeBuilder<ReportSubcontractorPublicHoliday> builder)
        {
            builder.ToTable("ReportSubcontractorPublicHoliday");
            builder.HasKey(k => k.Id);
        }
    }
}