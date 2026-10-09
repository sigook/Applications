using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

public class WorkerProfileAvailabilityTimeConfiguration : IEntityTypeConfiguration<WorkerProfileAvailabilityTime>
{
    public void Configure(EntityTypeBuilder<WorkerProfileAvailabilityTime> builder)
    {
        builder.ToTable("WorkerProfileAvailabilityTimes");
        builder.HasKey(a => new { a.WorkerProfileId, a.AvailabilityTimeId });
    }
}
