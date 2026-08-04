using Covenant.Common.Entities.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Notification;

public class UserNotificationTypeConfiguration : IEntityTypeConfiguration<UserNotificationType>
{
    public void Configure(EntityTypeBuilder<UserNotificationType> builder)
    {
        builder.ToTable("UserNotificationTypes");
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => new { t.UserId, t.NotificationTypeId }).IsUnique();
    }
}
