using Covenant.Common.Entities.Agency;

namespace Covenant.Common.Entities.Request
{
    public class RequestRecruiter
    {
        private RequestRecruiter() 
        { 
        }

        public RequestRecruiter(Guid requestId, Guid recruiterId)
        {
            RequestId = requestId;
            RecruiterId = recruiterId;
        }

        public Request Request { get; private set; }
        public Guid RequestId { get; private set; }
        public AgencyPersonnel Recruiter { get; private set; }
        public Guid RecruiterId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        
    }
}