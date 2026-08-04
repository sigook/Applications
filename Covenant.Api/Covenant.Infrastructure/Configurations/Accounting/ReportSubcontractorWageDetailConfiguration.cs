using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

public class ReportSubcontractorWageDetailConfiguration : IEntityTypeConfiguration<ReportSubcontractorWageDetail>
{
    public void Configure(EntityTypeBuilder<ReportSubcontractorWageDetail> builder)
    {
        builder.ToTable("ReportSubcontractorWageDetails");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.ReportSubcontractor)
            .WithMany(x => x.WageDetails)
            .HasForeignKey(x => x.ReportSubcontractorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TimeSheetTotal)
            .WithMany()
            .HasForeignKey(x => x.TimeSheetTotalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
