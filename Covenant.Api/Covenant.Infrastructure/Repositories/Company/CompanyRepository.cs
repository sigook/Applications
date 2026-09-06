using Covenant.Common.Configuration;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Mappers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Company;

public class CompanyRepository : ICompanyRepository
{
    private readonly CovenantContext _context;
    private readonly FilesConfiguration filesConfiguration;

    public CompanyRepository(CovenantContext context, IOptions<FilesConfiguration> options)
    {
        _context = context;
        filesConfiguration = options.Value;
    }

    public async Task Create<T>(T entity) where T : class => await _context.Set<T>().AddAsync(entity);

    public void Delete<T>(T entity) where T : class => _context.Set<T>().Remove(entity);

    public void Update<T>(T entity) where T : class => _context.Set<T>().Update(entity);

    public Task<CompanyProfileIdsModel> GetCompanyProfileId(Expression<Func<CompanyProfile, bool>> condition) =>
        _context.CompanyProfiles.Where(condition)
            .Select(p => new CompanyProfileIdsModel { Id = p.Id, CompanyId = p.CompanyId })
            .SingleOrDefaultAsync();

    public async Task<IEnumerable<LocationDetailModel>> GetCompanyLocations(Expression<Func<CompanyProfileLocation, bool>> condition)
    {
        var locations = await _context.CompanyProfileLocations
            .Include(cpl => cpl.Location)
            .ThenInclude(l => l.City)
            .ThenInclude(c => c.Province)
            .ThenInclude(p => p.Country)
            .Include(cpl => cpl.CompanyProfile)
            .Where(condition)
            .Select(CompanyExtensionsMapping.SelectCompanyProfileLocationDetail)
            .ToListAsync();
        return locations;
    }

