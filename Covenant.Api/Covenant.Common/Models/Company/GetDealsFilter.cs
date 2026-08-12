using Covenant.Common.Enums;

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
    public DealType? Type { get; set; }
    public List<DealStatus> Statuses { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public GetDealsSortBy SortBy { get; set; }
}
