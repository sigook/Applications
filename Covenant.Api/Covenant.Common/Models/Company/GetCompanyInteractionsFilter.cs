using Covenant.Common.Entities.Company;

namespace Covenant.Common.Models.Company;

public enum GetCompanyInteractionsSortBy : byte
{
    CreatedAt,
    Company,
    Status
}

public class GetCompanyInteractionsFilter : Pagination
{
    public Guid? CompanyProfileId { get; set; }
    public Guid? OwnerId { get; set; }
    public CompanyInteraction.Purpose? InteractionPurpose { get; set; }
    public CompanyInteraction.Type? InteractionType { get; set; }
    public List<CompanyInteraction.Status> Statuses { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public GetCompanyInteractionsSortBy SortBy { get; set; }
}
