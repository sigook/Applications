using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Models;
using Covenant.Common.Models.Request.Runners;
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
