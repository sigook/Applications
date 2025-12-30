using Covenant.Common.Entities.Company;

namespace Covenant.Common.Entities.Request
{
    public class RequestCompanyUser
    {
        public Guid RequestId { get; set; }
        public Request Request { get; set; }
        public Guid CompanyUserId { get; set; }
        public CompanyUser CompanyUser { get; set; }
    }
}
