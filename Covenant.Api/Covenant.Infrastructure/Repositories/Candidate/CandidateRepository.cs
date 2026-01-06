using Covenant.Common.Configuration;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Mappers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Covenant.Infrastructure.Repositories.Candidate
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly CovenantContext _context;
        private readonly FilesConfiguration filesConfiguration;

        public CandidateRepository(CovenantContext context, IOptions<FilesConfiguration> options)
        {
            _context = context;
            filesConfiguration = options.Value;
        }

        public Task<bool> CandidateExists(string email) => _context.Candidates.AnyAsync(u => u.Email.ToLower().Equals(email.ToLower()));

        public async Task<PaginatedList<CandidateListModel>> GetCandidates(Guid agencyId, GetCandidatesFilter filter)
        {
            var query = GetAllCandidates(agencyId, filter);
            return await query.ToPaginatedList(filter);
        }

        public IEnumerable<CandidateListModel> GetAllCandidates(Guid agencyId, GetCandidatesFilter filter)
        {
            var candidates = _context.Candidates
                .AsNoTracking()
                .Include(c => c.PhoneNumbers)
                .Include(c => c.Skills)
                .Include(c => c.Notes)
                .Include(c => c.RequestApplicants)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter.Skills))
            {
                candidates = candidates.Where(c => c.Skills.Any(s => s.Skill.ToLower().Contains(filter.Skills.ToLower())));
            }
            var query = candidates.Select(c => new CandidateListModel
            {
                AgencyId = c.AgencyId,
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Address = c.Address,
                PostalCode = c.PostalCode,
                Source = c.Source,
                PhoneNumbers = c.PhoneNumbers.Select(p => new PhoneNumberModel { Id = p.Id, PhoneNumber = p.PhoneNumber }),
                Skills = c.Skills.Where(s => !string.IsNullOrWhiteSpace(s.Skill)).OrderBy(s => s.Skill).Select(s => new SkillModel { Id = s.Id, Skill = s.Skill }),
                Requests = c.RequestApplicants.Where(wC => wC.CandidateId == c.Id).OrderByDescending(oC => oC.CreatedAt).Select(sC => new BaseModel<Guid> { Id = sC.Request.Id, Value = sC.Request.NumberId.ToString() }),
                ResidencyStatus = c.ResidencyStatus,
                Recruiter = c.Recruiter,
                CreatedAt = c.CreatedAt,
                HasVehicle = c.HasVehicle,
                HasDocuments = c.Documents.Any(),
                Dnu = c.Dnu,
                NotesCount = c.Notes.Count(n => !n.Note.IsDeleted)
            });
            var predicateNew = ApplyFilterCandidates(agencyId, filter);
            query = query.Where(predicateNew);
            query = ApplySortCandidates(query, filter);
            return query;
        }

        private Expression<Func<CandidateListModel, bool>> ApplyFilterCandidates(Guid agencyId, GetCandidatesFilter filter)
        {
            Expression<Func<CandidateListModel, bool>> predicate = c => c.AgencyId == agencyId;
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                var name = filter.Name.ToLower();
                predicate = predicate.And(c =>
                    c.Name.ToLower().Contains(name) ||
                    c.Email.ToLower().Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(filter.Phone))
            {
                var expression = new Regex(@"\s+");
                var phoneWithoutBlankSpaces = expression.Replace(filter.Phone, string.Empty);
                predicate = predicate.And(c =>
                    c.PhoneNumbers.Any(pn => pn.PhoneNumber.Replace("(", string.Empty).Replace(")", string.Empty).Contains(filter.Phone)) ||
                    c.PhoneNumbers.Any(pn => pn.PhoneNumber.Contains(phoneWithoutBlankSpaces)));
            }
            if (!string.IsNullOrWhiteSpace(filter.Address))
            {
                var address = filter.Address.ToLower();
                predicate = predicate.And(c =>
                    c.Address.ToLower().Contains(address) ||
                    c.PostalCode.ToLower().Contains(address));
            }
            if (filter.NumberId.HasValue)
                predicate = predicate.And(c => c.Requests.Any(r => r.Value == filter.NumberId.Value.ToString()));
            if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
                predicate = predicate.And(c => c.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && c.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
            if (!string.IsNullOrWhiteSpace(filter.Recruiter))
                predicate = predicate.And(c => c.Recruiter.ToLower().Contains(filter.Recruiter.ToLower()));
            if (filter.Statuses != null && filter.Statuses.Any())
                predicate = predicate.And(c => filter.Statuses.Contains(c.ResidencyStatus));
            if (!string.IsNullOrWhiteSpace(filter.Source))
                predicate = predicate.And(c => c.Source.ToLower().Contains(filter.Source.ToLower()));
            if (filter.ResumeOnly)
                predicate = predicate.And(c => c.HasDocuments);
            return predicate;
        }

        private IQueryable<CandidateListModel> ApplySortCandidates(IQueryable<CandidateListModel> query, GetCandidatesFilter filter)
        {
            switch (filter.SortBy)
            {
                case GetCandidatesSortBy.Name:
                    query = query.AddOrderBy(filter, c => c.Name);
                    break;
                case GetCandidatesSortBy.Address:
                    query = query.AddOrderBy(filter, c => c.Address);
                    break;
                case GetCandidatesSortBy.Skills:
                    query = query.AddOrderBy(filter, c => c.Skills.Any() ? c.Skills.FirstOrDefault().Skill : null);
                    break;
                case GetCandidatesSortBy.CreateAt:
                    query = query.AddOrderBy(filter, c => c.CreatedAt);
                    break;
                case GetCandidatesSortBy.Recruiter:
                    query = query.AddOrderBy(filter, c => c.Recruiter);
                    break;
                case GetCandidatesSortBy.Status:
                    query = query.AddOrderBy(filter, c => c.ResidencyStatus);
                    break;
                case GetCandidatesSortBy.Source:
                    query = query.AddOrderBy(filter, c => c.Source);
                    break;
            }
            return query;
        }

        public async Task<CandidateDetailModel> GetCandidateDetail(Guid id) =>
            await _context.Candidates.Where(c => c.Id == id)
                .Select(c => new CandidateDetailModel
                {
                    Id = c.Id,
                    NumberId = c.NumberId,
                    Name = c.Name,
                    Email = c.Email,
                    Address = c.Address,
                    PostalCode = c.PostalCode,
                    Gender = c.Gender == null ? null : new BaseModel<Guid> { Id = c.Gender.Id, Value = c.Gender.Value },
                    HasVehicle = c.HasVehicle,
                    ResidencyStatus = c.ResidencyStatus,
                    Dnu = c.Dnu
                }).SingleOrDefaultAsync();

        public Task<PhoneNumberModel> GetPhoneNumberDetail(Guid candidateId, Guid id) =>
            _context.CandidatePhones.Where(c => c.CandidateId == candidateId && c.Id == id)
                .Select(p => new PhoneNumberModel { Id = p.Id, PhoneNumber = p.PhoneNumber }).SingleOrDefaultAsync();

        public async Task<SkillModel> GetSkillDetail(Guid candidateId, Guid id) =>
            await _context.CandidateSkills.Where(c => c.CandidateId == candidateId && c.Id == id)
                .Select(p => new SkillModel { Id = p.Id, Skill = p.Skill }).SingleOrDefaultAsync();

        public async Task<PaginatedList<CovenantFileModel>> GetDocuments(Guid candidateId, Pagination pagination) =>
            await _context.CandidateDocuments.Where(c => c.CandidateId == candidateId)
                .Select(s => new CovenantFileModel
                {
                    Id = s.DocumentId,
                    FileName = s.Document.FileName,
                    Description = s.Document.Description,
                    PathFile = $"{filesConfiguration.FilesPath}{s.Document.FileName}"
                }).ToPaginatedList(pagination);

        public async Task<NoteModel> GetNoteDetail(Guid candidateId, Guid id) =>
            await _context.CandidateNotes.AsNoTracking()
                .Where(c => c.CandidateId == candidateId && c.NoteId == id)
                .Include(c => c.Note)
                .Select(CandidateExtensionsMapping.SelectNote)
                .SingleOrDefaultAsync();

        public async Task<PaginatedList<NoteModel>> GetNotes(Guid candidateId, Pagination pagination) =>
            await _context.CandidateNotes.AsNoTracking()
                .Where(c => c.CandidateId == candidateId && !c.Note.IsDeleted)
                .Select(CandidateExtensionsMapping.SelectNote).OrderByDescending(c => c.CreatedAt)
                .ToPaginatedList(pagination);

        public Task Update<T>(T entity) where T : class => Task.FromResult(_context.Set<T>().Update(entity));

        public async Task Create<T>(T entity) where T : class => await _context.Set<T>().AddAsync(entity);

        public void Delete<T>(T entity) where T : class => _context.Set<T>().Remove(entity);

        public Task SaveChangesAsync() => _context.SaveChangesAsync();

        public Task<CandidateNote> GetNote(Guid candidateId, Guid id) =>
            _context.CandidateNotes.Where(c => c.CandidateId == candidateId && c.NoteId == id)
                .Include(c => c.Note).SingleOrDefaultAsync();

        public Task<CandidateDocument> GetDocument(Guid candidateId, Guid id) =>
            _context.CandidateDocuments.SingleOrDefaultAsync(c => c.CandidateId == candidateId && c.DocumentId == id);

        public async Task<Common.Entities.Candidate.Candidate> GetCandidate(Expression<Func<Common.Entities.Candidate.Candidate, bool>> expression)
        {
            var candidate = await _context.Candidates
                .Include(c => c.Gender)
                .Include(c => c.PhoneNumbers)
                .Include(c => c.Skills)
                .Include(c => c.Documents).ThenInclude(d => d.Document)
                .Include(c => c.Notes).ThenInclude(n => n.Note)
                .Include(c => c.Gender)
                .FirstOrDefaultAsync(expression);
            return candidate;
        }

        public async Task<bool> CandidatePhonesExists(IEnumerable<string> phone)
        {
            return await _context.CandidatePhones.AnyAsync(cp => phone.Contains(cp.PhoneNumber));
        }

        public async Task BulkCandidates(IEnumerable<BulkCandidate> bulk)
        {
            var candidates = bulk.Select(c => c.Candidate).ToList();
            var candidatePhones = bulk.Select(c => c.CandidatesPhone).ToList();
            var candidateSkills = bulk.SelectMany(c => c.CandidatesSkills).ToList();
            var candidateDocuments = bulk.Select(c => c.CandidatesDocument).ToList();
            var applicatnRequests = bulk.Select(c => c.RequestApplicant).ToList();
            await _context.AddRangeAsync(candidates);
            await _context.AddRangeAsync(candidatePhones);
            if (candidateSkills.Any(cs => cs != null))
            {
                await _context.AddRangeAsync(candidateSkills);
            }
            if (candidateDocuments.Any(cd => cd != null))
            {
                var convenantFiles = candidateDocuments.Select(cd => cd.Document);
                await _context.AddRangeAsync(convenantFiles);
                await _context.AddRangeAsync(candidateDocuments);
            }
            if (applicatnRequests.Any(ar => ar != null))
            {
                await _context.AddRangeAsync(applicatnRequests);
            }
        }

        public void Delete<T>(IEnumerable<T> entities) where T : class => _context.Set<T>().RemoveRange(entities);
    }
}