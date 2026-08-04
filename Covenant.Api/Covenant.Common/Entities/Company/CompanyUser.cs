using Covenant.Common.Entities.Request;

namespace Covenant.Common.Entities.Company
{

    public class CompanyUser
    {
        private CompanyUser()
        {
        }

        public CompanyUser(Guid companyProfileId, User user)
        {
            CompanyProfileId = companyProfileId;
            User = user ?? throw new ArgumentNullException(nameof(user));
            UserId = user.Id;
            Id = user.Id;
        }

        public Guid Id { get; private set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Position { get; set; }
        public string MobileNumber { get; set; }
        public Guid CompanyProfileId { get; private set; }
        public CompanyProfile CompanyProfile { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public IEnumerable<RequestCompanyUser> RequestCompanyUser { get; set; }
    }
}
