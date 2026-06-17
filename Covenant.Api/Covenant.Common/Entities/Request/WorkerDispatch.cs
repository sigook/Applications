using Covenant.Common.Entities.Worker;

namespace Covenant.Common.Entities.Request
{
    public class WorkerDispatch
    {
        private WorkerDispatch()
        {
        }

        public WorkerDispatch(Guid requestRecruiterId, Guid workerProfileId, string createdBy = null)
        {
            RequestRecruiterId = requestRecruiterId;
            WorkerProfileId = workerProfileId;
            CreatedBy = createdBy;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid RequestRecruiterId { get; private set; }
        public RequestRecruiter RequestRecruiter { get; private set; }
        public Guid WorkerProfileId { get; private set; }
        public WorkerProfile WorkerProfile { get; private set; }
        public string CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
    }
}
