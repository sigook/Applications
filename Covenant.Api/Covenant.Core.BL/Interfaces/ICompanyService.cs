using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Company;
using Microsoft.AspNetCore.Http;

namespace Covenant.Core.BL.Interfaces;

public interface ICompanyService
{
    Task<Result<Guid>> CreateCompanyProfile(CompanyRegisterByItselfModel model);
    Task<Result> UpdateProfile(Guid profileId, CompanyProfileDetailModel model);
    Task<Result<Guid>> CreateCompanyLocation(CompanyProfileLocationDetailModel model, Guid? profileId = null);
    Task<Result> UpdateCompanyLocation(Guid id, CompanyProfileLocationDetailModel model);
    Task<Result> RequestNewJobPosition(ContactDto contact);
    Task<Result> RequestNewWorker(Guid requestId, CommentsModel model);
    Task<PaginatedList<InvoiceListModel>> GetCompanyInvoices(GetCompanyInvoiceFilter filter);
    Task<Result> CreateCompanyUser(CompanyUserModel model, Guid? companyId = null);
    Task<Result> CreateContact(CompanyProfileContactPersonModel model);
    Task<Result> DeleteCompanyUser(Guid userId, Guid? companyId = null);
    Task<Result<ResultGenerateDocument<byte[]>>> BulkCompany(Guid agencyId, IFormFile file);
}