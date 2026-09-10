using Covenant.Common.Enums;

namespace Covenant.Common.Models.Company.SalesDashboard;

public class GetDealsByStatusFilter
{
    public SalesPeriod Period { get; set; } = SalesPeriod.Week;
    public List<DealStatus> Statuses { get; set; }
    public Guid? OwnerId { get; set; }
}
