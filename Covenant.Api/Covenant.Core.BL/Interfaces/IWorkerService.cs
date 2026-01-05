using Covenant.Common.Functionals;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Worker;

namespace Covenant.Core.BL.Interfaces;

public interface IWorkerService
{
    Task<Result<Guid>> CreateWorker(int? orderId);
    Task<Result> DeleteWorker(Guid workerProfileId);
    Task<Result> UpdateWorkerPunchCardId(Guid profileId, Guid agencyId, string punchCardId);
    Task<Result<RequestApplicantDetailModel>> Apply(Guid requestId, WorkerRequestApplyModel model, Guid? workerId = null);
}