    public Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Guid companyProfileId, GetJobPositionsFilter filter)
    {
        var predicate = PredicateBuilder.New<CompanyProfileJobPositionRate>(cpjpr =>
            cpjpr.CompanyProfileId == companyProfileId && !cpjpr.IsDeleted);
        if (!string.IsNullOrWhiteSpace(filter.Role))
        {
            var role = filter.Role.ToLower();
            predicate = predicate.And(cpjpr => cpjpr.JobPosition.ToLower().Contains(role));
        }
        return GetJobPositions(predicate);
    }

    public async Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Expression<Func<CompanyProfileJobPositionRate, bool>> expression)
    {
        var query = _context.CompanyProfileJobPositionRates.Where(expression)
            .Select(s => new CompanyProfileJobPositionRateModel
            {
                Id = s.Id,
                Rate = s.Rate,
                WorkerRate = s.WorkerRate,
                WorkerRateMin = s.WorkerRateMin,
                WorkerRateMax = s.WorkerRateMax,
                OvertimeStartsAfter = s.OvertimeStartsAfter == null ? null : (double?)s.OvertimeStartsAfter.GetValueOrDefault().TotalHours,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                CreatedBy = s.CreatedBy,
                JobPosition = s.JobPosition,
                DisplayShift = s.Shift == null ? null : s.Shift.DisplayShift,
                Shift = s.Shift == null ? null : new ShiftModel
                {
                    Sunday = s.Shift.Sunday,
                    SundayStart = s.Shift.SundayStart,
                    SundayFinish = s.Shift.SundayFinish,
                    Monday = s.Shift.Monday,
                    MondayStart = s.Shift.MondayStart,
                    MondayFinish = s.Shift.MondayFinish,
                    Tuesday = s.Shift.Tuesday,
                    TuesdayStart = s.Shift.TuesdayStart,
                    TuesdayFinish = s.Shift.TuesdayFinish,
                    Wednesday = s.Shift.Wednesday,
                    WednesdayStart = s.Shift.WednesdayStart,
                    WednesdayFinish = s.Shift.WednesdayFinish,
                    Thursday = s.Shift.Thursday,
                    ThursdayStart = s.Shift.ThursdayStart,
                    ThursdayFinish = s.Shift.ThursdayFinish,
                    Friday = s.Shift.Friday,
                    FridayStart = s.Shift.FridayStart,
                    FridayFinish = s.Shift.FridayFinish,
                    Saturday = s.Shift.Saturday,
                    SaturdayStart = s.Shift.SaturdayStart,
                    SaturdayFinish = s.Shift.SaturdayFinish,
                    Comments = s.Shift.Comments
                }
            });
        var result = await query.OrderBy(c => c.JobPosition).ToListAsync();
        return result;
    }

    public async Task<CompanyProfileJobPositionRate> GetJobPosition(Guid id) => await _context.CompanyProfileJobPositionRates.FirstOrDefaultAsync(cpj => cpj.Id == id);

    public Task<CompanyProfileJobPositionRateModel> GetJobPositionDetail(Guid id) =>
        _context.CompanyProfileJobPositionRates.Where(c => c.Id == id)
            .Select(s => new CompanyProfileJobPositionRateModel
            {
                Id = s.Id,
                Rate = s.Rate,
                WorkerRate = s.WorkerRate,
                WorkerRateMin = s.WorkerRateMin,
                WorkerRateMax = s.WorkerRateMax,
                OvertimeStartsAfter = s.OvertimeStartsAfter == null ? null : (double?)s.OvertimeStartsAfter.GetValueOrDefault().TotalHours,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                CreatedBy = s.CreatedBy,
                JobPosition = s.JobPosition,
                Shift = s.Shift == null ? null : new ShiftModel
                {
                    Sunday = s.Shift.Sunday,
                    SundayStart = s.Shift.SundayStart,
                    SundayFinish = s.Shift.SundayFinish,
                    Monday = s.Shift.Monday,
                    MondayStart = s.Shift.MondayStart,
                    MondayFinish = s.Shift.MondayFinish,
                    Tuesday = s.Shift.Tuesday,
                    TuesdayStart = s.Shift.TuesdayStart,
                    TuesdayFinish = s.Shift.TuesdayFinish,
                    Wednesday = s.Shift.Wednesday,
                    WednesdayStart = s.Shift.WednesdayStart,
                    WednesdayFinish = s.Shift.WednesdayFinish,
                    Thursday = s.Shift.Thursday,
                    ThursdayStart = s.Shift.ThursdayStart,
                    ThursdayFinish = s.Shift.ThursdayFinish,
                    Friday = s.Shift.Friday,
                    FridayStart = s.Shift.FridayStart,
                    FridayFinish = s.Shift.FridayFinish,
                    Saturday = s.Shift.Saturday,
                    SaturdayStart = s.Shift.SaturdayStart,
                    SaturdayFinish = s.Shift.SaturdayFinish,
                    Comments = s.Shift.Comments
                }
            }).SingleOrDefaultAsync();

    public async Task<CompanyProfile> GetCompanyProfile(Expression<Func<CompanyProfile, bool>> expression)
    {
        var profile = await _context.CompanyProfiles.Where(expression)
            .Include(c => c.Agency).ThenInclude(a => a.User)
            .Include(c => c.Company)
            .Include(c => c.Logo)
            .Include(c => c.Locations).ThenInclude(l => l.Location).ThenInclude(c => c.City).ThenInclude(c => c.Province).ThenInclude(p => p.Country)
            .Include(c => c.ContactPeople)
            .Include(c => c.Industry).ThenInclude(i => i.Industry)
            .SingleOrDefaultAsync();
        if (profile is null) return null;
        var positionRates = await _context.CompanyProfileJobPositionRates
            .Where(c => !c.IsDeleted && c.CompanyProfileId == profile.Id)
            .ToListAsync();
        profile.JobPositionRates = positionRates;
        return profile;
    }

    public async Task<PaginatedList<CompanyProfileListModel>> GetCompaniesProfileForAgency(Guid agencyId, GetCompanyForAgencyFilter filter)
    {
        var query = GetAllCompaniesProfileForAgency(agencyId, filter);
        return await query.ToPaginatedList(filter);
    }

    public IQueryable<CompanyProfileListModel> GetAllCompaniesProfileForAgency(Guid agencyId, GetCompanyForAgencyFilter filter)
    {
        var companies = _context.CompanyProfiles
            .Include(cp => cp.Locations)
            .Include(cp => cp.Industry).ThenInclude(i => i.Industry)
            .Include(cp => cp.Company)
            .Include(cp => cp.Notes)
            .AsQueryable();
        if (filter.SalesPersonnelId.HasValue)
            companies = companies.Where(cp => cp.SalesRepresentativeId == filter.SalesPersonnelId.Value);
        var query = from cp in companies
                    from cpcp in _context.CompanyProfileContactPeople.Where(cpcp => cpcp.CompanyProfileId == cp.Id).Take(1).DefaultIfEmpty()
                    orderby cp.FullName
                    select new CompanyProfileListModel
                    {
                        AgencyId = cp.AgencyId,
                        Id = cp.Id,
                        CompanyId = cp.CompanyId,
                        FullName = cp.FullName,
                        NumberId = cp.NumberId,
                        Active = cp.Active,
                        Locations = cp.Locations
                            .Select(c => c.Location.Address + " " + c.Location.City.Value + " " + c.Location.City.Province.Code + " " + c.Location.PostalCode),
                        Industry = cp.Industry.IndustryId.HasValue ? cp.Industry.Industry.Value : cp.Industry.OtherIndustry,
                        CompanyStatus = cp.CompanyStatus,
                        ContactName = cpcp == null ? null : cpcp.FirstName + " " + cpcp.MiddleName + " " + cpcp.LastName,
                        ContactRole = cpcp == null ? null : cpcp.Position,
                        Phone = cp.Phone,
                        Email = cp.Company.Email,
                        Website = cp.Website,
                        CreatedBy = cp.CreatedBy,
                        CreatedAt = cp.CreatedAt,
                        UpdatedBy = cp.UpdatedBy,
                        UpdatedAt = cp.UpdatedAt,
                        NotesCount = cp.Notes.Count(n => !n.Note.IsDeleted),
                        SalesRepresentative = cp.SalesRepresentative.Name
                    };
        var predicateNew = ApplyFilterCompanyProfiles(agencyId, filter);
        query = query.Where(predicateNew);
        query = ApplySortCompanyProfiles(query, filter);
        return query;
    }

    public async Task<List<CompanyProfileWithDetailsModel>> GetCompaniesWithDetailsForAgency(Guid agencyId, GetCompanyForAgencyFilter filter)
    {
        var companyList = GetAllCompaniesProfileForAgency(agencyId, filter).ToList();
        var profileIds = companyList.Select(c => c.Id).ToList();

        var contacts = await _context.CompanyProfileContactPeople
            .Where(cp => profileIds.Contains(cp.CompanyProfileId))
            .Select(cp => new CompanyProfileContactPersonModel
            {
                Id = cp.Id,
                CompanyProfileId = cp.CompanyProfileId,
                Title = cp.Title,
                FirstName = cp.FirstName,
                MiddleName = cp.MiddleName,
                LastName = cp.LastName,
                Position = cp.Position,
                Email = cp.Email,
                MobileNumber = cp.MobileNumber,
                OfficeNumber = cp.OfficeNumber,
                OfficeNumberExt = cp.OfficeNumberExt
            }).ToListAsync();

        var jobPositions = await _context.CompanyProfileJobPositionRates
            .Where(jp => profileIds.Contains(jp.CompanyProfileId) && !jp.IsDeleted)
            .Select(jp => new CompanyProfileJobPositionRateModel
            {
                Id = jp.Id,
                CompanyProfileId = jp.CompanyProfileId,
                Rate = jp.Rate,
                WorkerRate = jp.WorkerRate,
                WorkerRateMin = jp.WorkerRateMin,
                WorkerRateMax = jp.WorkerRateMax,
                Description = jp.Description,
                CreatedAt = jp.CreatedAt,
                CreatedBy = jp.CreatedBy,
                JobPosition = jp.JobPosition
            }).ToListAsync();

        var users = await _context.CompanyUsers
            .Where(cu => profileIds.Contains(cu.CompanyProfileId))
            .Select(cu => new CompanyUserModel
            {
                Id = cu.Id,
                CompanyProfileId = cu.CompanyProfileId,
                Name = cu.Name,
                Lastname = cu.Lastname,
                Position = cu.Position,
                MobileNumber = cu.MobileNumber,
                Email = cu.User.Email,
                CreatedAt = cu.CreatedAt
            }).ToListAsync();

        var result = companyList.Select(c => new CompanyProfileWithDetailsModel
        {
            Company = c,
            Contacts = contacts.Where(cp => cp.CompanyProfileId == c.Id),
            JobPositions = jobPositions.Where(jp => jp.CompanyProfileId == c.Id),
            Users = users.Where(u => u.CompanyProfileId == c.Id)
        }).ToList();
        return result;
    }

    private Expression<Func<CompanyProfileListModel, bool>> ApplyFilterCompanyProfiles(Guid agencyId, GetCompanyForAgencyFilter filter)
    {
        Expression<Func<CompanyProfileListModel, bool>> predicate = c => c.AgencyId == agencyId;
        if (!string.IsNullOrWhiteSpace(filter.BusinessInfo))
        {
            var businessInfo = filter.BusinessInfo.ToLower();
            predicate = predicate.And(c =>
                c.FullName.ToLower().Contains(businessInfo) ||
                c.Locations.Any(l => l.Replace(" ", string.Empty).ToLower().Contains(businessInfo)));
        }
        if (!string.IsNullOrWhiteSpace(filter.ContactInfo))
        {
            var contactInfo = filter.ContactInfo.ToLower();
            predicate = predicate.And(c =>
                c.Email.ToLower().Contains(contactInfo) ||
                c.ContactName.ToLower().Contains(contactInfo) ||
                c.ContactRole.ToLower().Contains(contactInfo) ||
                c.Website.ToLower().Contains(contactInfo));
        }
        if (!string.IsNullOrWhiteSpace(filter.Industry))
            predicate = predicate.And(c => c.Industry.ToLower().Contains(filter.Industry.ToLower()));
        if (!string.IsNullOrWhiteSpace(filter.CreatedBy))
            predicate = predicate.And(c => c.CreatedBy.ToLower().Contains(filter.CreatedBy.ToLower()));
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(c => c.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && c.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
        if (!string.IsNullOrWhiteSpace(filter.UpdatedBy))
            predicate = predicate.And(c => c.UpdatedBy.ToLower().Contains(filter.UpdatedBy.ToLower()));
        if (filter.UpdatedAtFrom.HasValue && filter.UpdatedAtTo.HasValue)
            predicate = predicate.And(c =>
                (c.UpdatedAt.HasValue ? c.UpdatedAt.Value.Date : c.UpdatedAt) >= filter.UpdatedAtFrom.Value.Date &&
                (c.UpdatedAt.HasValue ? c.UpdatedAt.Value.Date : c.UpdatedAt) <= filter.UpdatedAtTo.Value.Date);
        if (filter.CompanyStatuses != null && filter.CompanyStatuses.Any())
            predicate = predicate.And(c => filter.CompanyStatuses.Contains(c.CompanyStatus));
        if (!string.IsNullOrWhiteSpace(filter.SalesRepresentative))
            predicate = predicate.And(c => c.SalesRepresentative.ToLower().Contains(filter.SalesRepresentative.ToLower()));
        return predicate;
    }

    private IQueryable<CompanyProfileListModel> ApplySortCompanyProfiles(IQueryable<CompanyProfileListModel> query, GetCompanyForAgencyFilter filter)
    {
        switch (filter.SortBy)
        {
            case GetCompanyForAgencySortBy.Name:
                query = query.AddOrderBy(filter, o => o.FullName);
                break;
            case GetCompanyForAgencySortBy.Industry:
                query = query.AddOrderBy(filter, o => o.Industry);
                break;
            case GetCompanyForAgencySortBy.CreatedAt:
                if (!filter.IsDescending)
                    query = query.AddOrderBy(filter, o => o.CreatedAt).ThenBy(o => o.CreatedBy);
                else
                    query = query.AddOrderBy(filter, o => o.CreatedAt).ThenByDescending(o => o.CreatedBy);
                break;
            case GetCompanyForAgencySortBy.UpdatedAt:
                if (!filter.IsDescending)
                    query = query.AddOrderBy(filter, o => o.UpdatedAt).ThenBy(o => o.UpdatedBy);
                else
                    query = query.AddOrderBy(filter, o => o.UpdatedAt).ThenByDescending(o => o.UpdatedBy);
                break;
            case GetCompanyForAgencySortBy.SalesRepresentative:
                query = query.AddOrderBy(filter, o => o.SalesRepresentative);
                break;
        }
        return query;
    }

    public async Task<CompanyProfileDetailModel> GetCompanyProfileDetail(Expression<Func<CompanyProfile, bool>> expression)
    {
        var query = _context.CompanyProfiles
            .Include(cp => cp.Locations)
            .Include(cp => cp.ContactPeople)
            .Include(cp => cp.JobPositionRates)
            .Include(cp => cp.Industry).ThenInclude(i => i.Industry)
            .Include(cp => cp.Logo)
            .Where(expression)
            .Select(cp => new CompanyProfileDetailModel
            {
                Id = cp.Id,
                NumberId = cp.NumberId,
                CompanyId = cp.CompanyId,
                FullName = cp.FullName,
                Phone = cp.Phone,
                PhoneExt = cp.PhoneExt,
                Fax = cp.Fax,
                FaxExt = cp.FaxExt,
                Email = cp.Company.Email,
                Website = cp.Website,
                About = cp.About,
                InternalInfo = cp.InternalInfo,
                CompanyStatus = cp.CompanyStatus,
                Active = cp.Active,
                PaidHolidays = cp.PaidHolidays,
                RequiredPaymentMethod = cp.RequiredPaymentMethod,
                CreatedAt = cp.CreatedAt,
                VaccinationRequired = cp.VaccinationRequired,
                VaccinationRequiredComments = cp.VaccinationRequiredComments,
                Logo = cp.Logo == null
                    ? null
                    : new CovenantFileModel
                    {
                        Id = cp.Logo.Id,
                        Description = cp.Logo.Description,
                        FileName = cp.Logo.FileName,
                        PathFile = $"{filesConfiguration.FilesPath}{cp.Logo.FileName}"
                    },
                Industry = cp.Industry == null ? null : new CompanyProfileIndustryDetailModel
                {
                    Id = cp.Industry.Id,
                    Industry = cp.Industry.Industry == null ? null : new BaseModel<Guid> { Id = cp.Industry.Industry.Id, Value = cp.Industry.Industry.Value },
                    OtherIndustry = cp.Industry.OtherIndustry
                },
                RequiresPermissionToSeeRequests = cp.RequiresPermissionToSeeRequests,
                SalesRepresentativeId = cp.SalesRepresentativeId,
                OvertimeStartsAfter = cp.OvertimeStartsAfter.TotalHours
            });
        return await query.FirstOrDefaultAsync();
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public Task<Guid> GetCompanyIdForUser(Guid userId) =>
        _context.CompanyUsers.Where(w => w.UserId == userId)
            .Select(s => s.CompanyProfile.CompanyId).SingleOrDefaultAsync();

    public Task<CompanyUserModel> GetCompanyUserDetail(Guid id) =>
        _context.CompanyUsers.Where(uw => uw.UserId == id)
            .Select(CompanyExtensionsMapping.SelectCompanyUser).SingleOrDefaultAsync();

    public Task<CompanyUser> GetCompanyUser(Guid id) =>
        _context.CompanyUsers.Where(uw => uw.UserId == id)
            .Include(u => u.User)
            .SingleOrDefaultAsync();

    public async Task<IEnumerable<CompanyProfileContactPersonModel>> GetContactPeople(Expression<Func<CompanyProfileContactPerson, bool>> condition)
    {
        var query = await _context.CompanyProfileContactPeople
            .Where(condition)
            .Select(CompanyExtensionsMapping.SelectContactPerson)
            .ToListAsync();
        return query;
    }

    public Task<CompanyProfileContactPersonModel> GetContactPersonDetail(Guid profileId, Guid id) =>
        _context.CompanyProfileContactPeople.Where(c => c.CompanyProfileId == profileId && c.Id == id)
            .Select(CompanyExtensionsMapping.SelectContactPerson).SingleOrDefaultAsync();

    public Task<CompanyProfileContactPerson> GetContactPerson(Guid id) => _context.CompanyProfileContactPeople.SingleOrDefaultAsync(c => c.Id == id);

    public Task<CompanyProfileLocationDetailModel> GetLocationDetail(Guid id) =>
        _context.CompanyProfileLocations.Where(c => c.LocationId == id)
            .Select(l => new CompanyProfileLocationDetailModel
            {
                Id = l.LocationId,
                Address = l.Location.Address,
                PostalCode = l.Location.PostalCode,
                City = new CityModel(l.Location.City.Id, l.Location.City.Value),
                Province = new ProvinceModel
                {
                    Id = l.Location.City.ProvinceId,
                    Value = l.Location.City.Province.Value,
                    Code = l.Location.City.Province.Code,
                    Country = new CountryModel { Id = l.Location.City.Province.Country.Id, Value = l.Location.City.Province.Country.Value, Code = l.Location.City.Province.Country.Code }
                },
                IsBilling = l.IsBilling,
                Entrance = l.Location.Entrance,
                MainIntersection = l.Location.MainIntersection
            }).SingleOrDefaultAsync();

    public Task<CompanyProfileLocation> GetLocation(Guid id) =>
        _context.CompanyProfileLocations.Where(c => c.LocationId == id)
            .Include(c => c.Location)
            .Select(l => l).SingleOrDefaultAsync();

    public Task<PaginatedList<CompanyProfileDocumentModel>> GetDocuments(Guid profileId, Pagination pagination) =>
        _context.CompanyProfileDocuments.Where(c => c.CompanyProfileId == profileId)
            .Select(s => new CompanyProfileDocumentModel
            {
                Id = s.DocumentId,
                FileName = s.Document.FileName,
                Description = s.Document.Description,
                PathFile = $"{filesConfiguration.FilesPath}{s.Document.FileName}",
                DocumentType = s.DocumentType,
                CanDownload = true
            }).ToPaginatedList(pagination);

    public Task<CompanyProfileDocument> GetDocument(Guid id) => _context.CompanyProfileDocuments.SingleOrDefaultAsync(c => c.DocumentId == id);

    public Task<PaginatedList<NoteModel>> GetNotes(Guid profileId, Pagination pagination) =>
        _context.CompanyProfileNotes.Where(w => w.CompanyProfileId == profileId && !w.Note.IsDeleted)
            .Select(CompanyExtensionsMapping.SelectNote).OrderByDescending(c => c.CreatedAt).ToPaginatedList(pagination);

    public Task<NoteModel> GetNoteDetail(Guid profileId, Guid id) =>
        _context.CompanyProfileNotes.Where(w => w.CompanyProfileId == profileId && w.NoteId == id)
            .Select(CompanyExtensionsMapping.SelectNote).SingleOrDefaultAsync();

    public Task<CompanyProfileNote> GetNote(Guid profileId, Guid id) =>
        _context.CompanyProfileNotes.Where(c => c.CompanyProfileId == profileId && c.NoteId == id)
            .Include(c => c.Note).SingleOrDefaultAsync();

    public Task<string> GetCompanyProfileInvoiceNotes(Guid companyProfileId) =>
        _context.CompanyProfileInvoiceNotes.Where(n => n.CompanyProfileId == companyProfileId)
            .Select(n => n.HtmlNotes)
            .AsNoTracking()
            .SingleOrDefaultAsync();

    public async Task UpdateCompanyProfileInvoiceNotes(Guid companyProfileId, string htmlNotes)
    {
        CompanyProfileInvoiceNotes notes = await _context.CompanyProfileInvoiceNotes.Where(n => n.CompanyProfileId == companyProfileId)
            .SingleOrDefaultAsync();
        if (notes is null)
        {
            await _context.CompanyProfileInvoiceNotes.AddAsync(new CompanyProfileInvoiceNotes(companyProfileId, htmlNotes));
        }
        else
        {
            notes.ChangeHtmlNotes(htmlNotes);
            _context.CompanyProfileInvoiceNotes.Update(notes);
        }
        await _context.SaveChangesAsync();
    }

    public Task<List<CompanyProfileInvoiceRecipientModel>> GetInvoiceRecipients(Guid companyProfileId) =>
        _context.CompanyProfileInvoiceRecipients.Where(r => r.CompanyProfileId == companyProfileId)
            .Select(r => new CompanyProfileInvoiceRecipientModel { Id = r.Id, Email = r.Email, Name = r.Name })
            .ToListAsync();
    public async Task<CompanyProfileInvoiceRecipient> GetInvoiceRecipient(Guid id)
    {
        return await _context.CompanyProfileInvoiceRecipients.FirstOrDefaultAsync(cpir => cpir.Id == id);
    }

    public async Task UpdateInvoiceRecipient(Guid id, CompanyProfileInvoiceRecipientModel model)
    {
        var entity = await _context.CompanyProfileInvoiceRecipients.FirstOrDefaultAsync(r => r.Id == id);
        if (entity is null) return;
        entity.Name = model.Name;
        entity.UpdateEmail(model.Email);
        _context.CompanyProfileInvoiceRecipients.Update(entity);
    }

    public async Task<Guid> CreateInvoiceRecipient(Guid companyProfileId, CompanyProfileInvoiceRecipientModel model)
    {
        var entity = new CompanyProfileInvoiceRecipient(companyProfileId, model.Email) { Name = model.Name };
        await Create(entity);
        return entity.Id;
    }

    public async Task<IEnumerable<CompanyUserModel>> GetAllCompanyUsers(Guid companyProfileId)
    {
        var companyUsers = await _context.CompanyUsers
            .Where(cu => cu.CompanyProfileId == companyProfileId)
            .OrderBy(cu => cu.Name).ThenBy(cu => cu.Lastname)
            .Select(CompanyExtensionsMapping.SelectCompanyUser).ToListAsync();
        return companyUsers;
    }

    public async Task BulkCompanies(IEnumerable<BulkCompany> bulk)
    {
        var companies = bulk.Select(c => c.CompanyProfile).ToList();
        var companyPhones = bulk.Select(c => c.ContactPerson).ToList();
        var companyLocations = bulk.Select(c => c.CompanyLocation).ToList();

        await _context.AddRangeAsync(companies);
        await _context.AddRangeAsync(companyPhones);
        await _context.AddRangeAsync(companyLocations);

    }

    public async Task<PaginatedList<DealListModel>> GetDeals(Guid agencyId, GetDealsFilter filter)
    {
        var query = _context.Deals
            .Where(d => d.CompanyProfile.AgencyId == agencyId)
            .Select(d => new DealListModel
            {
                Id = d.Id,
                Title = d.Title,
                CompanyProfileId = d.CompanyProfileId,
                CompanyName = d.CompanyProfile.FullName,
                OwnerId = d.UserId,
                Owner = d.User.Email,
                Date = d.Date,
                Value = d.Value,
                Type = d.Type,
                Status = d.Status,
                DocumentId = d.DocumentId,
                DocumentName = d.Document.FileName,
                DocumentPath = d.DocumentId == null ? null : filesConfiguration.FilesPath + d.Document.FileName,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            });
        query = query.Where(ApplyFilterDeals(filter));
        query = ApplySortDeals(query, filter);
        return await query.ToPaginatedList(filter);
    }

    public Task<Deal> GetDeal(Expression<Func<Deal, bool>> expression) =>
        _context.Deals.FirstOrDefaultAsync(expression);

    public async Task<PaginatedList<CompanyInteractionListModel>> GetInteractions(Guid agencyId, GetCompanyInteractionsFilter filter)
    {
        var query = _context.CompanyInteractions
            .Where(i => i.CompanyProfile.AgencyId == agencyId)
            .Select(i => new CompanyInteractionListModel
            {
                Id = i.Id,
                CompanyProfileId = i.CompanyProfileId,
                CompanyName = i.CompanyProfile.FullName,
                OwnerId = i.UserId,
                Owner = i.User.Email,
                Description = i.Description,
                InteractionPurpose = i.InteractionPurpose,
                InteractionType = i.InteractionType,
                InteractionStatus = i.InteractionStatus,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            });
        query = query.Where(ApplyFilterCompanyInteractions(filter));
        query = ApplySortCompanyInteractions(query, filter);
        return await query.ToPaginatedList(filter);
    }

    public Task<CompanyInteraction> GetInteraction(Expression<Func<CompanyInteraction, bool>> expression) =>
        _context.CompanyInteractions.FirstOrDefaultAsync(expression);

    public async Task<CompanyDeletionCheckModel> GetDeletionCheck(Guid companyProfileId)
    {
        var model = await _context.CompanyProfiles
            .Where(c => c.Id == companyProfileId)
            .Select(c => new CompanyDeletionCheckModel { Id = c.Id, FullName = c.FullName })
            .SingleOrDefaultAsync();
        if (model is null) return null;
        var counts = new Dictionary<string, int>
        {
            ["Requests"] = await _context.Requests.CountAsync(r => r.CompanyProfileId == companyProfileId),
            ["Invoices"] = await _context.Invoices.CountAsync(i => i.CompanyProfileId == companyProfileId),
            ["USA invoices"] = await _context.InvoicesUSA.CountAsync(i => i.CompanyProfileId == companyProfileId),
            ["Deals"] = await _context.Deals.CountAsync(d => d.CompanyProfileId == companyProfileId),
            ["Interactions"] = await _context.CompanyInteractions.CountAsync(i => i.CompanyProfileId == companyProfileId),
            ["Worker comments"] = await _context.WorkerComments.CountAsync(c => c.CompanyProfileId == companyProfileId)
        };
        model.Blockers = counts.Where(c => c.Value > 0)
            .Select(c => new CompanyDeletionBlockerModel { Entity = c.Key, Count = c.Value })
            .ToList();
        return model;
    }

    public Task<List<Guid>> GetCompanyUserIds(Guid companyProfileId) =>
        _context.CompanyUsers.Where(cu => cu.CompanyProfileId == companyProfileId)
            .Select(cu => cu.UserId).ToListAsync();

    public async Task DeleteCompanyProfile(Guid companyProfileId)
    {
        var locationIds = await _context.CompanyProfileLocations
            .Where(l => l.CompanyProfileId == companyProfileId).Select(l => l.LocationId).ToListAsync();
        var noteIds = await _context.CompanyProfileNotes
            .Where(n => n.CompanyProfileId == companyProfileId).Select(n => n.NoteId).ToListAsync();
        var fileIds = await _context.CompanyProfileDocuments
            .Where(d => d.CompanyProfileId == companyProfileId).Select(d => d.DocumentId).ToListAsync();
        var logoId = await _context.CompanyProfiles
            .Where(c => c.Id == companyProfileId).Select(c => c.LogoId).SingleOrDefaultAsync();
        if (logoId.HasValue) fileIds.Add(logoId.Value);

        await using var transaction = await _context.Database.BeginTransactionAsync();
        await _context.CompanyProfileNotes.Where(n => n.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await _context.CompanyProfileDocuments.Where(d => d.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await _context.CompanyProfileLocations.Where(l => l.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await _context.CompanyProfileContactPeople.Where(c => c.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await _context.CompanyProfileJobPositionRates.Where(j => j.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await _context.CompanyProfileInvoiceNotes.Where(n => n.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await _context.CompanyProfileInvoiceRecipients.Where(r => r.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await _context.CompanyUsers.Where(u => u.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await _context.CompanyProfiles.Where(c => c.Id == companyProfileId).ExecuteDeleteAsync();
        await _context.CovenantNotes.Where(n => noteIds.Contains(n.Id)).ExecuteDeleteAsync();
        await _context.Locations.Where(l => locationIds.Contains(l.Id)).ExecuteDeleteAsync();
        await _context.CovenantFiles.Where(f => fileIds.Contains(f.Id)).ExecuteDeleteAsync();
        await transaction.CommitAsync();
    }

    private static Expression<Func<DealListModel, bool>> ApplyFilterDeals(GetDealsFilter filter)
    {
        Expression<Func<DealListModel, bool>> predicate = d => true;
        if (filter.CompanyProfileId.HasValue)
            predicate = predicate.And(d => d.CompanyProfileId == filter.CompanyProfileId.Value);
        if (filter.OwnerId.HasValue)
            predicate = predicate.And(d => d.OwnerId == filter.OwnerId.Value);
        if (filter.Type.HasValue)
            predicate = predicate.And(d => d.Type == filter.Type.Value);
        if (filter.Statuses != null && filter.Statuses.Any())
            predicate = predicate.And(d => filter.Statuses.Contains(d.Status));
        if (filter.DateFrom.HasValue && filter.DateTo.HasValue)
            predicate = predicate.And(d => d.Date.Date >= filter.DateFrom.Value.Date && d.Date.Date <= filter.DateTo.Value.Date);
        return predicate;
    }

    private static IQueryable<DealListModel> ApplySortDeals(IQueryable<DealListModel> query, GetDealsFilter filter) =>
        filter.SortBy switch
        {
            GetDealsSortBy.Company => query.AddOrderBy(filter, d => d.CompanyName),
            GetDealsSortBy.Value => query.AddOrderBy(filter, d => d.Value),
            GetDealsSortBy.Status => query.AddOrderBy(filter, d => d.Status),
            GetDealsSortBy.Date => query.AddOrderBy(filter, d => d.Date),
            _ => query.AddOrderBy(filter, d => d.Date)
        };

    private static Expression<Func<CompanyInteractionListModel, bool>> ApplyFilterCompanyInteractions(GetCompanyInteractionsFilter filter)
    {
        Expression<Func<CompanyInteractionListModel, bool>> predicate = i => true;
        if (filter.CompanyProfileId.HasValue)
            predicate = predicate.And(i => i.CompanyProfileId == filter.CompanyProfileId.Value);
        if (filter.OwnerId.HasValue)
            predicate = predicate.And(i => i.OwnerId == filter.OwnerId.Value);
        if (filter.InteractionPurpose.HasValue)
            predicate = predicate.And(i => i.InteractionPurpose == filter.InteractionPurpose.Value);
        if (filter.InteractionType.HasValue)
            predicate = predicate.And(i => i.InteractionType == filter.InteractionType.Value);
        if (filter.Statuses != null && filter.Statuses.Any())
            predicate = predicate.And(i => filter.Statuses.Contains(i.InteractionStatus));
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(i => i.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && i.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
        return predicate;
    }

    private static IQueryable<CompanyInteractionListModel> ApplySortCompanyInteractions(IQueryable<CompanyInteractionListModel> query, GetCompanyInteractionsFilter filter) =>
        filter.SortBy switch
        {
            GetCompanyInteractionsSortBy.Company => query.AddOrderBy(filter, i => i.CompanyName),
            GetCompanyInteractionsSortBy.Status => query.AddOrderBy(filter, i => i.InteractionStatus),
            GetCompanyInteractionsSortBy.CreatedAt => query.AddOrderBy(filter, i => i.CreatedAt),
            _ => query.AddOrderBy(filter, i => i.CreatedAt)
        };

    public async Task<List<BaseModel<Guid>>> GetCompaniesList(Guid agencyId, string searchTerm)
    {
        var companyProfiles = _context.CompanyProfiles.Where(cp => cp.AgencyId == agencyId);
        var query = companyProfiles
            .Select(cp => new BaseModel<Guid>
            {
                Id = cp.Id,
                Value = cp.FullName
            });
        var predicate = PredicateBuilder.New<BaseModel<Guid>>(true);
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            predicate = predicate.And(p => EF.Functions.Like(p.Value.ToLower(), $"%{searchTerm}%"));
        }
        query = query.Where(predicate);
        var result = await query.OrderBy(c => c.Value).ToListAsync();
        return result;
    }
}