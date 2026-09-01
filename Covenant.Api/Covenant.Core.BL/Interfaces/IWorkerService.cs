using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Request;
using Covenant.Common.Models;
using Covenant.Common.Models.WebSite;
using Covenant.Common.Models.Worker;

namespace Covenant.Core.BL.Interfaces;

public interface IWorkerService
{
    Task<Result<Guid>> CreateWorker(int? orderId);
    Task<Result> DeleteWorker(Guid workerProfileId);
    Task<Result<RequestApplicantDetailModel>> Apply(Guid requestId, WorkerRequestApplyModel model, Guid? workerId = null);
    Task<Result> ApplyByEmail(ApplyByEmailModel model);
    Task<Result> UpdateProfileImage(Guid profileId);
    Task<Result> UpdateDocumentSection(Guid profileId, WorkerDocumentType documentType);
    Task<Result<PaginatedList<WorkerCommentModel>>> GetComments(Guid workerId, Pagination pagination);
    Task<Result> AddAgencyComment(Guid workerProfileId, string comment, decimal rate);
    Task<Result> AddCompanyComment(Guid workerProfileId, string comment, decimal rate);
}
