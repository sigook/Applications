using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker
{
    public class WorkerProfileHolidayConfiguration : IEntityTypeConfiguration<WorkerProfileHoliday>
    {
        public void Configure(EntityTypeBuilder<WorkerProfileHoliday> builder)
        {
            builder.ToTable("WorkerProfileHoliday");
            builder.HasKey(k => k.Id);
            builder.HasIndex(h => new { h.WorkerProfileId, h.HolidayId }).IsUnique();
        }
    }
} 