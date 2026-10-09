using Covenant.Common.Entities.Worker;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Workers;

public class WorkerProfileSkillConfiguration : IEntityTypeConfiguration<WorkerProfileSkill>
{
    public void Configure(EntityTypeBuilder<WorkerProfileSkill> builder)
    {
        builder.ToTable("WorkerProfileSkills");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.WorkerProfile)
            .WithMany(x => x.Skills)
            .HasForeignKey(x => x.WorkerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
