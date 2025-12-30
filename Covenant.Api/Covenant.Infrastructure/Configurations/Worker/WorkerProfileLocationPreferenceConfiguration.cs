using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker
{
    public class WorkerProfileLocationPreferenceConfiguration : IEntityTypeConfiguration<WorkerProfileLocationPreference>
    {
        public void Configure(EntityTypeBuilder<WorkerProfileLocationPreference> builder)
        {
            builder.ToTable("WorkerProfileLocationPreference");
            builder.HasKey(a => new { a.WorkerProfileId, a.CityId });
        }
    }
} 