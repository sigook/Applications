using Covenant.Common.Entities.Accounting.Subcontractor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Accounting;

public class ReportSubContractorOtherDeductionConfiguration : IEntityTypeConfiguration<ReportSubContractorOtherDeduction>
{
    public void Configure(EntityTypeBuilder<ReportSubContractorOtherDeduction> builder)
    {
        builder.ToTable("ReportSubContractorOtherDeductions");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.ReportSubcontractor)
            .WithMany(x => x.OtherDeductionsDetail)
            .HasForeignKey(x => x.ReportSubcontractorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
