using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Covenant.Infrastructure.Configurations.Request.Runners;

public class RunnerStatusHistoryConfiguration : IEntityTypeConfiguration<RunnerStatusHistory>
{
    public void Configure(EntityTypeBuilder<RunnerStatusHistory> builder)
    {
        builder.Property(e => e.PreviousStatus).HasConversion(new EnumToStringConverter<RunnerStatus>());
        builder.Property(e => e.NewStatus).HasConversion(new EnumToStringConverter<RunnerStatus>());
    }
}
