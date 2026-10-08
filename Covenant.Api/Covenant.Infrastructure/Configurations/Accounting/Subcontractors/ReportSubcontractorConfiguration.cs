using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Accounting.Subcontractors;

public class ReportSubcontractorConfiguration : IEntityTypeConfiguration<ReportSubcontractor>
{
    public void Configure(EntityTypeBuilder<ReportSubcontractor> builder)
    {
        builder.ToTable("ReportSubcontractors");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.WorkerProfile)
            .WithMany()
            .HasForeignKey(x => x.WorkerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
