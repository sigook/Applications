using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services;

public class SalesService(
    IRequestService requestService,
    IAgencyService agencyService,
    IRequestRepository requestRepository,
    ICompanyRepository companyRepository,
    IAgencyRepository agencyRepository,
    IIdentityServerService identityServerService) : ISalesService
{
    private const string NotPersonnelError = "The sales user does not belong to the agency";

    private Guid? SalesScope => identityServerService.IsSales() ? identityServerService.GetUserId() : null;

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

    public async Task<AgencyRequestDetailModel> GetRequestDetail(Guid requestId)
    {
        AgencyRequestDetailModel model = await requestRepository.GetRequestDetailForAgency(requestId);
        if (model is null || !identityServerService.IsSales()) return model;
        return model.SalesRepresentativeId == await GetSalesPersonnelId() ? model : null;
    }

    public async Task<Result<Guid>> CreateRequest(RequestCreateModel model)
    {
        if (identityServerService.IsSales())
        {
            Guid personnelId = await GetSalesPersonnelId();
            if (personnelId == Guid.Empty) return Result.Fail<Guid>(NotPersonnelError);
            model.SalesRepresentativeId = personnelId;
        }
        return await requestService.CreateRequest(model);
    }

    public async Task<Result> UpdateRequest(Guid requestId, RequestCreateModel model)
    {
        if (identityServerService.IsSales())
        {
            Guid personnelId = await GetSalesPersonnelId();
            if (personnelId == Guid.Empty) return Result.Fail(NotPersonnelError);

            RequestComission comission = await requestRepository.GetRequestComission(requestId);
            if (comission is null || comission.AgencyPersonnelId != personnelId)
                return Result.Fail("The request is not assigned to the sales user");

            model.SalesRepresentativeId = personnelId;
        }
        return await requestService.UpdateRequest(requestId, model);
    }

    public async Task<PaginatedList<CompanyProfileListModel>> GetCompanies(GetCompanyForAgencyFilter filter)
    {
        filter.SalesUserId = SalesScope;
        return await companyRepository.GetCompaniesProfileForAgency(identityServerService.GetAgencyId(), filter);
    }

    public IEnumerable<CompanyProfileListModel> GetCompaniesForReport(GetCompanyForAgencyFilter filter)
    {
        filter.SalesUserId = SalesScope;
        return companyRepository.GetAllCompaniesProfileForAgency(identityServerService.GetAgencyId(), filter);
    }

    public async Task<CompanyProfileDetailModel> GetCompanyDetail(Guid companyProfileId)
    {
        CompanyProfileDetailModel model = await companyRepository.GetCompanyProfileDetail(cp => cp.Id == companyProfileId);
        if (model is null || !identityServerService.IsSales()) return model;
        return model.SalesRepresentativeId == await GetSalesPersonnelId() ? model : null;
    }

    public async Task<Result<Guid>> CreateCompany(CompanyProfileDetailModel model)
    {
        if (identityServerService.IsSales())
        {
            Guid personnelId = await GetSalesPersonnelId();
            if (personnelId == Guid.Empty) return Result.Fail<Guid>(NotPersonnelError);
            model.SalesRepresentativeId = personnelId;
        }
        return await agencyService.CreateCompany(model);
    }

    public async Task<Result> UpdateCompany(Guid companyProfileId, CompanyProfileDetailModel model)
    {
        if (identityServerService.IsSales())
        {
            Guid personnelId = await GetSalesPersonnelId();
            if (personnelId == Guid.Empty) return Result.Fail(NotPersonnelError);

            CompanyProfileDetailModel current = await companyRepository.GetCompanyProfileDetail(cp => cp.Id == companyProfileId);
            if (current is null || current.SalesRepresentativeId != personnelId)
                return Result.Fail("The client is not assigned to the sales user");

            model.SalesRepresentativeId = personnelId;
        }
        return await agencyService.UpdateCompany(companyProfileId, model);
    }

    private void ApplyScope(GetRequestForAgencyFilter filter)
    {
        filter.HasPermissionToSeeInternalRequests = identityServerService.IsAdmin();
        filter.SalesUserId = SalesScope;
    }

    private async Task<Guid> GetSalesPersonnelId()
    {
        Guid agencyId = identityServerService.GetAgencyId();
        List<AgencyPersonnel> personnel = await agencyRepository.GetPersonnelByUserId(identityServerService.GetUserId());
        return personnel.FirstOrDefault(p => p.AgencyId == agencyId)?.Id ?? Guid.Empty;
    }
}
