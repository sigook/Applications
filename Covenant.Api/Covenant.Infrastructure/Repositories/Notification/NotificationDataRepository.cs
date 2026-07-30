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
            return (from wr in _context.WorkerRequest.Where(c => c.RequestId == requestId && c.WorkerProfileId == workerProfileId)
                    join r in _context.Request.Where(c => c.Id == requestId) on wr.RequestId equals r.Id
                    join a in _context.Agencies on r.CompanyProfile.AgencyId equals a.Id
                    join au in _context.User on a.UserId equals au.Id
                    join unt in _context.UserNotificationType.Where(t => t.NotificationTypeId == notificationTypeId) on au.Id equals unt.UserId
                        into tmp1
                    from unt in tmp1.DefaultIfEmpty()
                    select new NotificationAgencyModel
                    {
                        JobTitle = r.JobTitle,
                        AgencyEmail = au.Email,
                        CompanyFullName = r.CompanyProfile.FullName,
                        WorkerFullName = $"{wr.WorkerProfile.FirstName} {wr.WorkerProfile.MiddleName} {wr.WorkerProfile.LastName} {wr.WorkerProfile.SecondLastName}",
                        EmailNotification = unt != null && unt.EmailNotification
                    }).SingleOrDefaultAsync();
        }

        public Task<NotificationAgencyModel> GetAgencyData(Guid requestId, int notificationTypeId)
        {
            return (from r in _context.Request.Where(c => c.Id == requestId)
                    join a in _context.Agencies on r.CompanyProfile.AgencyId equals a.Id
                    join au in _context.User on a.UserId equals au.Id
                    join unt in _context.UserNotificationType.Where(t => t.NotificationTypeId == notificationTypeId) on au.Id equals unt.UserId
                        into tmp1
                    from unt in tmp1.DefaultIfEmpty()
                    select new NotificationAgencyModel
                    {
                        JobTitle = r.JobTitle,
                        AgencyEmail = au.Email,
                        CompanyFullName = r.CompanyProfile.FullName,
                        EmailNotification = unt != null && unt.EmailNotification
                    }).SingleOrDefaultAsync();
        }

        public Task<NotificationCompanyModel> GetCompanyData(Guid companyId, int notificationTypeId)
        {
            return (from cu in _context.User.Where(u => u.Id == companyId)
                    join unt in _context.UserNotificationType.Where(t => t.NotificationTypeId == notificationTypeId) on cu.Id equals unt.UserId
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
                    join au in _context.User on a.UserId equals au.Id
                    select new NotificationAgencyModel
                    {
                        AgencyEmail = au.Email,
                    }).SingleOrDefaultAsync();
        }

        public Task<NotificationCompanyModel> GetCompanyData(Guid requestId, Guid workerProfileId, int notificationTypeId)
        {
            return (from wr in _context.WorkerRequest.Where(c => c.RequestId == requestId && c.WorkerProfileId == workerProfileId)
                    join r in _context.Request.Where(c => c.Id == requestId) on wr.RequestId equals r.Id
                    join cu in _context.User on r.CompanyProfile.CompanyId equals cu.Id
                    join unt in _context.UserNotificationType.Where(t => t.NotificationTypeId == notificationTypeId) on cu.Id equals unt.UserId
                        into tmp1
                    from unt in tmp1.DefaultIfEmpty()
                    join a in _context.Agencies on r.CompanyProfile.AgencyId equals a.Id
                    select new NotificationCompanyModel
                    {
                        JobTitle = r.JobTitle,
                        CompanyEmail = cu.Email,
                        AgencyFullName = a.FullName,
                        WorkerFullName = $"{wr.WorkerProfile.FirstName} {wr.WorkerProfile.MiddleName} {wr.WorkerProfile.LastName} {wr.WorkerProfile.SecondLastName}",
                        EmailNotification = unt != null && unt.EmailNotification
                    }).SingleOrDefaultAsync();
        }

        public Task<NotificationWorkerModel> GetWorkerData(Guid requestId, Guid workerProfileId, int notificationTypeId)
        {
            return (from wr in _context.WorkerRequest.Where(c => c.RequestId == requestId && c.WorkerProfileId == workerProfileId)
                    join r in _context.Request on wr.RequestId equals r.Id
                    join a in _context.Agencies on r.CompanyProfile.AgencyId equals a.Id
                    join wu in _context.User on wr.WorkerProfile.WorkerId equals wu.Id
                    join unt in _context.UserNotificationType.Where(t => t.NotificationTypeId == notificationTypeId) on wu.Id equals unt.UserId
                        into tmp1
                    from unt in tmp1.DefaultIfEmpty()
                    select new NotificationWorkerModel
                    {
                        JobTitle = r.JobTitle,
                        AgencyFullName = a.FullName,
                        CompanyFullName = r.CompanyProfile.FullName,
                        WorkerEmail = wu.Email,
                        EmailNotification = unt != null && unt.EmailNotification
                    }).SingleOrDefaultAsync();
        }

        public Task<NotificationWorkerModel> GetWorkerData(Guid workerRequestId, int notificationTypeId)
        {
            return (from wr in _context.WorkerRequest.Where(c => c.Id == workerRequestId)
                    join r in _context.Request on wr.RequestId equals r.Id
                    join a in _context.Agencies on r.CompanyProfile.AgencyId equals a.Id
                    join wu in _context.User on wr.WorkerProfile.WorkerId equals wu.Id
                    join unt in _context.UserNotificationType.Where(t => t.NotificationTypeId == notificationTypeId) on wu.Id equals unt.UserId
                        into tmp1
                    from unt in tmp1.DefaultIfEmpty()
                    select new NotificationWorkerModel
                    {
                        JobTitle = r.JobTitle,
                        AgencyFullName = a.FullName,
                        CompanyFullName = r.CompanyProfile.FullName,
                        WorkerEmail = wu.Email,
                        EmailNotification = unt != null && unt.EmailNotification
                    }).SingleOrDefaultAsync();
        }
    }
}