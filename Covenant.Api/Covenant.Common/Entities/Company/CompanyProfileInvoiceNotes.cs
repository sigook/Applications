namespace Covenant.Common.Entities.Company
{
    public class CompanyProfileInvoiceNotes
    {
        private CompanyProfileInvoiceNotes() 
        { 
        }

        public CompanyProfileInvoiceNotes(Guid companyProfileId, string htmlNotes, Guid id = default)
        {
            CompanyProfileId = companyProfileId;
            HtmlNotes = htmlNotes;
            Id = id == default ? Guid.NewGuid() : id;
        }

        public Guid Id { get; private set; }
        public CompanyProfile CompanyProfile { get; private set; }
        public Guid CompanyProfileId { get; private set; }
        public string HtmlNotes { get; private set; }

        public void ChangeHtmlNotes(string htmlNotes)
        {
            HtmlNotes = htmlNotes;
        }
    }
}