using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker
{
    public class WorkerProfileAvailabilityDayConfiguration : IEntityTypeConfiguration<WorkerProfileAvailabilityDay>
    {
        public void Configure(EntityTypeBuilder<WorkerProfileAvailabilityDay> builder)
        {
            builder.ToTable("WorkerProfileAvailabilityDay");
            builder.HasKey(a => new { a.WorkerProfileId, a.DayId });
        }
    }
} 