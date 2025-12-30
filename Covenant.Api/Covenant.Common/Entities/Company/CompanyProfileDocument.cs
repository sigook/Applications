using Covenant.Common.Enums;

namespace Covenant.Common.Entities.Company
{
    public class CompanyProfileDocument
    {
        public CompanyProfileDocument()
        {
        }

        public CompanyProfileDocument(Guid companyProfileId, CovenantFile document, string createdBy)
        {
            CompanyProfileId = companyProfileId;
            Document = document ?? throw new ArgumentNullException(nameof(document));
            DocumentId = document.Id;
            CreatedBy = createdBy;
        }

        public Guid DocumentId { get; set; }
        public CovenantFile Document { get; set; }
        public Guid CompanyProfileId { get; set; }
        public CompanyProfile CompanyProfile { get; internal set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public CompanyProfileDocumentType DocumentType { get; set; } = CompanyProfileDocumentType.None;
    }
}