using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker
{
    public class WorkerProfileAvailabilityConfiguration : IEntityTypeConfiguration<WorkerProfileAvailability>
    {
        public void Configure(EntityTypeBuilder<WorkerProfileAvailability> builder)
        {
            builder.ToTable("WorkerProfileAvailability");
            builder.HasKey(a => new { a.WorkerProfileId, a.AvailabilityId });
        }
    }
} 