using Covenant.Common.Entities.Companies;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Agencies;
using Covenant.Common.Models.Companies.Agencies;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models.Identity;
using Covenant.Common.Models.Locations;
using Covenant.Common.Models.Workers.Agencies;
using Covenant.Common.Models.Workers;
using Covenant.Common.Models;

namespace Covenant.Core.BL.Interfaces.Agencies;

public interface IAgencyService
{
    Task<Result<Guid>> CreateCompany(CompanyProfileDetailModel model);
    Task<Result> UpdateCompany(Guid companyProfileId, CompanyProfileDetailModel model);
    Task<Result<Guid>> CreateCompanyContactPerson(Guid profileId, CompanyProfileContactPersonModel model);
    Task<Result> UpdateCompanyContactPerson(Guid id, CompanyProfileContactPersonModel model);
    Task<Result<Guid>> CreateCompanyJobPosition(Guid profileId, CompanyProfileJobPositionRateModel model);
    Task<Result> UpdateCompanyJobPosition(Guid id, Guid profileId, CompanyProfileJobPositionRateModel model);
    Task<Result> DeleteCompanyJobPosition(Guid id);
    Task NotifySinsExpired();
    Task NotifyLicensesExpired();
    Task<Result> IncreaseWorkersQuantityByOne(Guid requestId);
    Task<Result<Guid>> BookWorker(Guid requestId, Guid workerProfileId, AgencyBookWorkerModel model);
    Task UpdateWorkerProfileTaxCategory(Guid workerProfileId, WorkerProfileDetailModel model);
    Task UpdateWorkerProfileTaxRate(Guid workerProfileId, WorkerProfileDetailModel model);
    Task<Result<CompanyProfileDocument>> CreateCompanyDocument(Guid companyProfileId);
    Task<PaginatedList<CompanyProfileDocumentModel>> GetCompanyDocuments(Guid compnayProfileId, Pagination pagination);
    Task DeleteCompanyDocument(Guid companyProfileDocumentId);
    Task<Result> UpdateEmailCompanyProfile(Guid companyProfileId, UpdateEmailModel model);
    Task<Result> AddUpdateWorkerHoliday(Guid workerProfileId, WorkerProfileHolidayModel model);
    Task<Result> CreateHoliday(WorkerProfileHolidayModel model);
    string[] GetAssignableRoles();
    Task<IEnumerable<AgencyPersonnelModel>> GetAgencyPersonnel(Guid agencyId);
    Task<AgencyPersonnelModel> GetAgencyPersonnel(Guid agencyId, Guid id);
    Task<Result> CreateAgencyPersonnel(AgencyPersonnelModel model, Guid? agencyId = null);
    Task<Result> UpdateAgencyPersonnel(Guid id, AgencyPersonnelModel model);
    Task<Result> DeleteAgencyPersonnel(Guid agencyId, Guid id);
    Task SetPrimaryPersonnelAgency(Guid userId, Guid personnelId);
    Task<Result> UpdateAgencyProfile(Guid agencyId, AgencyModel model);
    Task<Result<Guid>> CreateAgencyLocation(Guid agencyId, LocationModel model);
    Task<Result> UpdateAgencyLocation(Guid agencyId, Guid id, LocationModel model);
    Task<Result> DeleteAgencyLocation(Guid agencyId, Guid id);
    Task<Result> CreateAgency(AgencyModel model);
    Task<Result<UserAttendanceTodayModel>> GetUserAttendanceToday(string timeZone);
    Task<Result<UserAttendanceTodayModel>> ToggleUserAttendance(string timeZone);
    Task<Result<List<UserAttendanceTodayModel>>> GetUserAttendancesToday(string timeZone);
    Task<Result<UserAttendanceReportModel>> GetUserAttendanceReport(GetUserAttendanceReportFilter filter);
    Task<Result<ResultGenerateDocument<MemoryStream>>> GetUserAttendanceReportFile(GetUserAttendanceReportFilter filter);
    Task<Result> UpdateUserAttendance(Guid id, UpdateUserAttendanceModel model);
}
