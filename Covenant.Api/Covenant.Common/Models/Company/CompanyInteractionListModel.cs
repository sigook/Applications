using Covenant.Common.Enums;

namespace Covenant.Common.Models.Company;

public class CompanyInteractionListModel
{
    public Guid Id { get; set; }
    public Guid CompanyProfileId { get; set; }
    public string CompanyName { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; }
    public string Description { get; set; }
    public InteractionPurpose InteractionPurpose { get; set; }
    public InteractionType InteractionType { get; set; }
    public InteractionStatus InteractionStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
