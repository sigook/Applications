using Covenant.Common.Entities.Agency;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Configurations.Agencies;

public class UserAttendanceConfiguration : IEntityTypeConfiguration<UserAttendance>
{
    public void Configure(EntityTypeBuilder<UserAttendance> builder)
    {
        builder.ToTable("UserAttendances");
        builder.HasKey(k => k.Id);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.UserId, x.Date }).IsUnique();
        builder.Property(x => x.Date).HasColumnType("timestamp without time zone");
        builder.Property(x => x.ClockIn).HasColumnType("timestamp without time zone");
        builder.Property(x => x.ClockOut).HasColumnType("timestamp without time zone");
        builder.Property(x => x.EditedAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.EditedBy).HasMaxLength(256);
        builder.Property(x => x.EditReason).HasMaxLength(500);
        builder.Ignore(x => x.IsOpen);
    }
}
