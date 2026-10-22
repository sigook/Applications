using Covenant.Common.Entities.Company;

namespace Covenant.Common.Entities.Request
{
    public class RequestCompanyUser
    {
        internal RequestCompanyUser()
        {
        }

        public RequestCompanyUser(Guid requestId, Guid companyUserId)
        {
            RequestId = requestId;
            CompanyUserId = companyUserId;
        }

        public Guid RequestId { get; private set; }
        public Request Request { get; private set; }
        public Guid CompanyUserId { get; private set; }
        public CompanyUser CompanyUser { get; private set; }
    }
}
