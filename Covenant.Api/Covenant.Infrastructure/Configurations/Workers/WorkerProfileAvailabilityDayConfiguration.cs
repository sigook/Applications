using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

public class WorkerProfileAvailabilityDayConfiguration : IEntityTypeConfiguration<WorkerProfileAvailabilityDay>
{
    public void Configure(EntityTypeBuilder<WorkerProfileAvailabilityDay> builder)
    {
        builder.ToTable("WorkerProfileAvailabilityDays");
        builder.HasKey(a => new { a.WorkerProfileId, a.DayId });
    }
}
