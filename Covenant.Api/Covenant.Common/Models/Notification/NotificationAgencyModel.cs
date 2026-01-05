namespace Covenant.Common.Models.Notification
{
	public class NotificationAgencyModel
	{
		public string JobTitle { get; set; }
		public string WorkerFullName { get; set; }
		public string CompanyFullName { get; set; }
		public string AgencyEmail { get; set; }
		public bool EmailNotification { get; set; }
	}
}