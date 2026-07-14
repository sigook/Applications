using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Request;

namespace Covenant.Core.BL.Interfaces;

public interface ISalesService
{
    Task<AgencyRequestsPagedResponse> GetRequests(GetRequestForAgencyFilter filter);
    IEnumerable<AgencyRequestListModel> GetRequestsForReport(GetRequestForAgencyFilter filter);

    Task<PaginatedList<CompanyProfileListModel>> GetCompanies(GetCompanyForAgencyFilter filter);
    IEnumerable<CompanyProfileListModel> GetCompaniesForReport(GetCompanyForAgencyFilter filter);
}
