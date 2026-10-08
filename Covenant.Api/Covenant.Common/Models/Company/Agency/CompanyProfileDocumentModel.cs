using Covenant.Common.Enums;

namespace Covenant.Common.Models.Company.Agency;

public class CompanyProfileDocumentModel : CovenantFileModel
{
    public CompanyProfileDocumentType DocumentType { get; set; }
}
