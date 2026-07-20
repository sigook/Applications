using Covenant.Common.Entities.Company;

namespace Covenant.Common.Models.Company;

public enum GetDealsSortBy : byte
{
    Date,
    Company,
    Value,
    Status
}

public class GetDealsFilter : Pagination
{
    public Guid? CompanyProfileId { get; set; }
    public Guid? OwnerId { get; set; }
    public Deal.DealType? Type { get; set; }
    public List<Deal.DealStatus> Statuses { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public GetDealsSortBy SortBy { get; set; }
}
