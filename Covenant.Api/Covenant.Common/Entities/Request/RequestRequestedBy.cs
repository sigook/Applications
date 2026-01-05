using Covenant.Common.Entities.Company;

namespace Covenant.Common.Entities.Request
{
    public class RequestRequestedBy
    {
        private RequestRequestedBy() 
        { 
        }

        public RequestRequestedBy(Guid requestId, Guid contactPersonId)
        {
            RequestId = requestId;
            ContactPersonId = contactPersonId;
        }

        public Guid RequestId { get; private set; }
        public Request Request { get; private set; }
        public Guid ContactPersonId { get; private set; }
        public CompanyProfileContactPerson ContactPerson { get; private set; }
    }
}