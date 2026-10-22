using Covenant.Common.Entities.Companies;
using Covenant.Common.Enums;

namespace Covenant.Common.Entities.Sales;

public class CompanyInteraction
{
    public CompanyInteraction() { }

    public CompanyInteraction(string description, Guid userId, Guid companyProfileId, InteractionPurpose purpose, InteractionType type, InteractionStatus status)
    {
        Id = Guid.NewGuid();
        Description = description;
        UserId = userId;
        CompanyProfileId = companyProfileId;
        InteractionPurpose = purpose;
        InteractionType = type;
        InteractionStatus = status;
    }

    public Guid Id { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public DateTime CreatedAt { get; internal set; } = DateTime.Now;
    public DateTime UpdatedAt { get; private set; } = DateTime.Now;
    public Guid CompanyProfileId { get; private set; }
    public CompanyProfile CompanyProfile { get; private set; }
    public InteractionPurpose InteractionPurpose { get; private set; }
    public InteractionType InteractionType { get; private set; }
    public InteractionStatus InteractionStatus { get; private set; } = InteractionStatus.NotStarted;

    public void Update(string description, InteractionPurpose purpose, InteractionType type, InteractionStatus status)
    {
        Description = description;
        InteractionPurpose = purpose;
        InteractionType = type;
        InteractionStatus = status;
        UpdatedAt = DateTime.Now;
    }
}
