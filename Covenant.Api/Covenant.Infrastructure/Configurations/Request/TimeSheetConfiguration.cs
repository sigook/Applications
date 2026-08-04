using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request;

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
