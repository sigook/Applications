using Covenant.Common.Enums;
using Covenant.Common.Models.Notifications;

namespace Covenant.Common.Entities.Notifications;

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
    public static NotificationType WorkerApplyNotifyCompany => new NotificationType("Worker Apply", NotificationTarget.Company, 43) { Description = "When a worker apply to any of your requests" };
    public static NotificationType WorkerDeclineNotifyCompany => new NotificationType("Worker Decline", NotificationTarget.Company, 53) { Description = "When a worker isn't going to work" };
    public static NotificationType InvoiceChargeSucceeded => new NotificationType("Invoice Charge Succeeded", NotificationTarget.Company, 83) { Description = "When Invoice Charge Succeeded" };
    public static NotificationType InvoiceChargeFailed => new NotificationType("Invoice Charge Failed", NotificationTarget.Company, 89) { Description = "When Invoice Charge Failed" };

    public static IEnumerable<NotificationType> NotificationTypeWorkers =>
    [
        WorkerHasBeenBooked, WorkerHasBeenRejected, WorkShiftHasExceeded, NewRequestNotifyWorker
    ];

    public static IEnumerable<NotificationType> GetAll =>
    [
        WorkerApplyNotifyCompany,
        WorkerDeclineNotifyCompany,
        WorkerHasBeenBooked,
        WorkerHasBeenRejected,
        NewRequestNotifyWorker,
        WorkShiftHasExceeded,
        InvoiceChargeSucceeded,
        InvoiceChargeFailed
    ];
    
}