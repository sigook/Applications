using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting.Invoice.Company;
using Covenant.Common.Models.Company.Agency;
using Covenant.Common.Models.Website;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Company;
using Microsoft.AspNetCore.Http;

namespace Covenant.Core.BL.Interfaces.Companies;

public interface ICompanyService
{
    Task<Result<Guid>> CreateCompanyProfile(CompanyRegisterByItselfModel model);
    Task<Result> UpdateProfile(Guid profileId, CompanyProfileDetailModel model);
    Task<Result> UpdateLogo(Guid profileId);
    Task<Result<Guid>> CreateCompanyLocation(CompanyProfileLocationDetailModel model, Guid? profileId = null);
    Task<Result> UpdateCompanyLocation(Guid id, CompanyProfileLocationDetailModel model);
    Task<Result> RequestNewJobPosition(ContactDto contact);
    Task<Result> RequestNewWorker(Guid requestId, CommentsModel model);
    Task<PaginatedList<InvoiceListModel>> GetCompanyInvoices(GetCompanyInvoiceFilter filter);
    Task<Result> CreateCompanyUser(CompanyUserModel model, Guid? companyProfileId = null);
    Task<Result> CreateContact(CompanyProfileContactPersonModel model);
    Task<Result> DeleteCompanyUser(Guid userId, Guid? companyProfileId = null);
    Task<Result<ResultGenerateDocument<byte[]>>> BulkCompany(Guid agencyId, IFormFile file);
    Task<CompanyDeletionCheckModel> CheckCompanyDeletion(Guid companyProfileId);
    Task<Result> DeleteCompanyProfile(Guid companyProfileId);
}