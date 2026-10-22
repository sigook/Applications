using Covenant.Common.Entities.Agency;

namespace Covenant.Common.Entities.Request
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
        public ICollection<Runners.Runner> Runners { get; private set; } = new List<Runners.Runner>();

        public void MoveTo(Guid recruiterId, DateTime? workDate)
        {
            RecruiterId = recruiterId;
            WorkDate = workDate?.Date;
        }
    }
}
