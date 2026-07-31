using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Covenant.Infrastructure.Configurations.Request.Runners;

public class RunnerInterviewConfiguration : IEntityTypeConfiguration<RunnerInterview>
{
    public void Configure(EntityTypeBuilder<RunnerInterview> builder)
    {
        builder.ToTable("RunnerInterviews");
        builder.Property(e => e.Type).HasConversion(new EnumToStringConverter<InterviewType>());
        builder.Property(e => e.Status).HasConversion(new EnumToStringConverter<InterviewStatus>());

        builder.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.RescheduledByUser)
            .WithMany()
            .HasForeignKey(e => e.RescheduledBy)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
