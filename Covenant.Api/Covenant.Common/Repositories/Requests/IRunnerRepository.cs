using Covenant.Common.Entities.Requests.Runners;
using Covenant.Common.Models.Requests.Runners;
using Covenant.Common.Models;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Requests;

public interface IRunnerRepository : IBaseRepository
{
    Task<Runner> GetRunner(Expression<Func<Runner, bool>> expression);
    Task<List<Runner>> GetRunners(Expression<Func<Runner, bool>> expression);
    Task<bool> RunnerExists(Guid requestId, Guid workerProfileId);
    Task<PaginatedList<RunnerListModel>> GetRunners(IEnumerable<Guid> agencyIds, GetRunnersFilter filter);
    IQueryable<AgencyRunnerListModel> GetAllAgencyRunners(IEnumerable<Guid> agencyIds, GetAgencyRunnersFilter filter);
    Task<AgencyRunnersPagedResponse> GetAgencyRunners(IEnumerable<Guid> agencyIds, GetAgencyRunnersFilter filter);
    Task<RunnerDetailModel> GetRunnerDetail(Guid id);
}
