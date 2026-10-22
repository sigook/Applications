using Covenant.Common.Configuration;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories.Candidates;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Extensions;
using Covenant.Infrastructure.Mappers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Covenant.Infrastructure.Repositories.Candidates;

public class CandidateRepository(CovenantContext context, IOptions<FilesConfiguration> options) : BaseRepository<CovenantContext>(context), ICandidateRepository
{
    private readonly FilesConfiguration filesConfiguration = options.Value;

    public Task<bool> CandidateExists(string email) => Context.Candidates.AnyAsync(u => u.Email.ToLower().Equals(email.ToLower()));

    public Task<List<CandidateContactInfoModel>> GetCandidatesAvailableToInvite(Guid agencyId, Guid requestId) =>
        Context.Candidates
            .AsNoTracking()
            .Where(c => c.AgencyId == agencyId && !c.Dnu && !c.EmailUnsubscribed)
            .Where(c => !string.IsNullOrEmpty(c.Email) && c.Email.Contains("@"))
            .Where(c => c.CityId.HasValue || !string.IsNullOrEmpty(c.Address))
            .Where(c => !c.RequestApplicants.Any(ra => ra.RequestId == requestId))
            .OrderBy(c => c.NumberId)
            .Select(c => new CandidateContactInfoModel
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Address = c.Address,
                CityId = c.CityId
            })
            .ToListAsync();

    public async Task<PaginatedList<CandidateListModel>> GetCandidates(Guid agencyId, GetCandidatesFilter filter)
    {
        var query = GetAllCandidates(agencyId, filter);
        return await query.ToPaginatedList(filter);
    }

    public IQueryable<CandidateListModel> GetAllCandidates(Guid agencyId, GetCandidatesFilter filter)
    {
        var candidates = Context.Candidates
            .AsNoTracking()
            .Include(c => c.PhoneNumbers)
            .Include(c => c.Skills)
            .Include(c => c.Notes)
            .Include(c => c.RequestApplicants)
            .AsQueryable();
        if (SearchExtensions.HasSearchTokens(filter.Skills))
        {
            var skillMatches = Context.SkillSearchPredicate<CandidateSkill>(filter.Skills, s => s.Skill);
            candidates = candidates.Where(c => c.Skills.AsQueryable().Any(skillMatches));
        }
        candidates = candidates.WhereSearch(filter.Name, c => c.Name, c => c.Email);
        var query = candidates.Select(c => new CandidateListModel
        {
            AgencyId = c.AgencyId,
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Address = c.Address,
            PostalCode = c.PostalCode,
            City = c.City.Value,
            Province = c.City.Province.Code,
            Source = c.Source.Value,
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
        if (!string.IsNullOrWhiteSpace(filter.Phone))
        {
            var expression = new Regex(@"\s+");
            var phoneWithoutBlankSpaces = expression.Replace(filter.Phone, string.Empty);
            predicate = predicate.And(c =>
                c.PhoneNumbers.Any(pn => pn.PhoneNumber.Replace("(", string.Empty).Replace(")", string.Empty).Contains(filter.Phone)) ||
                c.PhoneNumbers.Any(pn => pn.PhoneNumber.Contains(phoneWithoutBlankSpaces)));
        }
        predicate = predicate.AndSearch(filter.Address, c => c.Address, c => c.PostalCode, c => c.City);
        if (filter.NumberId.HasValue)
            predicate = predicate.And(c => c.Requests.Any(r => r.Value == filter.NumberId.Value.ToString()));
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(c => c.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && c.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
        predicate = predicate.AndSearch(filter.Recruiter, c => c.Recruiter);
        if (filter.Statuses != null && filter.Statuses.Any())
            predicate = predicate.And(c => filter.Statuses.Contains(c.ResidencyStatus));
        if (filter.Sources != null && filter.Sources.Any())
            predicate = predicate.And(c => filter.Sources.Contains(c.Source));
        if (filter.ResumeOnly)
            predicate = predicate.And(c => c.HasDocuments);
        return predicate;
    }

    private static IQueryable<CandidateListModel> ApplySortCandidates(IQueryable<CandidateListModel> query, GetCandidatesFilter filter)
    {
        var ordered = filter.SortBy switch
        {
            GetCandidatesSortBy.Name => query.AddOrderBy(filter, c => c.Name),
            GetCandidatesSortBy.Address => query.AddOrderBy(filter, c => c.Address),
            GetCandidatesSortBy.Skills => query.AddOrderBy(filter, c => c.Skills.Any() ? c.Skills.FirstOrDefault().Skill : null),
            GetCandidatesSortBy.CreateAt => query.AddOrderBy(filter, c => c.CreatedAt),
            GetCandidatesSortBy.Recruiter => query.AddOrderBy(filter, c => c.Recruiter),
            GetCandidatesSortBy.Status => query.AddOrderBy(filter, c => c.ResidencyStatus),
            GetCandidatesSortBy.Source => query.AddOrderBy(filter, c => c.Source),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };
        return ordered.ThenBy(c => c.Id);
    }

    public async Task<CandidateDetailModel> GetCandidateDetail(Guid id) =>
        await Context.Candidates.Where(c => c.Id == id)
            .Select(c => new CandidateDetailModel
            {
                Id = c.Id,
                NumberId = c.NumberId,
                Name = c.Name,
                Email = c.Email,
                Address = c.Address,
                PostalCode = c.PostalCode,
                City = c.City == null ? null : new CityModel
                {
                    Id = c.City.Id,
                    Value = c.City.Value,
                    Code = c.City.Code,
                    Province = new ProvinceModel
                    {
                        Id = c.City.Province.Id,
                        Value = c.City.Province.Value,
                        Code = c.City.Province.Code,
                        Country = new CountryModel
                        {
                            Id = c.City.Province.Country.Id,
                            Value = c.City.Province.Country.Value,
                            Code = c.City.Province.Country.Code
                        }
                    }
                },
                Gender = c.Gender == null ? null : new BaseModel<Guid> { Id = c.Gender.Id, Value = c.Gender.Value },
                HasVehicle = c.HasVehicle,
                ResidencyStatus = c.ResidencyStatus,
                SourceId = c.SourceId,
                Dnu = c.Dnu,
                PhoneNumbers = c.PhoneNumbers.Select(p => new PhoneNumberModel { Id = p.Id, PhoneNumber = p.PhoneNumber })
            }).SingleOrDefaultAsync();

    public Task<PhoneNumberModel> GetPhoneNumberDetail(Guid candidateId, Guid id) =>
        Context.CandidatePhones.Where(c => c.CandidateId == candidateId && c.Id == id)
            .Select(p => new PhoneNumberModel { Id = p.Id, PhoneNumber = p.PhoneNumber }).SingleOrDefaultAsync();

    public async Task<SkillModel> GetSkillDetail(Guid candidateId, Guid id) =>
        await Context.CandidateSkills.Where(c => c.CandidateId == candidateId && c.Id == id)
            .Select(p => new SkillModel { Id = p.Id, Skill = p.Skill }).SingleOrDefaultAsync();

    public async Task<PaginatedList<CovenantFileModel>> GetDocuments(Guid candidateId, Pagination pagination) =>
        await Context.CandidateDocuments.Where(c => c.CandidateId == candidateId)
            .Select(s => new CovenantFileModel
            {
                Id = s.DocumentId,
                FileName = s.Document.FileName,
                Description = s.Document.Description,
                PathFile = $"{filesConfiguration.FilesPath}{s.Document.FileName}"
            }).OrderBy(d => d.FileName).ToPaginatedList(pagination);

    public async Task<NoteModel> GetNoteDetail(Guid candidateId, Guid id) =>
        await Context.CandidateNotes.AsNoTracking()
            .Where(c => c.CandidateId == candidateId && c.NoteId == id)
            .Include(c => c.Note)
            .Select(CandidateExtensionsMapping.SelectNote)
            .SingleOrDefaultAsync();

    public async Task<PaginatedList<NoteModel>> GetNotes(Guid candidateId, Pagination pagination) =>
        await Context.CandidateNotes.AsNoTracking()
            .Where(c => c.CandidateId == candidateId && !c.Note.IsDeleted)
            .Select(CandidateExtensionsMapping.SelectNote).OrderByDescending(c => c.CreatedAt)
            .ToPaginatedList(pagination);

    public Task<CandidateNote> GetNote(Guid candidateId, Guid id) =>
        Context.CandidateNotes.Where(c => c.CandidateId == candidateId && c.NoteId == id)
            .Include(c => c.Note).SingleOrDefaultAsync();

    public Task<CandidateDocument> GetDocument(Guid candidateId, Guid id) =>
        Context.CandidateDocuments.SingleOrDefaultAsync(c => c.CandidateId == candidateId && c.DocumentId == id);

    public async Task<Common.Entities.Candidate.Candidate> GetCandidate(Expression<Func<Common.Entities.Candidate.Candidate, bool>> expression)
    {
        var candidate = await Context.Candidates
            .Include(c => c.Gender)
            .Include(c => c.PhoneNumbers)
            .Include(c => c.Skills)
            .Include(c => c.Documents).ThenInclude(d => d.Document)
            .Include(c => c.Notes).ThenInclude(n => n.Note)
            .Include(c => c.City).ThenInclude(c => c.Province).ThenInclude(p => p.Country)
            .FirstOrDefaultAsync(expression);
        return candidate;
    }

    public async Task<bool> CandidatePhonesExists(IEnumerable<string> phone)
    {
        return await Context.CandidatePhones.AnyAsync(cp => phone.Contains(cp.PhoneNumber));
    }

    public async Task BulkCandidates(IEnumerable<BulkCandidate> bulk)
    {
        var candidates = bulk.Select(c => c.Candidate).ToList();
        var candidatePhones = bulk.Select(c => c.CandidatesPhone).ToList();
        var candidateSkills = bulk.SelectMany(c => c.CandidatesSkills).ToList();
        var candidateDocuments = bulk.Select(c => c.CandidatesDocument).ToList();
        var applicatnRequests = bulk.Select(c => c.RequestApplicant).ToList();
        await Context.AddRangeAsync(candidates);
        await Context.AddRangeAsync(candidatePhones);
        if (candidateSkills.Any(cs => cs != null))
        {
            await Context.AddRangeAsync(candidateSkills);
        }
        if (candidateDocuments.Any(cd => cd != null))
        {
            var convenantFiles = candidateDocuments.Select(cd => cd.Document);
            await Context.AddRangeAsync(convenantFiles);
            await Context.AddRangeAsync(candidateDocuments);
        }
        if (applicatnRequests.Any(ar => ar != null))
        {
            await Context.AddRangeAsync(applicatnRequests);
        }
    }

}
