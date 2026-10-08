using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

public class WorkerProfileHolidayConfiguration : IEntityTypeConfiguration<WorkerProfileHoliday>
{
    public void Configure(EntityTypeBuilder<WorkerProfileHoliday> builder)
    {
        builder.ToTable("WorkerProfileHolidays");
        builder.HasKey(k => k.Id);
        builder.HasIndex(h => new { h.WorkerProfileId, h.HolidayId }).IsUnique();
    }
}
