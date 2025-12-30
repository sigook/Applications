using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class TimeSheetTotalPayrollConfiguration : IEntityTypeConfiguration<TimeSheetTotalPayroll>
    {
        public void Configure(EntityTypeBuilder<TimeSheetTotalPayroll> builder)
        {
            builder.ToTable("TimeSheetTotalPayroll");
            builder.HasKey(k => k.Id);
            builder.HasIndex(i => i.TimeSheetId).IsUnique();
        }
    }
}