using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;

namespace Covenant.Common.Entities.Sales;

public class Deal
{
    public Deal() { }

    public Deal(string title, Guid userId, Guid companyProfileId, DateTime date, decimal value, DealType type, DealStatus status, Guid? documentId)
    {
        Id = Guid.NewGuid();
        Title = title;
        UserId = userId;
        CompanyProfileId = companyProfileId;
        Date = date;
        Value = value;
        Type = type;
        Status = status;
        DocumentId = documentId;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public Guid CompanyProfileId { get; private set; }
    public CompanyProfile CompanyProfile { get; private set; }
    public DateTime Date { get; private set; }
    public decimal Value { get; private set; }
    public DealType Type { get; private set; }
    public DealStatus Status { get; private set; }
    public Guid? DocumentId { get; internal set; }
    public CovenantFile Document { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public DateTime UpdatedAt { get; private set; } = DateTime.Now;

    public void Update(string title, DateTime date, decimal value, DealType type, DealStatus status, Guid? documentId)
    {
        Title = title;
        Date = date;
        Value = value;
        Type = type;
        Status = status;
        DocumentId = documentId;
        UpdatedAt = DateTime.Now;
    }

    public void AttachDocument(CovenantFile document) => Document = document;
}
