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
        builder.Property(e => e.Type).HasConversion(new EnumToStringConverter<InterviewType>());
        builder.Property(e => e.Status).HasConversion(new EnumToStringConverter<InterviewStatus>());
    }
}
