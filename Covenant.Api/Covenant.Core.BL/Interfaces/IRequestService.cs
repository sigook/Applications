using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Request;

namespace Covenant.Core.BL.Interfaces
{
    public interface IRequestService
    {
        Task SendInvitationToApply(InvitationToApplyModel model);
        Task<Result> OpenRequest(Guid requestId, string finalizedBy);
        Task<Result<Guid>> CreateRequest(RequestCreateModel model, Guid? companyId = null);
        Task<Result<Guid>> CompanyCreateRequest(RequestCreateModel model);
        Task<Result> UpdateRequest(Guid requestId, RequestCreateModel model);
        Task<Result> UpdateRequirements(Guid id, RequestUpdateRequirementsModel model);
        Task<Result> UpdateIsAsap(Guid id);
        Task<Result> UpdateIsAsapRequests(RequestsQuickUpdate requestsQuickUpdate);
        Task<Result> PunchCardUpdateVisibilityStatusInApp(Guid requestId);
        Task<Result> CancelRequest(Guid requestId, RequestCancellationDetailModel reason);
        Task<Result> ReduceWorkerQuantityByOne(Guid requestId);
        Task<Result> RejectWorker(Guid requestId, Guid workerId, CommentsModel model);
    }
}
