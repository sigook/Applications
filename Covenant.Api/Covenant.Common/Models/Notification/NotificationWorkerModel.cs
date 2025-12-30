namespace Covenant.Common.Models.Notification
{
    public class NotificationWorkerModel
    {
        public string JobTitle { get; set; }
        public string WorkerEmail { get; set; }
        public bool EmailNotification { get; set; }
        public string AgencyFullName { get; set; }
        public string CompanyFullName { get; set; }
    }
}