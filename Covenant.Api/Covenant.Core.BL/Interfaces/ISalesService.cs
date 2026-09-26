using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Company.SalesDashboard;
using Covenant.Common.Models.Request;

namespace Covenant.Core.BL.Interfaces;

public interface ISalesService
{
    Task<AgencyRequestsPagedResponse> GetRequests(GetRequestForAgencyFilter filter);
    IEnumerable<AgencyRequestListModel> GetRequestsForReport(GetRequestForAgencyFilter filter);

    Task<PaginatedList<CompanyProfileListModel>> GetCompanies(GetCompanyForAgencyFilter filter);
    IEnumerable<CompanyProfileListModel> GetCompaniesForReport(GetCompanyForAgencyFilter filter);

    Task<PaginatedList<CompanyInteractionListModel>> GetInteractions(Guid companyProfileId, GetCompanyInteractionsFilter filter);
    Task<Result<Guid>> CreateInteraction(Guid companyProfileId, CreateCompanyInteractionModel model);
    Task<Result> UpdateInteraction(Guid companyProfileId, Guid id, UpdateCompanyInteractionModel model);
    Task<Result> DeleteInteraction(Guid companyProfileId, Guid id);

    Task<PaginatedList<DealListModel>> GetDeals(Guid companyProfileId, GetDealsFilter filter);
    Task<Result<Guid>> CreateDeal(Guid companyProfileId);
    Task<Result> UpdateDeal(Guid companyProfileId, Guid id);
    Task<Result> DeleteDeal(Guid companyProfileId, Guid id);

    Task<Result<DealsByStatusModel>> GetDealsByStatus(GetDealsByStatusFilter filter);
    Task<SalesDashboardSummaryModel> GetDashboardSummary(GetSalesDashboardSummaryFilter filter);
    Task<List<RecentClientModel>> GetRecentClients();
    Task<List<CompanyInteractionListModel>> GetRecentInteractions();
    Task<List<DealListModel>> GetRecentDeals();
}
