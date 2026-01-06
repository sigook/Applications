using Covenant.Common.Entities;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Worker;

public interface IWorkerRepository
{
    Task Create<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    Task CreateWorkerProfileHoliday(WorkerProfileHoliday entity);
    Task UpdateProfile(WorkerProfile entity);
    Task<List<WorkerProfileAgencyListModel>> GetProfiles(Guid workerId);
    Task<WorkerProfile> GetProfile(Expression<Func<WorkerProfile, bool>> condition);
    Task<WorkerProfileBasicInfoModel> GetWorkerProfileBasicInfo(Guid workerProfileId);
    Task<WorkerProfileDetailModel> GetWorkerProfileDetail(Guid profileId);
    Task<PaginatedList<WorkerProfileListModel>> GetWorkersProfile(Guid agencyId, GetWorkerProfileFilter filter);
    IEnumerable<WorkerProfileListModel> GetAllWorkersProfile(Guid agencyId, GetWorkerProfileFilter filter);
    Task<List<AgencyWorkerDropdownModel>> GetWorkerProfilesDropdown(IEnumerable<Guid> agencyIds, string searchTerm);
    Task<List<CovenantFileModel>> GetOtherDocuments(Guid profileId);
    Task<WorkerProfileOtherDocument> GetOtherDocument(Guid otherDocumentId);
    Task<WorkerProfileLicense> GetLicense(Guid licenseId);
    Task<WorkerProfileCertificate> GetCertificate(Guid certificateId);
    Task<bool> InfoIsAlreadyTaken(Expression<Func<WorkerProfile, bool>> expression);
    Task<List<WorkerProfileHolidayModel>> GetWorkerProfileHoliday(Guid workerProfileId);
    Task<WorkerProfilePunchCardIdModel> GetWorkerProfilePunchCarId(string punchCardId);
    Task<WorkerProfilePunchCardIdModel> GetWorkerProfilePunchCarId(Guid profileId);
    Task<PaginatedList<PayStubHistoryModel>> GetWageHistory(Guid workerProfileId, Pagination pagination);
    Task<PayStubHistoryAccumulated> GetWageHistoryAccumulated(Guid workerProfileId, int rowNumber);
    Task<PaginatedList<WorkerProfileNoteListModel>> GetWorkerProfileNotes(Guid workerProfileId, Pagination pagination);
    Task<PaginatedList<WorkerContactInfoModel>> GetWorkersAvailableToInvite(AvailableToInvitePagination pagination);
    Task<IEnumerable<WorkerSINExpiredModel>> GetWorkersSinExpired(DateTime date);
    Task<IEnumerable<WorkerLicenseExpiredModel>> GetWorkerLicensesExpired(DateTime date);
    Task<WorkerProfileTaxCategory> GetWorkerProfileTaxCategory(Guid workerProfileId);
    Task SaveChangesAsync();
}