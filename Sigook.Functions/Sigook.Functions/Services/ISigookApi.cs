using Sigook.Functions.Models;

namespace Sigook.Functions.Services
{
    public interface ISigookApi
    {
        Task<PaginatedList<WorkerContactInfoModel>> GetWorkers(int pageIndex, Guid agencyId);
    }
}