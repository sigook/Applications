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
    Task<PaginatedList<RunnerListModel>> GetRunners(Guid agencyId, GetRunnersFilter filter);
    IQueryable<AgencyRunnerListModel> GetAllAgencyRunners(Guid agencyId, GetAgencyRunnersFilter filter);
    Task<AgencyRunnersPagedResponse> GetAgencyRunners(Guid agencyId, GetAgencyRunnersFilter filter);
    Task<RunnerDetailModel> GetRunnerDetail(Guid id);
}
