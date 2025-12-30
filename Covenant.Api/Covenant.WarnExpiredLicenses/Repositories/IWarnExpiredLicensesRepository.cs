using Covenant.Common.Entities;

namespace Covenant.WarnExpiredLicenses.Repositories
{
    public interface IWarnExpiredLicensesRepository
    {
        Task<List<WorkerLicenseExpiredModel>> Get(DateTime limit);
        Task CreateLastRun(TrackNotification entity);
        Task SaveChangesAsync();
        Task<TrackNotification> GetLastRun(string runner);
    }
}