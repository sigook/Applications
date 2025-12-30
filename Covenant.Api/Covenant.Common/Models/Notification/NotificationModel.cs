namespace Covenant.Common.Models.Notification
{
	public class NotificationModel
	{
		public string Title { get; set; }
		public string Body { get; set; }
		public string Topic { get; set; }
		public IReadOnlyDictionary<string, string> Data { get; set; } = new Dictionary<string, string>(0);

		public static NotificationModel NewRequestNotification(string title, string body, Guid requestId)
		{
			return new NotificationModel
			{
				Title = title,
				Body = body,
				Topic = "request",
				Data = new Dictionary<string, string> { { "requestId", requestId.ToString() } }
			};
		}
	}
}