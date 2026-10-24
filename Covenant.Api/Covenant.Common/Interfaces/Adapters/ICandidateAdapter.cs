using Covenant.Common.Entities.Candidates;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Candidates;
using Covenant.Common.Models.Workers;

namespace Covenant.Common.Interfaces.Adapters;

public interface ICandidateAdapter
{
    Result<Candidate> ConvertCandidateModelToCandidate(CandidateCreateModel model, Guid agencyId);

    WorkerProfileCreateModel ConvertCandidateToWorkerProfile(Candidate candidate);

    Task<BulkCandidate> ConvertCandidateCsvToCandidateBulk(CandidateCsvModel model, Guid agencyId, Request request);
}
