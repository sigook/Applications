using Covenant.Common.Enums;

namespace Covenant.Common.Models.Company;

public class DealListModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid CompanyProfileId { get; set; }
    public string CompanyName { get; set; }
    public Guid OwnerId { get; set; }
    public string Owner { get; set; }
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public DealType Type { get; set; }
    public DealStatus Status { get; set; }
    public Guid? DocumentId { get; set; }
    public string DocumentName { get; set; }
    public string DocumentPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
