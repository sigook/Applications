using Covenant.Common.Enums;
using Covenant.Common.Models.Company;

namespace Covenant.Common.Models.Sales;

public enum GetCompanyInteractionsSortBy : byte
{
    CreatedAt,
    Status
}

public class GetCompanyInteractionsFilter : Pagination
{
    public Guid? OwnerId { get; set; }
    public InteractionPurpose? InteractionPurpose { get; set; }
    public InteractionType? InteractionType { get; set; }
    public List<InteractionStatus> Statuses { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public GetCompanyInteractionsSortBy SortBy { get; set; }
}
