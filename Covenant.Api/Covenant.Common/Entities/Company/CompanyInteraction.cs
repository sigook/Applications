namespace Covenant.Common.Entities.Company;

public class CompanyInteraction
{
    public CompanyInteraction() { }

    public CompanyInteraction(string description, Guid ownerId, Guid companyId, Purpose purpose, Type type, Status status)
    {
        Id = Guid.NewGuid();
        Description = description;
        OwnerId = ownerId;
        CompanyId = companyId;
        InteractionPurpose = purpose;
        InteractionType = type;
        InteractionStatus = status;
    }

    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public User Owner { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid CompanyId { get; set; }
    public CompanyProfile Company { get; set; }
    public Purpose InteractionPurpose { get; set; }
    public Type InteractionType { get; set; }
    public Status InteractionStatus { get; set; } = Status.NotStarted;

    public void Update(string description, Purpose purpose, Type type, Status status)
    {
        Description = description;
        InteractionPurpose = purpose;
        InteractionType = type;
        InteractionStatus = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public enum Purpose
    {
        Intro = 0,
        FollowUp = 1,
        Proposal = 2,
        Negotiation = 3,
        Closing = 4
    }
    public enum Type
    {
        Call = 0,
        Mail = 1,
        Sms = 2,
        LinkedIn = 3
    }
    public enum Status
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2
    }
}
