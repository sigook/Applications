using Covenant.Common.Entities.Worker;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;

namespace Covenant.Common.Repositories.Worker
{
    public interface IWorkerCommentsRepository
    {
        Task CreateComment(WorkerComment comment);
        Task<PaginatedList<WorkerCommentModel>> GetComments(Guid workerId, Pagination pagination);
        Task<WorkerCommentModel> GetCommentModel(Guid workerId, Guid id);
        Task<WorkerComment> GetComment(Guid workerId, Guid id);
        Task UpdateComment(WorkerComment comment);
    }
}