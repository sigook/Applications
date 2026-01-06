using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Request
{
    public interface IWorkerRequestRepository
    {
        Task<Common.Entities.Request.WorkerRequest> GetWorkerRequest(Guid id);
        Task<Common.Entities.Request.WorkerRequest> GetWorkerRequest(Guid workerId, Guid requestId);
        Task<Common.Entities.Request.WorkerRequest> GetWorkerRequestByWorkerProfileId(Guid workerProfileId, Guid requestId);
        Task<IEnumerable<Common.Entities.Request.WorkerRequest>> GetWorkerRequestsByWorkerId(Guid workerId);
        Task<bool> WorkerRequestExists(Guid workerId, Guid requestId);
        Task<bool> CanClockIn(Expression<Func<WorkerProfile, bool>> condition, DateTime now);
        Task<WorkerRequestInfoModel> GetWorkerRequestInfo(Guid workerId, Guid requestId, DateTime now);
        Task CreateNote(WorkerRequestNote entity);
        Task<PaginatedList<NoteModel>> GetNotes(Guid workerRequestId, Pagination pagination);
        Task<NoteModel> GetNoteDetail(Guid id);
        Task<WorkerRequestNote> GetNote(Guid id);
        Task Update(WorkerRequestNote entity);
        Task UpdateWorkerRequest(Common.Entities.Request.WorkerRequest entity);
        Task SaveChangesAsync();
    }
}