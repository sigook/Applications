using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;

namespace Covenant.Common.Entities.Request
{
    public class RequestApplicant
    {
        public RequestApplicant()
        {
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Request Request { get; set; }
        public Guid RequestId { get; set; }
        public WorkerProfile WorkerProfile { get; set; }
        public Guid? WorkerProfileId { get; set; }
        public Candidate.Candidate Candidate { get; set; }
        public Guid? CandidateId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public string Comments { get; set; }
        public RequestApplicantStatus Status { get; set; } = RequestApplicantStatus.Pending;
        public ICollection<RequestApplicantComplianceItem> ComplianceItems { get; set; } = new List<RequestApplicantComplianceItem>();

        public static Result<RequestApplicant> CreateWithWorker(Guid requestId, Guid workerProfileId, string createdBy, string comments, RequestApplicantStatus status) =>
            Result.Ok(new RequestApplicant
            {
                RequestId = requestId,
                WorkerProfileId = workerProfileId,
                CandidateId = null,
                CreatedBy = createdBy,
                Comments = comments,
                Status = status
            });

        public static Result<RequestApplicant> CreateWithCandidate(Guid requestId, Guid candidateId, string createdBy, string comments, RequestApplicantStatus status) =>
            Result.Ok(new RequestApplicant
            {
                RequestId = requestId,
                CandidateId = candidateId,
                WorkerProfileId = null,
                CreatedBy = createdBy,
                Comments = comments,
                Status = status
            });

        public Result UpdateComments(string comments)
        {
            Comments = comments;
            return Result.Ok();
        }

        public Result MoveToInProgress()
        {
            if (Status is not (RequestApplicantStatus.Pending or RequestApplicantStatus.Cancelled))
                return Result.Fail("Only pending or cancelled applicants can be moved to in progress");
            Status = RequestApplicantStatus.InProgress;
            return Result.Ok();
        }

        public Result Cancel()
        {
            if (Status is not (RequestApplicantStatus.Pending or RequestApplicantStatus.InProgress))
                return Result.Fail("Only pending or in progress applicants can be cancelled");
            Status = RequestApplicantStatus.Cancelled;
            return Result.Ok();
        }

        public Result Confirm()
        {
            if (Status != RequestApplicantStatus.InProgress)
                return Result.Fail("Only in progress applicants can be confirmed");
            if (CandidateId is not null)
                return Result.Fail("A candidate cannot be confirmed, convert it to a worker first");
            Status = RequestApplicantStatus.Confirmed;
            return Result.Ok();
        }
    }
}
