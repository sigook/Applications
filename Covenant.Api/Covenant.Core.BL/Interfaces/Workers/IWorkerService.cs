using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Request.Worker;
using Covenant.Common.Models.Request;
using Covenant.Common.Models;
using Covenant.Common.Models.Notification;
using Covenant.Common.Models.Worker;
using System.Linq.Expressions;

namespace Covenant.Core.BL.Interfaces.Workers;

public interface IWorkerService
{
    Task<Result<Guid>> CreateWorker(int? orderId);
    Task<WorkerProfileDetailModel> GetProfileDetail(Expression<Func<WorkerProfile, bool>> condition);
    Task<Result> DeleteWorker(Guid workerProfileId);
    Task<Result<RequestApplicantDetailModel>> Apply(WorkerRequestApplyModel model, Guid? requestId = null);
    Task<WorkerRequestDetailModel> GetRequestDetail(Guid requestId);
    Task<Result> UpdateProfileImage(Guid profileId);
    Task<Result> UpdateDocumentSection(Guid profileId, WorkerDocumentType documentType);
    Task<PaginatedList<WorkerCommentModel>> GetAgencyComments(Guid workerProfileId, Pagination pagination);
    Task<PaginatedList<WorkerCommentModel>> GetMyComments(Pagination pagination);
    Task<Result> AddAgencyComment(Guid workerProfileId, string comment, decimal rate);
    Task<Result> AddCompanyComment(Guid workerProfileId, string comment, decimal rate);
    Task<Result> Unsubscribe(UnsubscribeModel model);
    Task<Result> AddNote(Guid workerProfileId, string note);
    Task<Result> UpdateApprovedToWork(Guid agencyId, Guid workerProfileId);
    Task<Result> UpdateExternalId(Guid workerProfileId, string externalId);
    Task<Result> UpdateWcCode(Guid workerProfileId, string wcCode);
    Task<Result> UpdateDnu(Guid agencyId, Guid workerProfileId);
    Task<Result> UpdateContractor(Guid agencyId, Guid workerProfileId);
    Task<Result> UpdateSubcontractor(Guid agencyId, Guid workerProfileId);
    Task<Result> UpdateEmail(Guid agencyId, Guid workerProfileId, string newEmail);
}
