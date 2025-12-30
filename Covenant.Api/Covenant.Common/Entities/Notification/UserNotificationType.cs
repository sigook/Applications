namespace Covenant.Common.Entities.Notification
{
    public class UserNotificationType
    {
        private UserNotificationType() 
        { 
        }

        public UserNotificationType(Guid userId, int notificationTypeId, Guid id = default)
        {
            Id = id == default ? Guid.NewGuid() : id;
            UserId = userId;
            NotificationTypeId = notificationTypeId;
        }

        public Guid Id { get; private set; }
        public User User { get; private set; }
        public Guid UserId { get; private set; }
        public NotificationType NotificationType { get; private set; }
        public int NotificationTypeId { get; private set; }
        public bool EmailNotification { get; set; }
        public bool PushNotification { get; set; }
        public bool SMSNotification { get; set; }
    }
}