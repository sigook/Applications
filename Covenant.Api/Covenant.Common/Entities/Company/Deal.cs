namespace Covenant.Common.Entities.Company;

public class Deal
{
    public Deal() { }

    public Deal(string title, Guid ownerId, Guid companyId, DateTime date, decimal value, DealType type, DealStatus status, Guid? documentId)
    {
        Id = Guid.NewGuid();
        Title = title;
        OwnerId = ownerId;
        CompanyId = companyId;
        Date = date;
        Value = value;
        Type = type;
        Status = status;
        DocumentId = documentId;
    }

    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public User Owner { get; set; }
    public Guid CompanyId { get; set; }
    public CompanyProfile Company { get; set; }
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public DealType Type { get; set; }
    public DealStatus Status { get; set; }
    public Guid? DocumentId { get; set; }
    public CovenantFile Document { get; set; }

    public void Update(string title, DateTime date, decimal value, DealType type, DealStatus status, Guid? documentId)
    {
        Title = title;
        Date = date;
        Value = value;
        Type = type;
        Status = status;
        DocumentId = documentId;
    }

    public enum DealType
    {
        Temporal = 0,
        Permanent = 1,
        TempToPerm = 2
    }

    public enum DealStatus
    {
        ToSend = 0,
        Sent = 1,
        Rejected = 2,
        Accepted = 3
    }
}
