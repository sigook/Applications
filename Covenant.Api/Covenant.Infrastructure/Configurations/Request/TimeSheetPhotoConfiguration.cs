using Covenant.Common.Entities.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Request
{
    public class TimeSheetPhotoConfiguration : IEntityTypeConfiguration<TimeSheetPhoto>
    {
        public void Configure(EntityTypeBuilder<TimeSheetPhoto> builder)
        {
            builder.ToTable("TimeSheetPhoto");
            builder.HasKey(k => new { k.TimeSheetId, k.PhotoId });
        }
    }
}