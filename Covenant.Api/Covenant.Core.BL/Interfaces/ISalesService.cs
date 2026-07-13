using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Request;

namespace Covenant.Core.BL.Interfaces;

public interface ISalesService
{
    Task<AgencyRequestsPagedResponse> GetRequests(GetRequestForAgencyFilter filter);
    IEnumerable<AgencyRequestListModel> GetRequestsForReport(GetRequestForAgencyFilter filter);
    Task<AgencyRequestDetailModel> GetRequestDetail(Guid requestId);
    Task<Result<Guid>> CreateRequest(RequestCreateModel model);
    Task<Result> UpdateRequest(Guid requestId, RequestCreateModel model);

    Task<PaginatedList<CompanyProfileListModel>> GetCompanies(GetCompanyForAgencyFilter filter);
    IEnumerable<CompanyProfileListModel> GetCompaniesForReport(GetCompanyForAgencyFilter filter);
    Task<CompanyProfileDetailModel> GetCompanyDetail(Guid companyProfileId);
    Task<Result<Guid>> CreateCompany(CompanyProfileDetailModel model);
    Task<Result> UpdateCompany(Guid companyProfileId, CompanyProfileDetailModel model);
}
