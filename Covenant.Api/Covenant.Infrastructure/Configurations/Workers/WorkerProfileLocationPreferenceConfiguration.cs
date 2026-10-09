using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

public class WorkerProfileLocationPreferenceConfiguration : IEntityTypeConfiguration<WorkerProfileLocationPreference>
{
    public void Configure(EntityTypeBuilder<WorkerProfileLocationPreference> builder)
    {
        builder.ToTable("WorkerProfileLocationPreferences");
        builder.HasKey(a => new { a.WorkerProfileId, a.CityId });
    }
}
