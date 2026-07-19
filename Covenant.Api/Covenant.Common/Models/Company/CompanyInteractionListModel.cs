using Covenant.Common.Entities.Company;

namespace Covenant.Common.Models.Company;

public class CompanyInteractionListModel
{
    public Guid Id { get; set; }
    public Guid CompanyProfileId { get; set; }
    public string CompanyName { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; }
    public string Description { get; set; }
    public CompanyInteraction.Purpose InteractionPurpose { get; set; }
    public CompanyInteraction.Type InteractionType { get; set; }
    public CompanyInteraction.Status InteractionStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
