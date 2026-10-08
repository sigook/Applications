using Covenant.Common.Enums;
using Covenant.Common.Models.Company;

namespace Covenant.Common.Models.Sales;

public class UpdateCompanyInteractionModel
{
    public string Description { get; set; }
    public InteractionPurpose InteractionPurpose { get; set; }
    public InteractionType InteractionType { get; set; }
    public InteractionStatus InteractionStatus { get; set; }
}
