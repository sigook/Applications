using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Models.Worker;

namespace Covenant.Common.Interfaces.Adapters;

public interface ICandidateAdapter
{
    Result<Candidate> ConvertCandidateModelToCandidate(CandidateCreateModel model, Guid agencyId);

    Task<WorkerProfileCreateModel> ConvertCandidateToWorkerProfile(Candidate candidate);

    Task<BulkCandidate> ConvertCandidateCsvToCandidateBulk(CandidateCsvModel model, Guid agencyId, Request request);
}
