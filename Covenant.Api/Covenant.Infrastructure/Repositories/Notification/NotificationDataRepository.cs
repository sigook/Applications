using Covenant.Common.Models.Notification;
using Covenant.Common.Repositories.Notification;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories.Notification
{
    public class NotificationDataRepository : INotificationDataRepository
    {
        private readonly CovenantContext _context;
        public NotificationDataRepository(CovenantContext context) => _context = context;

        public Task<NotificationAgencyModel> GetAgencyData(Guid requestId, Guid workerProfileId, int notificationTypeId)
        {
            return (from wr in _context.WorkerRequests.Where(c => c.RequestId == requestId && c.WorkerProfileId == workerProfileId)
                    join unt in _context.UserNotificationTypes.Where(t => t.NotificationTypeId == notificationTypeId) on wr.Request.CompanyProfile.Agency.UserId equals unt.UserId
                        into tmp1
                    from unt in tmp1.DefaultIfEmpty()
                    select new NotificationAgencyModel
                    {
                        JobTitle = wr.Request.JobTitle,
                        AgencyEmail = wr.Request.CompanyProfile.Agency.User.Email,
                        CompanyFullName = wr.Request.CompanyProfile.FullName,
                        WorkerFullName = $"{wr.WorkerProfile.FirstName} {wr.WorkerProfile.MiddleName} {wr.WorkerProfile.LastName} {wr.WorkerProfile.SecondLastName}",
                        EmailNotification = unt != null && unt.EmailNotification
                    }).SingleOrDefaultAsync();
        }

        public Task<NotificationAgencyModel> GetAgencyData(Guid requestId, int notificationTypeId)
        {
            return (from r in _context.Requests.Where(c => c.Id == requestId)
                    join unt in _context.UserNotificationTypes.Where(t => t.NotificationTypeId == notificationTypeId) on r.CompanyProfile.Agency.UserId equals unt.UserId
                        into tmp1
                    from unt in tmp1.DefaultIfEmpty()
                    select new NotificationAgencyModel
                    {
                        JobTitle = r.JobTitle,
                        AgencyEmail = r.CompanyProfile.Agency.User.Email,
                        CompanyFullName = r.CompanyProfile.FullName,
                        EmailNotification = unt != null && unt.EmailNotification
                    }).SingleOrDefaultAsync();
        }

        public Task<NotificationCompanyModel> GetCompanyData(Guid companyId, int notificationTypeId)
        {
            return (from cu in _context.Users.Where(u => u.Id == companyId)
                    join unt in _context.UserNotificationTypes.Where(t => t.NotificationTypeId == notificationTypeId) on cu.Id equals unt.UserId
                        into tmp1
                    from unt in tmp1.DefaultIfEmpty()
                    select new NotificationCompanyModel
                    {
                        CompanyEmail = cu.Email,
                        EmailNotification = unt != null && unt.EmailNotification
                    }).SingleOrDefaultAsync();
        }

        public Task<NotificationAgencyModel> GetAgencyData(Guid agencyId)
        {
            return (from a in _context.Agencies.Where(a => a.Id == agencyId)
                    select new NotificationAgencyModel
                    {
                        AgencyEmail = a.User.Email,
                    }).SingleOrDefaultAsync();
        }

        public Task<NotificationCompanyModel> GetCompanyData(Guid requestId, Guid workerProfileId, int notificationTypeId)
        {
            return (from wr in _context.WorkerRequests.Where(c => c.RequestId == requestId && c.WorkerProfileId == workerProfileId)
                    join unt in _context.UserNotificationTypes.Where(t => t.NotificationTypeId == notificationTypeId) on wr.Request.CompanyProfile.CompanyId equals unt.UserId
                        into tmp1
                    from unt in tmp1.DefaultIfEmpty()
                    select new NotificationCompanyModel
                    {
                        JobTitle = wr.Request.JobTitle,
                        CompanyEmail = wr.Request.CompanyProfile.Company.Email,
                        AgencyFullName = wr.Request.CompanyProfile.Agency.FullName,
                        WorkerFullName = $"{wr.WorkerProfile.FirstName} {wr.WorkerProfile.MiddleName} {wr.WorkerProfile.LastName} {wr.WorkerProfile.SecondLastName}",
                        EmailNotification = unt != null && unt.EmailNotification
                    }).SingleOrDefaultAsync();
        }

        public Task<NotificationWorkerModel> GetWorkerData(Guid requestId, Guid workerProfileId, int notificationTypeId)
        {
            return (from wr in _context.WorkerRequests.Where(c => c.RequestId == requestId && c.WorkerProfileId == workerProfileId)
                    join unt in _context.UserNotificationTypes.Where(t => t.NotificationTypeId == notificationTypeId) on wr.WorkerProfile.WorkerId equals unt.UserId
                        into tmp1
                    from unt in tmp1.DefaultIfEmpty()
                    select new NotificationWorkerModel
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
            return (from wr in _context.WorkerRequests.Where(c => c.Id == workerRequestId)
                    join unt in _context.UserNotificationTypes.Where(t => t.NotificationTypeId == notificationTypeId) on wr.WorkerProfile.WorkerId equals unt.UserId
                        into tmp1
                    from unt in tmp1.DefaultIfEmpty()
                    select new NotificationWorkerModel
                    {
                        JobTitle = wr.Request.JobTitle,
                        AgencyFullName = wr.Request.CompanyProfile.Agency.FullName,
                        CompanyFullName = wr.Request.CompanyProfile.FullName,
                        WorkerEmail = wr.WorkerProfile.Worker.Email,
                        EmailNotification = unt != null && unt.EmailNotification
                    }).SingleOrDefaultAsync();
        }
    }
}