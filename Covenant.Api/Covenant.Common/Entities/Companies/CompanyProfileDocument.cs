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

        public Guid DocumentId { get; private set; }
        public CovenantFile Document { get; private set; }
        public Guid CompanyProfileId { get; private set; }
        public CompanyProfile CompanyProfile { get; internal set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public string CreatedBy { get; private set; }
        public CompanyProfileDocumentType DocumentType { get; private set; } = CompanyProfileDocumentType.None;

        public void UpdateDocumentType(CompanyProfileDocumentType documentType) => DocumentType = documentType;
    }
}
