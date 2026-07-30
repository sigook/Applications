using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker
{
    public class WorkerProfileConfiguration : IEntityTypeConfiguration<WorkerProfile>
    {
        public void Configure(EntityTypeBuilder<WorkerProfile> builder)
        {
            builder.Property(c => c.NumberId).ValueGeneratedOnAdd();
            builder.HasIndex(p => p.WorkerId).IsUnique();

            builder.HasOne(wp => wp.WorkerProfileTaxCategory)
                .WithOne(wp => wp.WorkerProfile)
                .HasForeignKey<WorkerProfileTaxCategory>(wp => wp.WorkerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}