using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services;

public class SalesService(
    IRequestService requestService,
    IRequestRepository requestRepository,
    ICompanyRepository companyRepository,
    IIdentityServerService identityServerService) : ISalesService
{
    private Guid? SalesScope => identityServerService.IsSales() ? identityServerService.GetAgencyPersonnelId() : null;

    public async Task<AgencyRequestsPagedResponse> GetRequests(GetRequestForAgencyFilter filter)
    {
        Guid agencyId = filter.AgencyId ?? identityServerService.GetAgencyId();
        ApplyScope(filter);
        return await requestService.GetRequestsForAgency(agencyId, filter);
    }

    public IEnumerable<AgencyRequestListModel> GetRequestsForReport(GetRequestForAgencyFilter filter)
    {
        ApplyScope(filter);
        return requestRepository.GetAllRequestsForAgency(identityServerService.GetAgencyId(), filter);
    }

    public async Task<PaginatedList<CompanyProfileListModel>> GetCompanies(GetCompanyForAgencyFilter filter)
    {
        filter.SalesPersonnelId = SalesScope;
        return await companyRepository.GetCompaniesProfileForAgency(identityServerService.GetAgencyId(), filter);
    }

    public IEnumerable<CompanyProfileListModel> GetCompaniesForReport(GetCompanyForAgencyFilter filter)
    {
        filter.SalesPersonnelId = SalesScope;
        return companyRepository.GetAllCompaniesProfileForAgency(identityServerService.GetAgencyId(), filter);
    }

    private void ApplyScope(GetRequestForAgencyFilter filter)
    {
        filter.HasPermissionToSeeInternalRequests = identityServerService.IsAdmin();
        filter.SalesPersonnelId = SalesScope;
    }
}
