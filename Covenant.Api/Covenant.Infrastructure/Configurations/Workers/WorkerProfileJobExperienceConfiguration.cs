using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

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
