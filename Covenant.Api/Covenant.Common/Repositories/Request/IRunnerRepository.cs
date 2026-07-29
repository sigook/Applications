using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Models;
using Covenant.Common.Models.Request.Runners;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Request;

public interface IRunnerRepository
{
    Task<Runner> GetRunner(Expression<Func<Runner, bool>> expression);
    Task<List<Runner>> GetRunners(Expression<Func<Runner, bool>> expression);
    Task<bool> RunnerExists(Guid requestId, Guid workerProfileId);
    Task<PaginatedList<RunnerListModel>> GetRunners(Guid agencyId, GetRunnersFilter filter);
    Task<RunnerDetailModel> GetRunnerDetail(Guid id);
    Task<List<RunnerStartingTodayModel>> GetRunnersStartingToday(Guid agencyId, Guid updatedBy, DateTime windowStart, DateTime windowEnd);
    Task SaveChangesAsync();
    Task Update<T>(T entity) where T : class;
    Task Create<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
}
