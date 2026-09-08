namespace Covenant.Common.Models.Company.SalesDashboard;

public class DealsByStatusModel
{
    public SalesPeriodRangeModel Period { get; set; }
    public int TotalCount { get; set; }
    public decimal TotalValue { get; set; }
    public List<DealStatusSummaryModel> Items { get; set; } = [];
}
