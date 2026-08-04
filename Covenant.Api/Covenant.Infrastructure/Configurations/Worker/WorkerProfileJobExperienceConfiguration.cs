using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Worker;

public class WorkerProfileJobExperienceConfiguration : IEntityTypeConfiguration<WorkerProfileJobExperience>
{
    public void Configure(EntityTypeBuilder<WorkerProfileJobExperience> builder)
    {
        builder.ToTable("WorkerProfileJobExperiences");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.WorkerProfile)
            .WithMany(x => x.JobExperiences)
            .HasForeignKey(x => x.WorkerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
