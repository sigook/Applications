using Covenant.Common.Enums;

namespace Covenant.Common.Models.Company;

public class CreateCompanyInteractionModel
{
    public Guid CompanyProfileId { get; set; }
    public string Description { get; set; }
    public InteractionPurpose InteractionPurpose { get; set; }
    public InteractionType InteractionType { get; set; }
    public InteractionStatus InteractionStatus { get; set; }
}
