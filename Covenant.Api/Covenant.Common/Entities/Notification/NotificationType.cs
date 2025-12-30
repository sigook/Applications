using Covenant.Common.Enums;

namespace Covenant.Common.Entities.Notification
{
    public class NotificationType
    {
        private NotificationType() 
        { 
        }

        private NotificationType(string title, NotificationTarget target, int id)
        {
            Id = id;
            Title = title;
            Target = target;

        }

        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public NotificationTarget Target { get; private set; }

        public static NotificationType WorkerHasBeenBooked => new NotificationType("Worker Booked", NotificationTarget.Worker, 61) { Description = "When you have been booked in a request" };
        public static NotificationType WorkerHasBeenRejected => new NotificationType("Worker Rejected", NotificationTarget.Worker, 67) { Description = "When you have been rejected in a request" };
        public static NotificationType WorkShiftHasExceeded => new NotificationType("Work Shift Has Exceeded", NotificationTarget.Worker, 79) { Description = "When work shift has exceeded the maximum allow" };
        public static NotificationType NewRequestNotifyWorker => new NotificationType("New Job", NotificationTarget.Worker, 10) { Description = "When a new job is available" };
        public static NotificationType WorkerApplyNotifyAgency => new NotificationType("Worker Apply", NotificationTarget.Agency, 41) { Description = "When a worker apply to requests" };
        public static NotificationType WorkerDeclineNotifyAgency => new NotificationType("Worker Decline", NotificationTarget.Agency, 47) { Description = "When a worker isn't going to work" };
        public static NotificationType RequestHasBeenCanceledNotifyAgency => new NotificationType("Request has been canceled", NotificationTarget.Agency, 71) { Description = "When request has been canceled" };
        public static NotificationType NewRequest => new NotificationType("New Request", NotificationTarget.Agency, 9) { Description = "When new request has been created" };
        public static NotificationType NewWorkerRequested => new NotificationType("New Worker Requested", NotificationTarget.Agency, 73) { Description = "When a new worker is requested" };
        public static NotificationType WorkerApplyNotifyCompany => new NotificationType("Worker Apply", NotificationTarget.Company, 43) { Description = "When a worker apply to any of your requests" };
        public static NotificationType WorkerDeclineNotifyCompany => new NotificationType("Worker Decline", NotificationTarget.Company, 53) { Description = "When a worker isn't going to work" };
        public static NotificationType InvoiceChargeSucceeded => new NotificationType("Invoice Charge Succeeded", NotificationTarget.Company, 83) { Description = "When Invoice Charge Succeeded" };
        public static NotificationType InvoiceChargeFailed => new NotificationType("Invoice Charge Failed", NotificationTarget.Company, 89) { Description = "When Invoice Charge Failed" };

        public static IEnumerable<NotificationType> NotificationTypeWorkers => new[]
        {
            WorkerHasBeenBooked, WorkerHasBeenRejected, WorkShiftHasExceeded, NewRequestNotifyWorker
        };

        public static IEnumerable<NotificationType> GetAll => new[]
        {
            WorkerApplyNotifyAgency,
            WorkerApplyNotifyCompany,
            WorkerDeclineNotifyAgency,
            WorkerDeclineNotifyCompany,
            WorkerHasBeenBooked,
            WorkerHasBeenRejected,
            RequestHasBeenCanceledNotifyAgency,
            NewRequest,
            NewRequestNotifyWorker,
            NewWorkerRequested,
            WorkShiftHasExceeded,
            InvoiceChargeSucceeded,
            InvoiceChargeFailed
        };
        
    }
}