namespace Covenant.Common.Models.Notification
{
	public class NotificationCompanyModel
	{
		public string JobTitle { get; set; }
		public string WorkerFullName { get; set; }
		public string CompanyEmail { get; set; }
		public string AgencyFullName { get; set; }
		public bool EmailNotification { get; set; }
	}
}