using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidate;
using Microsoft.AspNetCore.Http;

namespace Covenant.Core.BL.Interfaces
{
    public interface ICandidateService
    {
        Task<Result<Guid>> CreateCandidate(CandidateCreateModel model, Guid agencyId, bool validatePhone = true);
        Task<Result> UpdateCandidate(Guid id, CandidateCreateModel model);
        Task<Result> UpdateRecruiterCandidate(Guid id);
        Task<Result> DeleteCandidate(Guid id);
        Task<Result> ConvertToWorker(Guid id);
        Task<Result<Guid>> CreateCandidateDocument(Guid id, CovenantFileModel model);
        Task<Result<ResultGenerateDocument<byte[]>>> BulkCandidates(Guid agencyId, IFormFile file);
    }
}
