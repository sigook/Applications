using Covenant.Common.Enums;

namespace Covenant.Common.Models.Company.SalesDashboard;

public class SalesPeriodRangeModel
{
    public SalesPeriod Period { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string Label { get; set; }
    public string TimeZone { get; set; }
}
