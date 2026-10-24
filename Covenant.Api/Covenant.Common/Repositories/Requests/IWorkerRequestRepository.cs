using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Models.Requests;
using Covenant.Common.Models;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Requests;

public interface IWorkerRequestRepository : IBaseRepository
{
    Task<WorkerRequest> GetWorkerRequest(Guid id);
    Task<WorkerRequest> GetWorkerRequest(Guid workerId, Guid requestId);
    Task<WorkerRequest> GetWorkerRequestByWorkerProfileId(Guid workerProfileId, Guid requestId);
    Task<IEnumerable<WorkerRequest>> GetWorkerRequestsByWorkerProfileId(Guid workerProfileId);
    Task<bool> WorkerRequestExists(Guid workerProfileId, Guid requestId);
    Task<bool> CanClockIn(Expression<Func<WorkerProfile, bool>> condition, DateTime now);
    Task<WorkerRequestInfoModel> GetWorkerRequestInfo(Guid workerId, Guid requestId, DateTime now);
    Task<PaginatedList<NoteModel>> GetNotes(Guid workerRequestId, Pagination pagination);
    Task<NoteModel> GetNoteDetail(Guid id);
    Task<WorkerRequestNote> GetNote(Guid id);
}
