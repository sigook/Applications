using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Requests;

public class TimeSheetTotalPayrollConfiguration : IEntityTypeConfiguration<TimeSheetTotalPayroll>
{
    public void Configure(EntityTypeBuilder<TimeSheetTotalPayroll> builder)
    {
        builder.ToTable("TimeSheetTotalPayrolls");
        builder.HasKey(k => k.Id);
        builder.HasIndex(i => i.TimeSheetId).IsUnique();
    }
}
