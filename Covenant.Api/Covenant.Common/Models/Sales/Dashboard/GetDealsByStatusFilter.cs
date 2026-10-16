using Covenant.Common.Enums;

namespace Covenant.Common.Models.Sales.Dashboard;

public class GetDealsByStatusFilter
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public List<DealStatus> Statuses { get; set; }
    public Guid? OwnerId { get; set; }
}
