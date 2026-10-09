using Covenant.Common.Enums;

namespace Covenant.Common.Models.Sales.Dashboard;

public class DealStatusSummaryModel
{
    public DealStatus Status { get; set; }
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
}
