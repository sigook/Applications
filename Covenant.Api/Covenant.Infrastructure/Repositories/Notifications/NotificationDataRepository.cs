using Covenant.Common.Models.Notifications;
using Covenant.Common.Repositories.Notifications;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories.Notifications;

public class NotificationDataRepository(CovenantContext context) : BaseRepository<CovenantContext>(context), INotificationDataRepository
{
    public Task<NotificationAgencyModel> GetRequestAgencyData(Guid requestId)
    {
        return Context.Requests.Where(r => r.Id == requestId)
            .Select(r => new NotificationAgencyModel
            {
                JobTitle = r.JobTitle,
                AgencyEmail = r.CompanyProfile.Agency.User.Email,
                CompanyFullName = r.CompanyProfile.FullName
            }).SingleOrDefaultAsync();
    }

    public Task<NotificationCompanyModel> GetCompanyData(Guid companyId, int notificationTypeId)
    {
        return Context.Users.Where(u => u.Id == companyId)
            .LeftJoin(
                Context.UserNotificationTypes.Where(t => t.NotificationTypeId == notificationTypeId),
                cu => cu.Id,
                unt => unt.UserId,
                (cu, unt) => new NotificationCompanyModel
                {
                    CompanyEmail = cu.Email,
                    EmailNotification = unt != null && unt.EmailNotification
                }).SingleOrDefaultAsync();
    }

    public Task<NotificationAgencyModel> GetAgencyData(Guid agencyId)
    {
        return (from a in Context.Agencies.Where(a => a.Id == agencyId)
                select new NotificationAgencyModel
                {
                    AgencyEmail = a.User.Email,
                }).SingleOrDefaultAsync();
    }

    public Task<NotificationCompanyModel> GetCompanyData(Guid requestId, Guid workerProfileId, int notificationTypeId)
    {
        return Context.WorkerRequests.Where(c => c.RequestId == requestId && c.WorkerProfileId == workerProfileId)
            .LeftJoin(
                Context.UserNotificationTypes.Where(t => t.NotificationTypeId == notificationTypeId),
                wr => wr.Request.CompanyProfile.CompanyId,
                unt => unt.UserId,
                (wr, unt) => new NotificationCompanyModel
                {
                    JobTitle = wr.Request.JobTitle,
                    CompanyEmail = wr.Request.CompanyProfile.Company.Email,
                    AgencyFullName = wr.Request.CompanyProfile.Agency.FullName,
                    WorkerFullName = wr.WorkerProfile.FirstName + (string.IsNullOrWhiteSpace(wr.WorkerProfile.MiddleName) ? string.Empty : " " + wr.WorkerProfile.MiddleName) + " " + wr.WorkerProfile.LastName + (string.IsNullOrWhiteSpace(wr.WorkerProfile.SecondLastName) ? string.Empty : " " + wr.WorkerProfile.SecondLastName),
                    EmailNotification = unt != null && unt.EmailNotification
                }).SingleOrDefaultAsync();
    }

    public Task<NotificationWorkerModel> GetWorkerData(Guid requestId, Guid workerProfileId, int notificationTypeId)
    {
        return Context.WorkerRequests.Where(c => c.RequestId == requestId && c.WorkerProfileId == workerProfileId)
            .LeftJoin(
                Context.UserNotificationTypes.Where(t => t.NotificationTypeId == notificationTypeId),
                wr => wr.WorkerProfile.WorkerId,
                unt => unt.UserId,
                (wr, unt) => new NotificationWorkerModel
                {
                    JobTitle = wr.Request.JobTitle,
                    AgencyFullName = wr.Request.CompanyProfile.Agency.FullName,
                    CompanyFullName = wr.Request.CompanyProfile.FullName,
                    WorkerEmail = wr.WorkerProfile.Worker.Email,
                    EmailNotification = unt != null && unt.EmailNotification
                }).SingleOrDefaultAsync();
    }

    public Task<NotificationWorkerModel> GetWorkerData(Guid workerRequestId, int notificationTypeId)
    {
        return Context.WorkerRequests.Where(c => c.Id == workerRequestId)
            .LeftJoin(
                Context.UserNotificationTypes.Where(t => t.NotificationTypeId == notificationTypeId),
                wr => wr.WorkerProfile.WorkerId,
                unt => unt.UserId,
                (wr, unt) => new NotificationWorkerModel
                {
                    JobTitle = wr.Request.JobTitle,
                    AgencyFullName = wr.Request.CompanyProfile.Agency.FullName,
                    CompanyFullName = wr.Request.CompanyProfile.FullName,
                    WorkerEmail = wr.WorkerProfile.Worker.Email,
                    EmailNotification = unt != null && unt.EmailNotification
                }).SingleOrDefaultAsync();
    }
}
