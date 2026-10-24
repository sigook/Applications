using Covenant.Common.Models.Notifications;

namespace Covenant.Common.Repositories.Notifications;

public interface INotificationDataRepository : IBaseRepository
{
    Task<NotificationAgencyModel> GetAgencyData(Guid requestId, Guid workerId, int notificationTypeId);
    Task<NotificationCompanyModel> GetCompanyData(Guid requestId, Guid workerId, int notificationTypeId);
    Task<NotificationWorkerModel> GetWorkerData(Guid requestId, Guid workerId, int notificationTypeId);
    Task<NotificationWorkerModel> GetWorkerData(Guid workerRequestId, int notificationTypeId);
    Task<NotificationAgencyModel> GetAgencyData(Guid requestId, int notificationTypeId);
    Task<NotificationCompanyModel> GetCompanyData(Guid companyId, int notificationTypeId);
    Task<NotificationAgencyModel> GetAgencyData(Guid agencyId);
}
