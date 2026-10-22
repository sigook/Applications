using Covenant.Common.Configuration;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities.Sales;
using Covenant.Common.Enums;
using Covenant.Common.Models.Companies.Agencies;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models.Locations;
using Covenant.Common.Models.Sales.Dashboard;
using Covenant.Common.Models.Sales;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Companies;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Extensions;
using Covenant.Infrastructure.Mappers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Companies;

public class CompanyRepository(CovenantContext context, IOptions<FilesConfiguration> options) : BaseRepository<CovenantContext>(context), ICompanyRepository
{
    private readonly FilesConfiguration filesConfiguration = options.Value;

    public Task<CompanyProfileIdsModel> GetCompanyProfileId(Expression<Func<CompanyProfile, bool>> condition) =>
        Context.CompanyProfiles.Where(condition)
            .Select(p => new CompanyProfileIdsModel { Id = p.Id, CompanyId = p.CompanyId })
            .SingleOrDefaultAsync();

    public async Task<IEnumerable<LocationDetailModel>> GetCompanyLocations(Expression<Func<CompanyProfileLocation, bool>> condition)
    {
        var locations = await Context.CompanyProfileLocations
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
        Expression<Func<CompanyProfileJobPositionRate, bool>> predicate = cpjpr =>
            cpjpr.CompanyProfileId == companyProfileId && !cpjpr.IsDeleted;
        return GetJobPositions(predicate.AndSearch(filter.Role, cpjpr => cpjpr.JobPosition));
    }

