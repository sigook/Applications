using Covenant.Common.Enums;

namespace Covenant.Common.Models.Company;

public class UpdateCompanyInteractionModel
{
    public string Description { get; set; }
    public InteractionPurpose InteractionPurpose { get; set; }
    public InteractionType InteractionType { get; set; }
    public InteractionStatus InteractionStatus { get; set; }
}
