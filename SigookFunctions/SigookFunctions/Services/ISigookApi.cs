using SigookFunctions.Models;

namespace SigookFunctions.Services
{
    public interface ISigookApi
    {
        Task<PaginatedList<WorkerContactInfoModel>> GetWorkers(int pageIndex, Guid agencyId);
    }
}