    public async Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Expression<Func<CompanyProfileJobPositionRate, bool>> expression)
    {
        var query = Context.CompanyProfileJobPositionRates.Where(expression)
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

    public async Task<CompanyProfileJobPositionRate> GetJobPosition(Guid id) => await Context.CompanyProfileJobPositionRates.FirstOrDefaultAsync(cpj => cpj.Id == id);

    public Task<CompanyProfileJobPositionRateModel> GetJobPositionDetail(Guid id) =>
        Context.CompanyProfileJobPositionRates.Where(c => c.Id == id)
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
        var profile = await Context.CompanyProfiles.Where(expression)
            .Include(c => c.Agency).ThenInclude(a => a.User)
            .Include(c => c.Company)
            .Include(c => c.Logo)
            .Include(c => c.Locations).ThenInclude(l => l.Location).ThenInclude(c => c.City).ThenInclude(c => c.Province).ThenInclude(p => p.Country)
            .Include(c => c.ContactPeople)
            .Include(c => c.Industry).ThenInclude(i => i.Industry)
            .SingleOrDefaultAsync();
        if (profile is null) return null;
        var positionRates = await Context.CompanyProfileJobPositionRates
            .Where(c => !c.IsDeleted && c.CompanyProfileId == profile.Id)
            .ToListAsync();
        profile.ReplaceJobPositionRates(positionRates);
        return profile;
    }

    public async Task<PaginatedList<CompanyProfileListModel>> GetCompaniesProfileForAgency(Guid agencyId, GetCompanyForAgencyFilter filter)
    {
        var query = GetAllCompaniesProfileForAgency(agencyId, filter);
        return await query.ToPaginatedList(filter);
    }

    public IQueryable<CompanyProfileListModel> GetAllCompaniesProfileForAgency(Guid agencyId, GetCompanyForAgencyFilter filter)
    {
        var companies = Context.CompanyProfiles
            .Include(cp => cp.Locations)
            .Include(cp => cp.Industry).ThenInclude(i => i.Industry)
            .Include(cp => cp.Company)
            .Include(cp => cp.Notes)
            .AsQueryable();
        if (filter.SalesPersonnelId.HasValue)
            companies = companies.Where(cp => cp.SalesRepresentativeId == filter.SalesPersonnelId.Value);
        var query = from cp in companies
                    from cpcp in Context.CompanyProfileContactPeople.Where(cpcp => cpcp.CompanyProfileId == cp.Id).OrderBy(cpcp => cpcp.Id).Take(1).DefaultIfEmpty()
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
                        ContactName = cpcp == null
                            ? null
                            : cpcp.FirstName + (string.IsNullOrWhiteSpace(cpcp.MiddleName) ? string.Empty : " " + cpcp.MiddleName) + " " + cpcp.LastName,
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

        var contacts = await Context.CompanyProfileContactPeople
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

        var jobPositions = await Context.CompanyProfileJobPositionRates
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

        var users = await Context.CompanyUsers
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

        var locations = await Context.CompanyProfileLocations
            .Where(l => profileIds.Contains(l.CompanyProfileId))
            .OrderByDescending(l => l.IsBilling)
            .ThenBy(l => l.Location.Address)
            .Select(l => new CompanyProfileLocationListModel
            {
                CompanyProfileId = l.CompanyProfileId,
                Address = l.Location.Address,
                City = l.Location.City.Value,
                Province = l.Location.City.Province.Value,
                ProvinceCode = l.Location.City.Province.Code,
                Country = l.Location.City.Province.Country.Value,
                PostalCode = l.Location.PostalCode,
                Entrance = l.Location.Entrance,
                MainIntersection = l.Location.MainIntersection,
                IsBilling = l.IsBilling
            }).ToListAsync();

        var result = companyList.Select(c => new CompanyProfileWithDetailsModel
        {
            Company = c,
            Contacts = contacts.Where(cp => cp.CompanyProfileId == c.Id),
            JobPositions = jobPositions.Where(jp => jp.CompanyProfileId == c.Id),
            Users = users.Where(u => u.CompanyProfileId == c.Id),
            Locations = locations.Where(l => l.CompanyProfileId == c.Id)
        }).ToList();
        return result;
    }

    private Expression<Func<CompanyProfileListModel, bool>> ApplyFilterCompanyProfiles(Guid agencyId, GetCompanyForAgencyFilter filter)
    {
        Expression<Func<CompanyProfileListModel, bool>> predicate = c => c.AgencyId == agencyId;
        if (SearchExtensions.HasSearchTokens(filter.BusinessInfo))
        {
            var nameMatches = SearchExtensions.SearchPredicate<CompanyProfileListModel>(filter.BusinessInfo, c => c.FullName);
            var locationMatches = SearchExtensions.SearchPredicate<string>(filter.BusinessInfo, l => l, l => l.Replace(" ", string.Empty));
            predicate = predicate.And(nameMatches.Or(c => c.Locations.AsQueryable().Any(locationMatches)));
        }
        predicate = predicate
            .AndSearch(filter.ContactInfo, c => c.Email, c => c.ContactName, c => c.ContactRole, c => c.Website)
            .AndSearch(filter.Industry, c => c.Industry)
            .AndSearch(filter.CreatedBy, c => c.CreatedBy)
            .AndSearch(filter.UpdatedBy, c => c.UpdatedBy)
            .AndSearch(filter.SalesRepresentative, c => c.SalesRepresentative);
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(c => c.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && c.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
        if (filter.UpdatedAtFrom.HasValue && filter.UpdatedAtTo.HasValue)
            predicate = predicate.And(c =>
                (c.UpdatedAt.HasValue ? c.UpdatedAt.Value.Date : c.UpdatedAt) >= filter.UpdatedAtFrom.Value.Date &&
                (c.UpdatedAt.HasValue ? c.UpdatedAt.Value.Date : c.UpdatedAt) <= filter.UpdatedAtTo.Value.Date);
        if (filter.CompanyStatuses != null && filter.CompanyStatuses.Any())
            predicate = predicate.And(c => filter.CompanyStatuses.Contains(c.CompanyStatus));
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

    public Task<CompanyProfileDetailModel> GetCompanyProfileDetail(Expression<Func<CompanyProfile, bool>> expression) =>
        SelectCompanyProfileDetail<CompanyProfileDetailModel>(expression).FirstOrDefaultAsync();

    public async Task<AgencyCompanyProfileDetailModel> GetAgencyCompanyProfileDetail(Guid companyProfileId)
    {
        var model = await SelectCompanyProfileDetail<AgencyCompanyProfileDetailModel>(cp => cp.Id == companyProfileId)
            .FirstOrDefaultAsync();
        if (model is null) return null;
        model.Summary = await Context.CompanyProfiles
            .Where(cp => cp.Id == companyProfileId)
            .Select(cp => new CompanyProfileSummaryModel
            {
                OpenRequestsCount = Context.Requests.Count(r => r.CompanyProfileId == cp.Id && r.Status == RequestStatus.Open),
                AsapRequestsCount = Context.Requests.Count(r => r.CompanyProfileId == cp.Id && r.Status == RequestStatus.Open && r.IsAsap),
                WorkersWorkingCount = Context.Requests.Where(r => r.CompanyProfileId == cp.Id && r.Status != RequestStatus.Cancelled)
                    .Sum(r => r.WorkersQuantityWorking),
                RolesCount = cp.JobPositionRates.Count(j => !j.IsDeleted),
                ContactsCount = cp.ContactPeople.Count,
                UsersCount = Context.CompanyUsers.Count(cu => cu.CompanyProfileId == cp.Id),
                LocationsCount = cp.Locations.Count,
                DocumentsCount = cp.Documents.Count,
                SalesRepresentativeName = cp.SalesRepresentative == null
                    ? null
                    : cp.SalesRepresentative.Name ?? cp.SalesRepresentative.User.Email
            })
            .SingleAsync();
        return model;
    }

    private IQueryable<T> SelectCompanyProfileDetail<T>(Expression<Func<CompanyProfile, bool>> expression)
        where T : CompanyProfileDetailModel, new() =>
        Context.CompanyProfiles
            .Where(expression)
            .Select(cp => new T
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
                OvertimeStartsAfter = cp.OvertimeStartsAfter.TotalHours,
                PaymentTermDays = cp.PaymentTermDays
            });

    public Task<Guid> GetCompanyIdForUser(Guid userId) =>
        Context.CompanyUsers.Where(w => w.UserId == userId)
            .Select(s => s.CompanyProfile.CompanyId).SingleOrDefaultAsync();

    public Task<CompanyUserModel> GetCompanyUserDetail(Guid id) =>
        Context.CompanyUsers.Where(uw => uw.UserId == id)
            .Select(CompanyExtensionsMapping.SelectCompanyUser).SingleOrDefaultAsync();

    public Task<CompanyUser> GetCompanyUser(Guid id) =>
        Context.CompanyUsers.Where(uw => uw.UserId == id)
            .Include(u => u.User)
            .SingleOrDefaultAsync();

    public async Task<IEnumerable<CompanyProfileContactPersonModel>> GetContactPeople(Expression<Func<CompanyProfileContactPerson, bool>> condition)
    {
        var query = await Context.CompanyProfileContactPeople
            .Where(condition)
            .Select(CompanyExtensionsMapping.SelectContactPerson)
            .ToListAsync();
        return query;
    }

    public Task<CompanyProfileContactPersonModel> GetContactPersonDetail(Guid profileId, Guid id) =>
        Context.CompanyProfileContactPeople.Where(c => c.CompanyProfileId == profileId && c.Id == id)
            .Select(CompanyExtensionsMapping.SelectContactPerson).SingleOrDefaultAsync();

    public Task<CompanyProfileContactPerson> GetContactPerson(Guid id) => Context.CompanyProfileContactPeople.SingleOrDefaultAsync(c => c.Id == id);

    public Task<CompanyProfileLocationDetailModel> GetLocationDetail(Guid id) =>
        Context.CompanyProfileLocations.Where(c => c.LocationId == id)
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
        Context.CompanyProfileLocations.Where(c => c.LocationId == id)
            .Include(c => c.Location)
            .Select(l => l).SingleOrDefaultAsync();

    public Task<PaginatedList<CompanyProfileDocumentModel>> GetDocuments(Guid profileId, Pagination pagination) =>
        Context.CompanyProfileDocuments.Where(c => c.CompanyProfileId == profileId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(s => new CompanyProfileDocumentModel
            {
                Id = s.DocumentId,
                FileName = s.Document.FileName,
                Description = s.Document.Description,
                PathFile = $"{filesConfiguration.FilesPath}{s.Document.FileName}",
                DocumentType = s.DocumentType,
                CanDownload = true
            }).ToPaginatedList(pagination);

    public Task<CompanyProfileDocument> GetDocument(Guid id) => Context.CompanyProfileDocuments.SingleOrDefaultAsync(c => c.DocumentId == id);

    public Task<PaginatedList<NoteModel>> GetNotes(Guid profileId, Pagination pagination) =>
        Context.CompanyProfileNotes.Where(w => w.CompanyProfileId == profileId && !w.Note.IsDeleted)
            .Select(CompanyExtensionsMapping.SelectNote).OrderByDescending(c => c.CreatedAt).ToPaginatedList(pagination);

    public Task<NoteModel> GetNoteDetail(Guid profileId, Guid id) =>
        Context.CompanyProfileNotes.Where(w => w.CompanyProfileId == profileId && w.NoteId == id)
            .Select(CompanyExtensionsMapping.SelectNote).SingleOrDefaultAsync();

    public Task<CompanyProfileNote> GetNote(Guid profileId, Guid id) =>
        Context.CompanyProfileNotes.Where(c => c.CompanyProfileId == profileId && c.NoteId == id)
            .Include(c => c.Note).SingleOrDefaultAsync();

    public Task<string> GetCompanyProfileInvoiceNotes(Guid companyProfileId) =>
        Context.CompanyProfileInvoiceNotes.Where(n => n.CompanyProfileId == companyProfileId)
            .Select(n => n.HtmlNotes)
            .AsNoTracking()
            .SingleOrDefaultAsync();

    public async Task UpdateCompanyProfileInvoiceNotes(Guid companyProfileId, string htmlNotes)
    {
        CompanyProfileInvoiceNotes notes = await Context.CompanyProfileInvoiceNotes.Where(n => n.CompanyProfileId == companyProfileId)
            .SingleOrDefaultAsync();
        if (notes is null)
        {
            await Context.CompanyProfileInvoiceNotes.AddAsync(new CompanyProfileInvoiceNotes(companyProfileId, htmlNotes));
        }
        else
        {
            notes.ChangeHtmlNotes(htmlNotes);
            Context.CompanyProfileInvoiceNotes.Update(notes);
        }
        await Context.SaveChangesAsync();
    }

    public Task<List<CompanyProfileInvoiceRecipientModel>> GetInvoiceRecipients(Guid companyProfileId) =>
        Context.CompanyProfileInvoiceRecipients.Where(r => r.CompanyProfileId == companyProfileId)
            .Select(r => new CompanyProfileInvoiceRecipientModel { Id = r.Id, Email = r.Email, Name = r.Name })
            .ToListAsync();
    public async Task<CompanyProfileInvoiceRecipient> GetInvoiceRecipient(Guid id)
    {
        return await Context.CompanyProfileInvoiceRecipients.FirstOrDefaultAsync(cpir => cpir.Id == id);
    }

    public async Task UpdateInvoiceRecipient(Guid id, CompanyProfileInvoiceRecipientModel model)
    {
        var entity = await Context.CompanyProfileInvoiceRecipients.FirstOrDefaultAsync(r => r.Id == id);
        if (entity is null) return;
        entity.UpdateName(model.Name);
        entity.UpdateEmail(model.Email);
        Context.CompanyProfileInvoiceRecipients.Update(entity);
    }

    public async Task<Guid> CreateInvoiceRecipient(Guid companyProfileId, CompanyProfileInvoiceRecipientModel model)
    {
        var entity = new CompanyProfileInvoiceRecipient(companyProfileId, model.Email);
        entity.UpdateName(model.Name);
        await Create(entity);
        return entity.Id;
    }

    public async Task<IEnumerable<CompanyUserModel>> GetAllCompanyUsers(Guid companyProfileId)
    {
        var companyUsers = await Context.CompanyUsers
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

        await Context.AddRangeAsync(companies);
        await Context.AddRangeAsync(companyPhones);
        await Context.AddRangeAsync(companyLocations);

    }

    public async Task<PaginatedList<DealListModel>> GetDeals(Guid agencyId, Guid? companyProfileId, GetDealsFilter filter)
    {
        var deals = Context.Deals.Where(d => d.CompanyProfile.AgencyId == agencyId);
        if (companyProfileId.HasValue) deals = deals.Where(d => d.CompanyProfileId == companyProfileId.Value);
        var query = deals
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
        Context.Deals.FirstOrDefaultAsync(expression);

    public async Task<PaginatedList<CompanyInteractionListModel>> GetInteractions(Guid agencyId, Guid? companyProfileId, GetCompanyInteractionsFilter filter)
    {
        var interactions = Context.CompanyInteractions.Where(i => i.CompanyProfile.AgencyId == agencyId);
        if (companyProfileId.HasValue) interactions = interactions.Where(i => i.CompanyProfileId == companyProfileId.Value);
        var query = interactions
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
        Context.CompanyInteractions.FirstOrDefaultAsync(expression);

    public Task<List<DealStatusSummaryModel>> GetDealsByStatus(Guid agencyId, Guid? ownerId, DateTime fromUtc, DateTime toUtcExclusive, List<DealStatus> statuses)
    {
        var query = Context.Deals
            .Where(d => d.CompanyProfile.AgencyId == agencyId && d.Date >= fromUtc && d.Date < toUtcExclusive);
        if (ownerId.HasValue) query = query.Where(d => d.UserId == ownerId.Value);
        if (statuses is { Count: > 0 }) query = query.Where(d => statuses.Contains(d.Status));
        return query
            .GroupBy(d => d.Status)
            .Select(g => new DealStatusSummaryModel
            {
                Status = g.Key,
                Count = g.Count(),
                TotalValue = g.Sum(d => d.Value)
            })
            .ToListAsync();
    }

    public Task<List<InteractionTypeSummaryModel>> GetInteractionsByType(Guid agencyId, Guid? ownerId, DateTime fromUtc, DateTime toUtcExclusive)
    {
        var query = Context.CompanyInteractions
            .Where(i => i.CompanyProfile.AgencyId == agencyId && i.CreatedAt >= fromUtc && i.CreatedAt < toUtcExclusive);
        if (ownerId.HasValue) query = query.Where(i => i.UserId == ownerId.Value);
        return query
            .GroupBy(i => i.InteractionType)
            .Select(g => new InteractionTypeSummaryModel
            {
                Type = g.Key,
                Count = g.Count()
            })
            .ToListAsync();
    }

    public Task<List<RecentClientModel>> GetRecentInteractionClients(Guid agencyId, Guid? ownerId, int take)
    {
        var interactions = Context.CompanyInteractions.AsQueryable();
        if (ownerId.HasValue) interactions = interactions.Where(i => i.UserId == ownerId.Value);
        return Context.CompanyProfiles
            .Where(cp => cp.AgencyId == agencyId)
            .Select(cp => new
            {
                cp.Id,
                cp.FullName,
                cp.Company.Email,
                LastInteractionAt = interactions.Where(i => i.CompanyProfileId == cp.Id).Max(i => (DateTime?)i.CreatedAt)
            })
            .Where(c => c.LastInteractionAt != null)
            .OrderByDescending(c => c.LastInteractionAt)
            .Take(take)
            .Select(c => new RecentClientModel
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                LastInteractionAt = c.LastInteractionAt.Value
            })
            .ToListAsync();
    }

    public async Task<CompanyDeletionCheckModel> GetDeletionCheck(Guid companyProfileId)
    {
        var model = await Context.CompanyProfiles
            .Where(c => c.Id == companyProfileId)
            .Select(c => new CompanyDeletionCheckModel { Id = c.Id, FullName = c.FullName })
            .SingleOrDefaultAsync();
        if (model is null) return null;
        var counts = new Dictionary<string, int>
        {
            ["Requests"] = await Context.Requests.CountAsync(r => r.CompanyProfileId == companyProfileId),
            ["Invoices"] = await Context.Invoices.CountAsync(i => i.CompanyProfileId == companyProfileId),
            ["Deals"] = await Context.Deals.CountAsync(d => d.CompanyProfileId == companyProfileId),
            ["Interactions"] = await Context.CompanyInteractions.CountAsync(i => i.CompanyProfileId == companyProfileId),
            ["Worker comments"] = await Context.WorkerComments.CountAsync(c => c.CompanyProfileId == companyProfileId)
        };
        model.Blockers = counts.Where(c => c.Value > 0)
            .Select(c => new CompanyDeletionBlockerModel { Entity = c.Key, Count = c.Value })
            .ToList();
        return model;
    }

    public Task<List<Guid>> GetCompanyUserIds(Guid companyProfileId) =>
        Context.CompanyUsers.Where(cu => cu.CompanyProfileId == companyProfileId)
            .Select(cu => cu.UserId).ToListAsync();

    public async Task DeleteCompanyProfile(Guid companyProfileId)
    {
        var locationIds = await Context.CompanyProfileLocations
            .Where(l => l.CompanyProfileId == companyProfileId).Select(l => l.LocationId).ToListAsync();
        var noteIds = await Context.CompanyProfileNotes
            .Where(n => n.CompanyProfileId == companyProfileId).Select(n => n.NoteId).ToListAsync();
        var fileIds = await Context.CompanyProfileDocuments
            .Where(d => d.CompanyProfileId == companyProfileId).Select(d => d.DocumentId).ToListAsync();
        var logoId = await Context.CompanyProfiles
            .Where(c => c.Id == companyProfileId).Select(c => c.LogoId).SingleOrDefaultAsync();
        if (logoId.HasValue) fileIds.Add(logoId.Value);

        await using var transaction = await Context.Database.BeginTransactionAsync();
        await Context.CompanyProfileNotes.Where(n => n.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await Context.CompanyProfileDocuments.Where(d => d.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await Context.CompanyProfileLocations.Where(l => l.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await Context.CompanyProfileContactPeople.Where(c => c.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await Context.CompanyProfileJobPositionRates.Where(j => j.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await Context.CompanyProfileInvoiceNotes.Where(n => n.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await Context.CompanyProfileInvoiceRecipients.Where(r => r.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await Context.CompanyUsers.Where(u => u.CompanyProfileId == companyProfileId).ExecuteDeleteAsync();
        await Context.CompanyProfiles.Where(c => c.Id == companyProfileId).ExecuteDeleteAsync();
        await Context.CovenantNotes.Where(n => noteIds.Contains(n.Id)).ExecuteDeleteAsync();
        await Context.Locations.Where(l => locationIds.Contains(l.Id)).ExecuteDeleteAsync();
        await Context.CovenantFiles.Where(f => fileIds.Contains(f.Id)).ExecuteDeleteAsync();
        await transaction.CommitAsync();
    }

    private static Expression<Func<DealListModel, bool>> ApplyFilterDeals(GetDealsFilter filter)
    {
        Expression<Func<DealListModel, bool>> predicate = d => true;
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
            GetDealsSortBy.Value => query.AddOrderBy(filter, d => d.Value),
            GetDealsSortBy.Status => query.AddOrderBy(filter, d => d.Status),
            GetDealsSortBy.Date => query.AddOrderBy(filter, d => d.Date),
            _ => query.AddOrderBy(filter, d => d.Date)
        };

    private static Expression<Func<CompanyInteractionListModel, bool>> ApplyFilterCompanyInteractions(GetCompanyInteractionsFilter filter)
    {
        Expression<Func<CompanyInteractionListModel, bool>> predicate = i => true;
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
            GetCompanyInteractionsSortBy.Status => query.AddOrderBy(filter, i => i.InteractionStatus),
            GetCompanyInteractionsSortBy.CreatedAt => query.AddOrderBy(filter, i => i.CreatedAt),
            _ => query.AddOrderBy(filter, i => i.CreatedAt)
        };

    public async Task<List<BaseModel<Guid>>> GetCompaniesList(Guid agencyId, string searchTerm)
    {
        var query = Context.CompanyProfiles
            .Where(cp => cp.AgencyId == agencyId)
            .WhereSearch(searchTerm, cp => cp.FullName)
            .Select(cp => new BaseModel<Guid>
            {
                Id = cp.Id,
                Value = cp.FullName
            });
        var result = await query.OrderBy(c => c.Value).ToListAsync();
        return result;
    }
}