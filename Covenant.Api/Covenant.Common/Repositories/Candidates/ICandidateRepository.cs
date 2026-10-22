using Covenant.Common.Entities.Candidates;
using Covenant.Common.Models.Candidates;
using Covenant.Common.Models;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Candidates;

public interface ICandidateRepository : IBaseRepository
{
    Task<Candidate> GetCandidate(Expression<Func<Candidate, bool>> expression);
    Task<List<CandidateContactInfoModel>> GetCandidatesAvailableToInvite(Guid agencyId, Guid requestId);
    Task<bool> CandidateExists(string email);
    Task<PaginatedList<CandidateListModel>> GetCandidates(Guid agencyId, GetCandidatesFilter filter);
    IQueryable<CandidateListModel> GetAllCandidates(Guid agencyId, GetCandidatesFilter filter);
    Task<CandidateDetailModel> GetCandidateDetail(Guid id);
    Task<PhoneNumberModel> GetPhoneNumberDetail(Guid candidateId, Guid id);
    Task<SkillModel> GetSkillDetail(Guid candidateId, Guid id);
    Task<CandidateDocument> GetDocument(Guid candidateId, Guid id);
    Task<PaginatedList<CovenantFileModel>> GetDocuments(Guid candidateId, Pagination pagination);
    Task<NoteModel> GetNoteDetail(Guid candidateId, Guid id);
    Task<PaginatedList<NoteModel>> GetNotes(Guid candidateId, Pagination pagination);
    Task<CandidateNote> GetNote(Guid candidateId, Guid id);
    Task<bool> CandidatePhonesExists(IEnumerable<string> phone);
    Task BulkCandidates(IEnumerable<BulkCandidate> bulk);
}