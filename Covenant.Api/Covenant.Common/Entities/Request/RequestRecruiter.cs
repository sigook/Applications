using Covenant.Common.Entities.Agency;

namespace Covenant.Common.Entities.Request
{
    public class RequestRecruiter
    {
        private RequestRecruiter()
        {
        }

        public RequestRecruiter(Guid requestId, Guid recruiterId, DateTime? workDate = null)
        {
            RequestId = requestId;
            RecruiterId = recruiterId;
            WorkDate = workDate?.Date;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Request Request { get; private set; }
        public Guid RequestId { get; private set; }
        public AgencyPersonnel Recruiter { get; private set; }
        public Guid RecruiterId { get; private set; }
        public DateTime? WorkDate { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public ICollection<WorkerDispatch> Dispatches { get; private set; } = new List<WorkerDispatch>();

        public void AddDispatch(Guid workerProfileId, string createdBy = null)
        {
            if (Dispatches.Any(d => d.WorkerProfileId == workerProfileId)) return;
            Dispatches.Add(new WorkerDispatch(Id, workerProfileId, createdBy));
        }

        public void RemoveDispatch(Guid workerProfileId)
        {
            var dispatch = Dispatches.FirstOrDefault(d => d.WorkerProfileId == workerProfileId);
            if (dispatch is not null) Dispatches.Remove(dispatch);
        }

        public void MoveTo(Guid recruiterId, DateTime? workDate)
        {
            RecruiterId = recruiterId;
            WorkDate = workDate?.Date;
        }
    }
}
