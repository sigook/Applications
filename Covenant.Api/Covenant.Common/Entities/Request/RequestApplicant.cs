using Covenant.Common.Entities.Worker;
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

        public static Result<RequestApplicant> CreateWithWorker(Guid requestId, Guid workerProfileId, string createdBy, string comments) =>
            Result.Ok(new RequestApplicant
            {
                RequestId = requestId,
                WorkerProfileId = workerProfileId,
                CandidateId = null,
                CreatedBy = createdBy,
                Comments = comments
            });

        public static Result<RequestApplicant> CreateWithCandidate(Guid requestId, Guid candidateId, string createdBy, string comments) =>
            Result.Ok(new RequestApplicant
            {
                RequestId = requestId,
                CandidateId = candidateId,
                WorkerProfileId = null,
                CreatedBy = createdBy,
                Comments = comments,
            });

        public Result UpdateComments(string comments)
        {
            Comments = comments;
            return Result.Ok();
        }
    }
}