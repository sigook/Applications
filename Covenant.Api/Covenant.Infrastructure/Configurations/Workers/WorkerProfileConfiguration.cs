using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

public class WorkerProfileConfiguration : IEntityTypeConfiguration<WorkerProfile>
{
    public void Configure(EntityTypeBuilder<WorkerProfile> builder)
    {
        builder.ToTable("WorkerProfiles");
        builder.Property(c => c.NumberId).ValueGeneratedOnAdd();
        builder.HasIndex(p => p.WorkerId).IsUnique();

        builder.HasOne(wp => wp.WorkerProfileTaxCategory)
            .WithOne(wp => wp.WorkerProfile)
            .HasForeignKey<WorkerProfileTaxCategory>(wp => wp.WorkerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
