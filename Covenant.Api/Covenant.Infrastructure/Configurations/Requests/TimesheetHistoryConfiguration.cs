using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Requests;

public class TimesheetHistoryConfiguration : IEntityTypeConfiguration<TimesheetHistory>
{
    public void Configure(EntityTypeBuilder<TimesheetHistory> builder)
    {
        builder.HasNoKey();
        builder.ToView("TimesheetHistory");
    }
}
