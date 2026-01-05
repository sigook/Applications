namespace Covenant.Common.Entities.Company
{
    public class CompanyProfileInvoiceRecipient
    {
        private CompanyProfileInvoiceRecipient()
        {
        }

        public CompanyProfileInvoiceRecipient(Guid companyProfileId, string email, Guid id = default)
        {
            CompanyProfileId = companyProfileId;
            Email = email;
            Id = id == default ? Guid.NewGuid() : id;
        }

        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string Name { get; set; }
        public CompanyProfile CompanyProfile { get; set; }
        public Guid CompanyProfileId { get; private set; }

        public void UpdateEmail(string email) => this.Email = email;
    }
}