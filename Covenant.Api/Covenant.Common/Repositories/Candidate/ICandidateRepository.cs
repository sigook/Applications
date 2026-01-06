using Covenant.Common.Entities.Candidate;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidate;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Candidate;

public interface ICandidateRepository
{
    Task<Common.Entities.Candidate.Candidate> GetCandidate(Expression<Func<Common.Entities.Candidate.Candidate, bool>> expression);
    Task<bool> CandidateExists(string email);
    Task<PaginatedList<CandidateListModel>> GetCandidates(Guid agencyId, GetCandidatesFilter filter);
    IEnumerable<CandidateListModel> GetAllCandidates(Guid agencyId, GetCandidatesFilter filter);
    Task<CandidateDetailModel> GetCandidateDetail(Guid id);
    Task<PhoneNumberModel> GetPhoneNumberDetail(Guid candidateId, Guid id);
    Task<SkillModel> GetSkillDetail(Guid candidateId, Guid id);
    Task<CandidateDocument> GetDocument(Guid candidateId, Guid id);
    Task<PaginatedList<CovenantFileModel>> GetDocuments(Guid candidateId, Pagination pagination);
    Task<NoteModel> GetNoteDetail(Guid candidateId, Guid id);
    Task<PaginatedList<NoteModel>> GetNotes(Guid candidateId, Pagination pagination);
    Task<CandidateNote> GetNote(Guid candidateId, Guid id);
    Task<bool> CandidatePhonesExists(IEnumerable<string> phone);
    Task SaveChangesAsync();
    Task Update<T>(T entity) where T : class;
    Task Create<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    void Delete<T>(IEnumerable<T> entities) where T : class;
    Task BulkCandidates(IEnumerable<BulkCandidate> bulk);
}