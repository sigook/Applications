using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Requests;

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
