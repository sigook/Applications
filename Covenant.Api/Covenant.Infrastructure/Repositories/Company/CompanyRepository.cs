using Covenant.Common.Configuration;
using Covenant.Common.Entities.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Mappers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Company
{
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

        public async Task<Guid> GetCompanyId(Guid companyProfileId) =>
            await _context.CompanyProfile.Where(p => p.Id == companyProfileId)
                .Select(p => p.CompanyId).SingleOrDefaultAsync();

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

        public async Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Expression<Func<CompanyProfileJobPositionRate, bool>> expression)
        {
            var query = _context.CompanyProfileJobPositionRate.Where(expression)
                .Select(s => new CompanyProfileJobPositionRateModel
                {
                    Id = s.Id,
                    Rate = s.Rate,
                    WorkerRate = s.WorkerRate,
                    WorkerRateMin = s.WorkerRateMin,
                    WorkerRateMax = s.WorkerRateMax,
                    AsapRate = s.AsapRate,
                    Description = s.Description,
                    CreatedAt = s.CreatedAt,
                    CreatedBy = s.CreatedBy,
                    OtherJobPosition = s.OtherJobPosition,
                    JobPosition = s.JobPositionId == null ? null : new JobPositionDetailModel
                    {
                        Id = s.JobPosition.Id,
                        Value = s.JobPosition.Value
                    },
                    Value = s.JobPositionId == null ? s.OtherJobPosition : s.JobPosition.Value,
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
            var result = await query.OrderBy(c => c.Value).ToListAsync();
            return result;
        }

        public async Task<CompanyProfileJobPositionRate> GetJobPosition(Guid id) => await _context.CompanyProfileJobPositionRate.FirstOrDefaultAsync(cpj => cpj.Id == id);

        public Task<CompanyProfileJobPositionRateModel> GetJobPositionDetail(Guid id) =>
            _context.CompanyProfileJobPositionRate.Where(c => c.Id == id)
                .Select(s => new CompanyProfileJobPositionRateModel
                {
                    Id = s.Id,
                    Rate = s.Rate,
                    WorkerRate = s.WorkerRate,
                    WorkerRateMin = s.WorkerRateMin,
                    WorkerRateMax = s.WorkerRateMax,
                    AsapRate = s.AsapRate,
                    Description = s.Description,
                    CreatedAt = s.CreatedAt,
                    CreatedBy = s.CreatedBy,
                    OtherJobPosition = s.OtherJobPosition,
                    Value = s.JobPositionId == null ? s.OtherJobPosition : s.JobPosition.Value,
                    JobPosition = s.JobPositionId == null ? null : new JobPositionDetailModel
                    {
                        Id = s.JobPosition.Id,
                        Value = s.JobPosition.Value
                    },
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

        public Task<List<CompanyProfileAgencyListModel>> GetCompanyProfiles(Guid companyId) =>
            (from cp in _context.CompanyProfile.Where(c => c.CompanyId == companyId)
             join a in _context.Agencies on cp.AgencyId equals a.Id
             join cf in _context.CovenantFile on a.LogoId equals cf.Id into tmp
             from cfl in tmp.DefaultIfEmpty()
             select new CompanyProfileAgencyListModel
             {
                 Id = cp.Id,
                 AgencyId = a.Id,
                 AgencyFullName = a.FullName,
                 AgencyLogo = cfl == null ? null : $"{filesConfiguration.FilesPath}{cfl.FileName}",
                 Active = cp.Active
             }).AsNoTracking().ToListAsync();

        public async Task<CompanyProfile> GetCompanyProfile(Expression<Func<CompanyProfile, bool>> expression)
        {
            var profile = await _context.CompanyProfile.Where(expression)
                .Include(c => c.Agency).ThenInclude(a => a.User)
                .Include(c => c.Company)
                .Include(c => c.Logo)
                .Include(c => c.Locations).ThenInclude(l => l.Location).ThenInclude(c => c.City).ThenInclude(c => c.Province).ThenInclude(p => p.Country)
                .Include(c => c.ContactPersons)
                .Include(c => c.Industry).ThenInclude(i => i.Industry)
                .SingleOrDefaultAsync();
            if (profile is null) return null;
            var positionRates = await _context.CompanyProfileJobPositionRate
                .Where(c => !c.IsDeleted && c.CompanyProfileId == profile.Id)
                .Include(c => c.JobPosition)
                .ToListAsync();
            profile.JobPositionRates = positionRates;
            return profile;
        }

        public async Task<PaginatedList<CompanyProfileListModel>> GetCompaniesProfileForAgency(Guid agencyId, GetCompanyForAgencyFilter filter)
        {
            var query = GetAllCompaniesProfileForAgency(agencyId, filter);
            return await query.ToPaginatedList(filter);
        }

        public IEnumerable<CompanyProfileListModel> GetAllCompaniesProfileForAgency(Guid agencyId, GetCompanyForAgencyFilter filter)
        {
            var companies = _context.CompanyProfile
                .Include(cp => cp.Locations)
                .Include(cp => cp.Industry).ThenInclude(i => i.Industry)
                .Include(cp => cp.Company)
                .Include(cp => cp.Notes)
                .AsQueryable();
            var query = from cp in companies
                        join cf in _context.CovenantFile on cp.LogoId equals cf.Id into tmp
                        from cfl in tmp.DefaultIfEmpty()
                        from cpcp in _context.CompanyProfileContactPersons.Where(cpcp => cpcp.CompanyProfileId == cp.Id).Take(1).DefaultIfEmpty()
                        orderby cp.BusinessName
                        select new CompanyProfileListModel
                        {
                            AgencyId = cp.AgencyId,
                            Id = cp.Id,
                            CompanyId = cp.CompanyId,
                            BusinessName = cp.BusinessName,
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

        private Expression<Func<CompanyProfileListModel, bool>> ApplyFilterCompanyProfiles(Guid agencyId, GetCompanyForAgencyFilter filter)
        {
            Expression<Func<CompanyProfileListModel, bool>> predicate = c => c.AgencyId == agencyId;
            if (!string.IsNullOrWhiteSpace(filter.BusinessInfo))
            {
                var businessInfo = filter.BusinessInfo.ToLower();
                predicate = predicate.And(c =>
                    c.BusinessName.ToLower().Contains(businessInfo) ||
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
            var query = _context.CompanyProfile
                .Include(cp => cp.Locations)
                .Include(cp => cp.ContactPersons)
                .Include(cp => cp.JobPositionRates).ThenInclude(jpr => jpr.JobPosition)
                .Include(cp => cp.Industry).ThenInclude(i => i.Industry)
                .Include(cp => cp.Logo)
                .Where(expression)
                .Select(cp => new CompanyProfileDetailModel
                {
                    Id = cp.Id,
                    NumberId = cp.NumberId,
                    CompanyId = cp.CompanyId,
                    FullName = cp.FullName,
                    BusinessName = cp.BusinessName,
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
                    RequiresPermissionToSeeOrders = cp.RequiresPermissionToSeeOrders,
                    SalesRepresentativeId = cp.SalesRepresentativeId,
                    OvertimeStartsAfter = cp.OvertimeStartsAfter.TotalHours
                });
            return await query.FirstOrDefaultAsync();
        }

        public Task SaveChangesAsync() => _context.SaveChangesAsync();

        public async Task<IEnumerable<CompanyUserModel>> GetCompanyUsers(Guid companyId)
        {
            var companyUsers = await _context.CompanyUser
                .Where(uW => uW.CompanyId == companyId)
                .Select(CompanyExtensionsMapping.SelectCompanyUser)
                .ToListAsync();
            return companyUsers;
        }

        public Task<Guid> GetCompanyIdForUser(Guid userId) =>
            _context.CompanyUser.Where(w => w.UserId == userId)
                .Select(s => s.CompanyId).SingleOrDefaultAsync();

        public Task<CompanyUserModel> GetCompanyUserDetail(Guid id) =>
            _context.CompanyUser.Where(uw => uw.UserId == id)
                .Select(CompanyExtensionsMapping.SelectCompanyUser).SingleOrDefaultAsync();

        public Task<CompanyUser> GetCompanyUser(Guid id) =>
            _context.CompanyUser.Where(uw => uw.UserId == id)
                .Include(u => u.User)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<CompanyProfileContactPersonModel>> GetContactPersons(Expression<Func<CompanyProfileContactPerson, bool>> condition)
        {
            var query = await _context.CompanyProfileContactPersons
                .Where(condition)
                .Select(CompanyExtensionsMapping.SelectContactPerson)
                .ToListAsync();
            return query;
        }

        public Task<CompanyProfileContactPersonModel> GetContactPersonDetail(Guid profileId, Guid id) =>
            _context.CompanyProfileContactPersons.Where(c => c.CompanyProfileId == profileId && c.Id == id)
                .Select(CompanyExtensionsMapping.SelectContactPerson).SingleOrDefaultAsync();

        public Task<CompanyProfileContactPerson> GetContactPerson(Guid id) => _context.CompanyProfileContactPersons.SingleOrDefaultAsync(c => c.Id == id);

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
            _context.CompanyProfileInvoiceRecipient.Where(r => r.CompanyProfileId == companyProfileId)
                .Select(r => new CompanyProfileInvoiceRecipientModel { Id = r.Id, Email = r.Email, Name = r.Name })
                .ToListAsync();
        public async Task<CompanyProfileInvoiceRecipient> GetInvoiceRecipient(Guid id)
        {
            return await _context.CompanyProfileInvoiceRecipient.FirstOrDefaultAsync(cpir => cpir.Id == id);
        }

        public async Task UpdateInvoiceRecipient(Guid id, CompanyProfileInvoiceRecipientModel model)
        {
            var entity = await _context.CompanyProfileInvoiceRecipient.FirstOrDefaultAsync(r => r.Id == id);
            if (entity is null) return;
            entity.Name = model.Name;
            entity.UpdateEmail(model.Email);
            _context.CompanyProfileInvoiceRecipient.Update(entity);
        }

        public async Task<Guid> CreateInvoiceRecipient(Guid companyProfileId, CompanyProfileInvoiceRecipientModel model)
        {
            var entity = new CompanyProfileInvoiceRecipient(companyProfileId, model.Email) { Name = model.Name };
            await Create(entity);
            return entity.Id;
        }

        public async Task<IEnumerable<CompanyUserModel>> GetAllCompanyUsers(Guid companyId)
        {
            var companyUsers = await _context.CompanyUser
                .Where(cu => cu.CompanyId == companyId)
                .Select(CompanyExtensionsMapping.SelectCompanyUser).ToListAsync();
            return companyUsers;
        }

        public async Task<IEnumerable<ProvinceModel>> GetCompanyProvincesWithTaxes(Guid id)
        {
            var locations = _context.CompanyProfile.Include(cp => cp.Locations)
                .Where(cp => cp.Id == id)
                .SelectMany(cp => cp.Locations)
                .Where(l => l.Location.City.Province.ProvinceTax.Tax1 > 0)
                .Select(l => new ProvinceModel
                {
                    Id = l.Location.City.Province.Id,
                    Value = l.Location.City.Province.Value,
                    Code = l.Location.City.Province.Code
                }).Distinct();
            var results = await locations.OrderBy(l => l.Value).ToListAsync();
            return results;
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
    }
}