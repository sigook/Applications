using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;

namespace Covenant.Core.BL.Interfaces;

public interface IRequestApplicantService
{
    Task<Result<RequestApplicantDetailModel>> Create(Guid requestId, RequestApplicantModel model);
    Task<Result> UpdateComments(Guid applicantId, string comments);
    Task<PaginatedList<RequestApplicantDetailModel>> GetApplicants(Guid requestId, GetRequestApplicantFilter filter);
    Task<List<ApplicantSearchResultModel>> Search(Guid requestId, string searchTerm);
    Task<Result> Delete(Guid applicantId);
    Task<Result> ChangeStatus(Guid requestId, Guid applicantId, ChangeRequestApplicantStatusModel model);
    Task<Result<List<ApplicantComplianceItemModel>>> GetComplianceItems(Guid requestId, Guid applicantId);
    Task<Result> CompleteComplianceItem(Guid requestId, Guid applicantId, Guid itemId, CompleteApplicantComplianceItemModel model);
    Task<Result> UncompleteComplianceItem(Guid requestId, Guid applicantId, Guid itemId);
}
