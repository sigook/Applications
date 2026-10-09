using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Requests;

public class TimeSheetConfiguration : IEntityTypeConfiguration<TimeSheet>
{
    public void Configure(EntityTypeBuilder<TimeSheet> builder)
    {
        builder.ToTable("TimeSheets");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.WorkerRequest)
            .WithMany(x => x.TimeSheets)
            .HasForeignKey(x => x.WorkerRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
