namespace Covenant.Common.Models.Notification
{
	public class UserNotificationListModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public bool EmailNotification { get; set; }
		public bool PushNotification { get; set; }
		public bool SMSNotification { get; set; }
	}
}