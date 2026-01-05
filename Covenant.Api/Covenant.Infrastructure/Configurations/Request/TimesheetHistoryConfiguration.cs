using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request;

public class TimesheetHistoryConfiguration : IEntityTypeConfiguration<TimesheetHistory>
{
    public void Configure(EntityTypeBuilder<TimesheetHistory> builder)
    {
        builder.HasNoKey();
        builder.ToView("TimesheetHistory");
    }
}
