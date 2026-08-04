using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request;

public class TimeSheetTotalConfiguration : IEntityTypeConfiguration<TimeSheetTotal>
{
    public void Configure(EntityTypeBuilder<TimeSheetTotal> builder)
    {
        builder.ToTable("TimeSheetTotals");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.TimeSheet)
            .WithOne(x => x.TimeSheetTotal)
            .HasForeignKey<TimeSheetTotal>(x => x.TimeSheetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
