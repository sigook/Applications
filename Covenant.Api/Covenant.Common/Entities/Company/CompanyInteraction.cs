namespace Covenant.Common.Entities.Company;

public class CompanyInteraction
{
    public CompanyInteraction() { }

    public CompanyInteraction(string description, User owner, CompanyProfile company, Purpose purpose, Type type, Status status)
    {
        Id = Guid.NewGuid();
        Description = description;
        Owner = owner;
        Company = company;
        InteractionPurpose = purpose;
        InteractionType = type;
        InteractionStatus = status;
    }

    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public User Owner { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set;} = DateTime.UtcNow;
    public CompanyProfile Company { get; set; }
    public Purpose InteractionPurpose { get; set; }
    public Type InteractionType { get; set; }
    public Status InteractionStatus { get; set; } = Status.NotStarted;

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
