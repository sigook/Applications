using Covenant.Common.Enums;

namespace Covenant.Common.Models.Company;

public class GetCompanyInteractionsFilter : Pagination
{
    public Guid? CompanyProfileId { get; set; }
    public Guid? OwnerId { get; set; }
    public InteractionPurpose? InteractionPurpose { get; set; }
    public InteractionType? InteractionType { get; set; }
    public List<InteractionStatus> Statuses { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public GetCompanyInteractionsSortBy SortBy { get; set; }
}
