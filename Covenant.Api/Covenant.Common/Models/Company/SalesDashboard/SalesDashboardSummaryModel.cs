namespace Covenant.Common.Models.Company.SalesDashboard;

public class SalesDashboardSummaryModel
{
    public DateTime AsOf { get; set; }
    public SalesPeriodRangeModel Quarter { get; set; }
    public SalesPeriodRangeModel Week { get; set; }
    public List<DealStatusSummaryModel> Pipeline { get; set; } = [];
    public List<InteractionTypeSummaryModel> Activity { get; set; } = [];
}
