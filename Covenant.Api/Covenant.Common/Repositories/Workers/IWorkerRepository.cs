using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Workers.Agencies;
using Covenant.Common.Models.Workers;
using Covenant.Common.Models;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Workers;

public interface IWorkerRepository : IBaseRepository
{
    Task CreateWorkerProfileHoliday(WorkerProfileHoliday entity);
    Task<WorkerProfile> GetProfile(Expression<Func<WorkerProfile, bool>> condition);
    Task<PaginatedList<WorkerCommentModel>> GetComments(Expression<Func<WorkerComment, bool>> condition, Pagination pagination);
    Task<WorkerProfileBasicInfoModel> GetWorkerProfileBasicInfo(Guid workerProfileId);
    Task<WorkerProfileDetailModel> GetWorkerProfileDetail(Expression<Func<WorkerProfile, bool>> condition);
    Task<PaginatedList<WorkerProfileListModel>> GetWorkersProfile(Guid agencyId, GetWorkerProfileFilter filter);
    IQueryable<WorkerProfileListModel> GetAllWorkersProfile(Guid agencyId, GetWorkerProfileFilter filter);
    Task<List<AgencyWorkerDropdownModel>> GetWorkerProfilesDropdown(IEnumerable<Guid> agencyIds, string searchTerm);
    Task<List<CovenantFileModel>> GetOtherDocuments(Guid profileId);
    Task<WorkerProfileOtherDocument> GetOtherDocument(Guid otherDocumentId);
    Task<WorkerProfileLicense> GetLicense(Guid licenseId);
    Task<WorkerProfileCertificate> GetCertificate(Guid certificateId);
    Task<bool> InfoIsAlreadyTaken(Expression<Func<WorkerProfile, bool>> expression);
    Task<bool> SocialInsuranceIsAlreadyTaken(string socialInsurance, Guid? excludeProfileId = null);
    Task<List<WorkerProfileHolidayModel>> GetWorkerProfileHoliday(Guid workerProfileId);
    Task<PaginatedList<PayStubHistoryModel>> GetWageHistory(Guid agencyId, Guid workerProfileId, GetWorkerPayStubHistoryFilter filter);
    Task<PaginatedList<WorkerProfileNoteListModel>> GetWorkerProfileNotes(Guid workerProfileId, Pagination pagination);
    Task<List<WorkerContactInfoModel>> GetWorkersAvailableToInvite(Guid agencyId, Guid cityId);
    Task<IEnumerable<WorkerSINExpiredModel>> GetWorkersSinExpired(DateTime date);
    Task<IEnumerable<WorkerLicenseExpiredModel>> GetWorkerLicensesExpired(DateTime date);
    Task<WorkerProfileTaxCategory> GetWorkerProfileTaxCategory(Guid workerProfileId);
}