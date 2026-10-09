using Covenant.Common.Entities.Accounting.Subcontractor;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Subcontractors;

public class ReportSubcontractorPublicHolidayConfiguration : IEntityTypeConfiguration<ReportSubcontractorPublicHoliday>
{
    public void Configure(EntityTypeBuilder<ReportSubcontractorPublicHoliday> builder)
    {
        builder.ToTable("ReportSubcontractorPublicHolidays");
        builder.HasKey(k => k.Id);

        builder.HasOne(x => x.ReportSubcontractor)
            .WithMany(x => x.Holidays)
            .HasForeignKey(x => x.ReportSubcontractorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
