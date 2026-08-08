using Covenant.Common.Functionals;
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

    Task<PaginatedList<CompanyInteractionListModel>> GetInteractions(GetCompanyInteractionsFilter filter);
    Task<Result<Guid>> CreateInteraction(CreateCompanyInteractionModel model);
    Task<Result> UpdateInteraction(Guid id, UpdateCompanyInteractionModel model);
    Task<Result> DeleteInteraction(Guid id);

    Task<PaginatedList<DealListModel>> GetDeals(GetDealsFilter filter);
    Task<Result<Guid>> CreateDeal(CreateDealModel model);
    Task<Result> UpdateDeal(Guid id, UpdateDealModel model);
    Task<Result> DeleteDeal(Guid id);
}
