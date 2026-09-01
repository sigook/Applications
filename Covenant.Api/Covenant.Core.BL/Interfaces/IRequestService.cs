using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Request;

namespace Covenant.Core.BL.Interfaces
{
    public interface IRequestService
    {
        Task<Result> SendInvitation(Guid requestId);
        Task<Result> OpenRequest(Guid requestId, string finalizedBy);
        Task<Result<Guid>> CreateRequest(RequestCreateModel model);
        Task<Result<Guid>> CompanyCreateRequest(RequestCreateModel model);
        Task<Result> UpdateRequest(Guid requestId, RequestCreateModel model);
        Task<Result> UpdateRequirements(Guid id, RequestUpdateRequirementsModel model);
        Task<Result> UpdateIsAsap(Guid id);
        Task<Result> UpdateIsAsapRequests(RequestsQuickUpdate requestsQuickUpdate);
        Task<Result> CancelRequest(Guid requestId, RequestCancellationDetailModel reason);
        Task<Result<BulkRequestCancellationResult>> BulkCancelRequests(BulkRequestCancellation model);
        Task<Result> BulkUpdateRecruiters(BulkRequestRecruiters model);
        Task<Result> ReduceWorkerQuantityByOne(Guid requestId);
        Task<Result> RejectWorker(Guid requestId, Guid workerProfileId, CommentsModel model);
        Task<Result<IEnumerable<RequestSourceDetailModel>>> GetRequestSources(Guid requestId);
        Task<Result> SetRequestSources(Guid requestId, IEnumerable<CreateRequestSourceModel> sources);
        Task<AgencyRequestsPagedResponse> GetRequestsForAgency(Guid agencyId, GetRequestForAgencyFilter filter);
        Task<ShiftModel> GetRequestShift(Guid requestId);
    }
}
