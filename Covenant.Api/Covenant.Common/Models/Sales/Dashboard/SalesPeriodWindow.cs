namespace Covenant.Common.Models.Sales.Dashboard;

public class SalesPeriodWindow
{
    public SalesPeriodRangeModel Range { get; set; }
    public DateTime FromUtc { get; set; }
    public DateTime ToUtcExclusive { get; set; }
}
