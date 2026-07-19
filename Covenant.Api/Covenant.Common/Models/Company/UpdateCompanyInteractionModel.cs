using Covenant.Common.Entities.Company;

namespace Covenant.Common.Models.Company;

public class UpdateCompanyInteractionModel
{
    public string Description { get; set; }
    public CompanyInteraction.Purpose InteractionPurpose { get; set; }
    public CompanyInteraction.Type InteractionType { get; set; }
    public CompanyInteraction.Status InteractionStatus { get; set; }
}
