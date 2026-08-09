using Covenant.Common.Enums;

namespace Covenant.Common.Models.Company;

public class CreateDealModel
{
    public string Title { get; set; }
    public Guid CompanyProfileId { get; set; }
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public DealType Type { get; set; }
    public DealStatus Status { get; set; }
    public Guid? DocumentId { get; set; }
}
