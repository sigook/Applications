namespace Covenant.Common.Entities.Companies
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
        public string Name { get; private set; }
        public CompanyProfile CompanyProfile { get; private set; }
        public Guid CompanyProfileId { get; private set; }

        public void UpdateEmail(string email) => this.Email = email;

        public void UpdateName(string name) => Name = name;
    }
}
