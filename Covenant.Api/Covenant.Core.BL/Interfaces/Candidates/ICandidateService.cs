using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidate;

namespace Covenant.Core.BL.Interfaces.Candidates;

public interface ICandidateService
{
    Task<Result<Guid>> CreateCandidate(CandidateCreateModel model, Guid agencyId, bool validatePhone = true);
    Task<Result<Guid>> CreateCandidateWithResume(Guid agencyId);
    Task<Result> UpdateCandidate(Guid id, CandidateCreateModel model);
    Task<Result> UpdateRecruiterCandidate(Guid id);
    Task<Result> DeleteCandidate(Guid id);
    Task<Result> DeleteCandidateByEmail(string email);
    Task<Result> ConvertToWorker(Guid id);
    Task<Result<Guid>> CreateCandidateDocument(Guid id);
    Task<Result> DeleteCandidateDocument(Guid candidateId, Guid documentId);
    Task<Result<Guid>> AddPhoneNumber(Guid candidateId, string phoneNumber);
    Task<Result> DeletePhoneNumber(Guid candidateId, Guid phoneNumberId);
    Task<Result<SkillModel>> AddSkill(Guid candidateId, string skill);
    Task<Result> DeleteSkill(Guid candidateId, Guid skillId);
    Task<Result<NoteModel>> CreateNote(Guid candidateId, NoteModel model);
    Task<Result> UpdateNote(Guid candidateId, Guid noteId, NoteModel model);
    Task<Result> DeleteNote(Guid candidateId, Guid noteId);
    Task<Result<ResultGenerateDocument<byte[]>>> BulkCandidates(Guid agencyId, Stream file);
}
