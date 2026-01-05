using Covenant.Common.Entities.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Covenant.Infrastructure.Configurations.Notification
{
    public class NotificationTypeConfiguration : IEntityTypeConfiguration<NotificationType>
    {
        public void Configure(EntityTypeBuilder<NotificationType> builder)
        {
            builder.HasKey(t => t.Id);
        }
    }

    public class UserNotificationTypeConfiguration : IEntityTypeConfiguration<UserNotificationType>
    {
        public void Configure(EntityTypeBuilder<UserNotificationType> builder)
        {
            builder.HasIndex(t => new { t.UserId, t.NotificationTypeId }).IsUnique();
        }
    }
}