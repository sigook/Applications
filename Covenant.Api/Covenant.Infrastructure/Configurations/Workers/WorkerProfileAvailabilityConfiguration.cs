using Covenant.Common.Entities.Workers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

public class WorkerProfileAvailabilityConfiguration : IEntityTypeConfiguration<WorkerProfileAvailability>
{
    public void Configure(EntityTypeBuilder<WorkerProfileAvailability> builder)
    {
        builder.ToTable("WorkerProfileAvailabilities");
        builder.HasKey(a => new { a.WorkerProfileId, a.AvailabilityId });
    }
}
