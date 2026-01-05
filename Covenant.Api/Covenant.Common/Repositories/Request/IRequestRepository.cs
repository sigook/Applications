using Covenant.Common.Entities.Request;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.WebSite;
using Covenant.Common.Models.Worker;
using Covenant.Common.Models.Request;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Request;

public interface IRequestRepository
{
    Task Create<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    Task Update<T>(T entity) where T : class;
    Task<Entities.Request.Request> GetRequest(Expression<Func<Entities.Request.Request, bool>> condition);
    Task<IEnumerable<Entities.Request.Request>> GetRequests(IEnumerable<Guid> ids);
    Task<PaginatedList<WorkerRequestAgencyBoardModel>> GetWorkersRequestBoard(Guid agencyId, Pagination pagination);
    IEnumerable<AgencyRequestListModel> GetAllRequestsForAgency(Guid agencyId, GetRequestForAgencyFilter filter);
    Task<PaginatedList<AgencyRequestListModel>> GetRequestsForAgency(Guid agencyId, GetRequestForAgencyFilter filter);
    Task<PaginatedList<RequestListModel>> GetRequestsForCompany(Guid companyId, GetRequestForCompanyFilter filter);
    Task<IEnumerable<JobViewModel>> GetAvailableRequest(IEnumerable<string> countries);
    Task<AgencyRequestDetailModel> GetRequestDetailForAgency(Guid id);
    Task<CompanyRequestDetailModel> GetRequestDetailForCompany(Guid id);
    Task<PaginatedList<AgencyWorkerRequestModel>> GetWorkersRequestByRequestId(Guid requestId, GetWorkersRequestFilter filter);
    Task<AgencyWorkerRequestModel> GetWorkerRequestByAgencyId(Guid agencyId, Guid requestId, Guid workerRequestId);
    Task<PaginatedList<WorkerRequestListModel>> GetRequestsForWorker(Guid workerId, Pagination pagination);
    Task<WorkerRequestDetailModel> GetRequestDetailForWorker(Guid workerId, Guid requestId);
    Task<PaginatedList<WorkerRequestListModel>> GetRequestsHistoryForWorker(Guid workerId, Pagination pagination);
    Task<PaginatedList<RequestListModel>> GetRequestsHistoryByWorkerProfileId(Guid workerProfileId, Pagination pagination);
    Task<AgencyWorkerRequestModel> GetRequestWorkerByCompanyId(Guid companyId, Guid requestId, Guid workerId);
    Task<ShiftModel> GetRequestShift(Guid requestId);
    Task<RequestCancellationDetail> GetRequestCancellationDetail(Guid requestId);
    Task<RequestFinalizationDetail> GetRequestFinalizationDetail(Guid requestId);
    Task<PaginatedList<AgencyWorkerRequestModel>> GetAllWorkersThatCanApplyToRequest(Guid agencyId, Guid requestId, Pagination pagination, string filter);
    Task<RequestContactPersonDetailModel> GetRequestedByDetail(Guid requestId, Guid contactPersonId);
    Task<PaginatedList<RequestContactPersonModel>> GetRequestedByList(Guid requestId, Pagination pagination);
    Task<RequestRequestedBy> GetRequestedBy(Guid requestId, Guid contactPersonId);
    Task<PaginatedList<RequestContactPersonModel>> GetReportToList(Guid requestId, Pagination pagination);
    Task<RequestContactPersonDetailModel> GetReportToDetail(Guid requestId, Guid contactPersonId);
    Task<RequestReportTo> GetReportTo(Guid requestId, Guid contactPersonId);
    Task<PaginatedList<NoteModel>> GetNotes(Guid requestId, Pagination pagination);
    Task<NoteModel> GetNoteDetail(Guid requestId, Guid id);
    Task<RequestNote> GetNote(Guid requestId, Guid id);
    Task<PaginatedList<RequestRecruiterDetailModel>> GetRecruiters(Guid requestId, Pagination pagination);
    Task<RequestSkill> GetSkill(Guid requestId, Guid id);
    Task<IEnumerable<SkillModel>> GetSkills(Guid requestId);
    Task<RequestApplicant> GetRequestApplicant(Expression<Func<RequestApplicant, bool>> expression);
    Task<IEnumerable<RequestApplicant>> GetRequestApplicants(Expression<Func<RequestApplicant, bool>> expression);
    Task<PaginatedList<RequestApplicantDetailModel>> GetRequestApplicants(Guid requestId, GetRequestApplicantFilter filter);
    Task<RequestComission> GetRequestComission(Guid requestId);
    Task<IEnumerable<RequestCompanyUser>> GetRequestCompanyUsers(Guid requestId);
    Task SaveChangesAsync();
    Task PutRequestInProgress();
    Task<IEnumerable<CompanyProfileListModel>> GetCompaniesWithRequests(IEnumerable<Guid> agencyIds);
    Task<bool> ExistsRequestByNumber(int orderId);
}