using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request;

public class RequestSkillConfiguration : IEntityTypeConfiguration<RequestSkill>
{
    public void Configure(EntityTypeBuilder<RequestSkill> builder)
    {
        builder.ToTable("RequestSkills");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Request)
            .WithMany()
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
