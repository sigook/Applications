using Covenant.Common.Enums;

namespace Covenant.Common.Models.Company;

public class CompanyProfileDocumentModel : CovenantFileModel
{
    public CompanyProfileDocumentType DocumentType { get; set; }
}
