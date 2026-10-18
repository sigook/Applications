namespace Covenant.Common.Models.Sales.Dashboard;

public class SalesDashboardModel
{
    public SalesDashboardSummaryModel Summary { get; set; }
    public DealsByStatusModel DealsByStatus { get; set; }
    public List<RecentClientModel> RecentClients { get; set; } = [];
    public List<CompanyInteractionListModel> RecentInteractions { get; set; } = [];
    public List<DealListModel> RecentDeals { get; set; } = [];
}
