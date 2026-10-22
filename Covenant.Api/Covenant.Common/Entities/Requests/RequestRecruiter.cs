using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Requests.Runners;

namespace Covenant.Common.Entities.Requests
{
    public class RequestRecruiter
    {
        private RequestRecruiter()
        {
        }

        public RequestRecruiter(Guid requestId, Guid recruiterId, DateTime createdAt, DateTime? workDate = null)
        {
            RequestId = requestId;
            RecruiterId = recruiterId;
            WorkDate = workDate?.Date;
            CreatedAt = createdAt;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Request Request { get; private set; }
        public Guid RequestId { get; private set; }
        public AgencyPersonnel Recruiter { get; private set; }
        public Guid RecruiterId { get; private set; }
        public DateTime? WorkDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public ICollection<Runner> Runners { get; private set; } = new List<Runner>();

        public void MoveTo(Guid recruiterId, DateTime? workDate)
        {
            RecruiterId = recruiterId;
            WorkDate = workDate?.Date;
        }
    }
}
