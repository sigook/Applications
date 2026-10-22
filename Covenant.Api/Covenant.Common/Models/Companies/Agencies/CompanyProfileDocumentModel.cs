using Covenant.Common.Enums;

namespace Covenant.Common.Models.Companies.Agencies;

public class CompanyProfileDocumentModel : CovenantFileModel
{
    public CompanyProfileDocumentType DocumentType { get; set; }
}
