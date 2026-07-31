using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker
{
    public class WorkerProfileAvailabilityTimeConfiguration : IEntityTypeConfiguration<WorkerProfileAvailabilityTime>
    {
        public void Configure(EntityTypeBuilder<WorkerProfileAvailabilityTime> builder)
        {
            builder.ToTable("WorkerProfileAvailabilityTimes");
            builder.HasKey(a => new { a.WorkerProfileId, a.AvailabilityTimeId });
        }
    }
} 