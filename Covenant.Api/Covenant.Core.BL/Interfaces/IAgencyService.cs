using Covenant.Common.Entities.Company;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Security;
using Covenant.Common.Models.Worker;

namespace Covenant.Core.BL.Interfaces;

public interface IAgencyService
{
    Task<Result<Guid>> CreateCompany(CompanyProfileDetailModel model);
    Task<Result> UpdateCompany(Guid companyProfileId, CompanyProfileDetailModel model);
    Task<Result<Guid>> CreateCompanyContactPerson(Guid profileId, CompanyProfileContactPersonModel model);
    Task<Result> UpdateCompanyContactPerson(Guid id, CompanyProfileContactPersonModel model);
    Task<Result<Guid>> CreateCompanyJobPosition(Guid profileId, CompanyProfileJobPositionRateModel model);
    Task<Result> UpdateCompanyJobPosition(Guid id, Guid profileId, CompanyProfileJobPositionRateModel model);
    Task NotifySinsExpired();
    Task NotifyLicensesExpired();
    Task<Result> IncreaseWorkersQuantityByOne(Guid requestId);
    Task<Result<Guid>> BookWorker(Guid requestId, Guid workerId, AgencyBookWorkerModel model);
    Task UpdateWorkerProfileTaxCategory(Guid workerProfileId, WorkerProfileDetailModel model);
    Task UpdateWorkerProfileTaxRate(Guid workerProfileId, WorkerProfileDetailModel model);
    Task<Result<CompanyProfileDocument>> CreateCompanyDocument(Guid companyProfileId, CompanyProfileDocumentModel model);
    Task<PaginatedList<CompanyProfileDocumentModel>> GetCompanyDocuments(Guid compnayProfileId, Pagination pagination);
    Task DeleteCompanyDocument(Guid companyProfileDocumentId);
    Task<Result> UpdateEmailCompanyProfile(Guid companyProfileId, UpdateEmailModel model);
    Task<Result> AddUpdateWorkerHoliday(Guid workerProfileId, WorkerProfileHolidayModel model);
    Task<Result> CreateHoliday(WorkerProfileHolidayModel model);
    Task<Result> CreateAgencyPersonnel(AgencyPersonnelModel model, Guid? agencyId = null);
    Task<Result> CreateAgency(AgencyModel model);
}
