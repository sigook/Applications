namespace Covenant.Common.Models.Notification
{
	public class UserNotificationUpdateModel
	{
		public int Id { get; set; }
		public bool EmailNotification { get; set; }
		public bool PushNotification { get; set; }
		public bool SMSNotification { get; set; }
	}
}