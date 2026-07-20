using Covenant.Common.Entities.Company;

namespace Covenant.Common.Models.Company;

public class CreateDealModel
{
    public string Title { get; set; }
    public Guid CompanyProfileId { get; set; }
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public Deal.DealType Type { get; set; }
    public Deal.DealStatus Status { get; set; }
    public Guid? DocumentId { get; set; }
}
