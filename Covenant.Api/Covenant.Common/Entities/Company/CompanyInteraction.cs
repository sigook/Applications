using Covenant.Common.Enums;

namespace Covenant.Common.Entities.Company;

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

    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid CompanyProfileId { get; set; }
    public CompanyProfile CompanyProfile { get; set; }
    public InteractionPurpose InteractionPurpose { get; set; }
    public InteractionType InteractionType { get; set; }
    public InteractionStatus InteractionStatus { get; set; } = InteractionStatus.NotStarted;

    public void Update(string description, InteractionPurpose purpose, InteractionType type, InteractionStatus status)
    {
        Description = description;
        InteractionPurpose = purpose;
        InteractionType = type;
        InteractionStatus = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
