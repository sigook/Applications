using Covenant.Common.Entities.Candidates;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using System.Text.Json.Serialization;

namespace Covenant.Common.Entities.Requests
{
    public class RequestApplicant
    {
        public RequestApplicant()
        {
        }

        [JsonInclude]
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Request Request { get; private set; }
                [JsonInclude]
                public Guid RequestId { get; private set; }
        public WorkerProfile WorkerProfile { get; private set; }
        [JsonInclude]
        public Guid? WorkerProfileId { get; private set; }
        public Candidate Candidate { get; private set; }
        [JsonInclude]
        public Guid? CandidateId { get; private set; }
        [JsonInclude]
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        [JsonInclude]
        public string CreatedBy { get; private set; }
        [JsonInclude]
        public string Comments { get; private set; }
        [JsonInclude]
        public RequestApplicantStatus Status { get; private set; } = RequestApplicantStatus.Pending;
        public ICollection<RequestApplicantComplianceItem> ComplianceItems { get; private set; } = new List<RequestApplicantComplianceItem>();

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

        public void MoveToWorker(Guid workerProfileId)
        {
            CandidateId = null;
            WorkerProfileId = workerProfileId;
        }

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
