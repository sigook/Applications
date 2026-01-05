using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Covenant.Infrastructure.Repositories.Worker;

public class WorkerRepository : IWorkerRepository
{
    private readonly CovenantContext _context;
    private readonly FilesConfiguration filesConfiguration;

    public WorkerRepository(CovenantContext context, IOptions<FilesConfiguration> options)
    {
        _context = context;
        filesConfiguration = options.Value;
    }

    public Task UpdateProfile(WorkerProfile entity) => Task.FromResult(_context.WorkerProfile.Update(entity));

    public Task<List<WorkerProfileAgencyListModel>> GetProfiles(Guid workerId)
    {
        return (from wp in _context.WorkerProfile.Where(c => c.WorkerId == workerId)
                join a in _context.Agencies on wp.AgencyId equals a.Id
                join cf in _context.CovenantFile on a.LogoId equals cf.Id into tmp
                from cfl in tmp.DefaultIfEmpty()
                select new WorkerProfileAgencyListModel
                {
                    Id = wp.Id,
                    AgencyFullName = a.FullName,
                    AgencyLogo = cfl == null ? null : $"{filesConfiguration.FilesPath}{cfl.FileName}"
                }).AsNoTracking().ToListAsync();
    }

    public Task<WorkerProfile> GetProfile(Expression<Func<WorkerProfile, bool>> condition) =>
        _context.WorkerProfile.Where(condition)
            .Include(e => e.Worker)
            .Include(c => c.ProfileImage)
            .Include(c => c.SocialInsuranceFile)
            .Include(c => c.Gender)
            .Include(c => c.Location).ThenInclude(c => c.City).ThenInclude(c => c.Province).ThenInclude(c => c.Country)
            .Include(c => c.Lift)
            .Include(c => c.IdentificationType1)
            .Include(c => c.IdentificationType1File)
            .Include(c => c.IdentificationType2)
            .Include(c => c.IdentificationType2File)
            .Include(c => c.PoliceCheckBackGround)
            .Include(c => c.Skills)
            .Include(c => c.Availabilities).ThenInclude(c => c.Availability)
            .Include(c => c.AvailabilityTimes).ThenInclude(c => c.AvailabilityTime)
            .Include(c => c.AvailabilityDays).ThenInclude(c => c.Day)
            .Include(c => c.LocationPreferences).ThenInclude(c => c.City)
            .Include(c => c.Languages).ThenInclude(c => c.Language)
            .Include(c => c.JobExperiences)
            .Include(c => c.Licenses).ThenInclude(c => c.License)
            .Include(c => c.Certificates).ThenInclude(c => c.Certificate)
            .Include(c => c.Certificates)
            .Include(c => c.Resume)
            .Include(c => c.WorkerProfileTaxCategory)
            .FirstOrDefaultAsync();

    public Task<WorkerProfileDetailModel> GetWorkerProfileDetail(Guid profileId)
    {
        return (from wp in _context.WorkerProfile.Where(wp => wp.Id == profileId)
                join u in _context.User on wp.WorkerId equals u.Id
                join cf in _context.CovenantFile on wp.ProfileImageId equals cf.Id into temp13
                from cf in temp13.DefaultIfEmpty()
                join gender12 in _context.Gender on wp.GenderId equals gender12.Id into tmp12
                from gender12 in tmp12.DefaultIfEmpty()
                join scf1 in _context.CovenantFile on wp.SocialInsuranceFileId equals scf1.Id into tmp1
                from scf1 in tmp1.DefaultIfEmpty()
                join icf2 in _context.CovenantFile on wp.IdentificationType1FileId equals icf2.Id into tmp2
                from icf2 in tmp2.DefaultIfEmpty()
                join icf3 in _context.CovenantFile on wp.IdentificationType2FileId equals icf3.Id into tmp3
                from icf3 in tmp3.DefaultIfEmpty()
                join type4 in _context.IdentificationType on wp.IdentificationType1Id equals type4.Id into tmp4
                from type4 in tmp4.DefaultIfEmpty()
                join type5 in _context.IdentificationType on wp.IdentificationType2Id equals type5.Id into tmp5
                from type5 in tmp5.DefaultIfEmpty()
                join pcf6 in _context.CovenantFile on wp.PoliceCheckBackGroundId equals pcf6.Id into tmp6
                from pcf6 in tmp6.DefaultIfEmpty()
                join l7 in _context.Location on wp.LocationId equals l7.Id
                join lift10 in _context.Lift on wp.LiftId equals lift10.Id into tmp10
                from lift10 in tmp10.DefaultIfEmpty()
                join cfr11 in _context.CovenantFile on wp.ResumeId equals cfr11.Id into tmp11
                from cfr11 in tmp11.DefaultIfEmpty()
                select new WorkerProfileDetailModel
                {
                    Id = wp.Id,
                    NumberId = wp.NumberId,
                    ProfileImage = cf == null
                        ? new CovenantFileModel
                        {
                            FileName = "worker.png",
                            PathFile = string.Concat(filesConfiguration.FilesPath, "worker.png")
                        }
                        : new CovenantFileModel
                        {
                            Id = cf.Id,
                            Description = cf.Description,
                            FileName = cf.FileName,
                            PathFile = string.Concat(filesConfiguration.FilesPath, cf.FileName)
                        },
                    FirstName = wp.FirstName,
                    MiddleName = wp.MiddleName,
                    LastName = wp.LastName,
                    SecondLastName = wp.SecondLastName,
                    BirthDay = wp.BirthDay,
                    Gender = gender12 == null ? null : new BaseModel<Guid> { Id = gender12.Id, Value = gender12.Value },
                    SocialInsurance = wp.SocialInsurance,
                    SocialInsuranceExpire = wp.SocialInsuranceExpire,
                    DueDate = wp.DueDate,
                    SocialInsuranceFile = scf1 == null
                        ? null
                        : new CovenantFileModel
                        {
                            Id = scf1.Id,
                            Description = scf1.Description,
                            FileName = scf1.FileName,
                            PathFile = string.Concat(filesConfiguration.FilesPath, scf1.FileName)
                        },
                    IdentificationNumber1 = wp.IdentificationNumber1,
                    IdentificationNumber2 = wp.IdentificationNumber2,
                    HavePoliceCheckBackground = wp.HavePoliceCheckBackground,
                    IdentificationType1File = icf2 == null
                        ? null
                        : new CovenantFileModel
                        {
                            Id = icf2.Id,
                            Description = icf2.Description,
                            FileName = icf2.FileName,
                            PathFile = string.Concat(filesConfiguration.FilesPath, icf2.FileName)
                        },
                    IdentificationType2File = icf3 == null
                        ? null
                        : new CovenantFileModel
                        {
                            Id = icf3.Id,
                            Description = icf3.Description,
                            FileName = icf3.FileName,
                            PathFile = string.Concat(filesConfiguration.FilesPath, icf3.FileName)
                        },
                    IdentificationType1 = type4 == null ? null : new BaseModel<Guid> { Id = type4.Id, Value = type4.Value },
                    IdentificationType2 = type5 == null ? null : new BaseModel<Guid> { Id = type5.Id, Value = type5.Value },
                    PoliceCheckBackGround = pcf6 == null
                        ? null
                        : new CovenantFileModel
                        {
                            Id = pcf6.Id,
                            Description = pcf6.Description,
                            FileName = pcf6.FileName,
                            PathFile = string.Concat(filesConfiguration.FilesPath, pcf6.FileName)
                        },
                    MobileNumber = wp.MobileNumber,
                    Phone = wp.Phone,
                    PhoneExt = wp.PhoneExt,
                    Location = l7 == null
                        ? null
                        : new LocationDetailModel
                        {
                            Id = l7.Id,
                            Address = l7.Address,
                            Latitude = l7.Latitude,
                            Longitude = l7.Longitude,
                            PostalCode = l7.PostalCode,
                            City = new CityModel
                            {
                                Id = l7.City.Id,
                                Value = l7.City.Value,
                                Code = l7.City.Code,
                                Province = new ProvinceModel
                                {
                                    Id = l7.City.Province.Id,
                                    Value = l7.City.Province.Value,
                                    Code = l7.City.Province.Code,
                                    Country = new CountryModel
                                    {
                                        Id = l7.City.Province.Country.Id,
                                        Value = l7.City.Province.Country.Value,
                                        Code = l7.City.Province.Country.Code
                                    }
                                }
                            }
                        },
                    HasVehicle = wp.HasVehicle,
                    Licenses = wp.Licenses.Select(l => new WorkerProfileLicenseDetailModel
                    {
                        Number = l.Number,
                        Expires = l.Expires,
                        Issued = l.Issued,
                        License = new CovenantFileModel
                        {
                            Id = l.Id,
                            Description = l.License.Description,
                            FileName = l.License.FileName,
                            PathFile = filesConfiguration.FilesPath + l.License.FileName
                        }
                    }),
                    Certificates = wp.Certificates.Select(c => new CovenantFileModel
                    {
                        Id = c.Id,
                        Description = c.Certificate.Description,
                        FileName = c.Certificate.FileName,
                        PathFile = filesConfiguration.FilesPath + c.Certificate.FileName
                    }),
                    OtherDocuments = wp.OtherDocuments.Select(od => new CovenantFileModel
                    {
                        Id = od.Id,
                        Description = od.Document.Description,
                        FileName = od.Document.FileName,
                        PathFile = filesConfiguration.FilesPath + od.Document.FileName
                    }),
                    Availabilities = wp.Availabilities.Select(a => new BaseModel<Guid>
                    {
                        Id = a.Availability.Id,
                        Value = a.Availability.Value
                    }),
                    AvailabilityTimes = wp.AvailabilityTimes.Select(at => new BaseModel<Guid>
                    {
                        Id = at.AvailabilityTime.Id,
                        Value = at.AvailabilityTime.Value
                    }),
                    AvailabilityDays = wp.AvailabilityDays.Select(ad => new BaseModel<Guid>
                    {
                        Id = ad.Day.Id,
                        Value = ad.Day.Value
                    }),
                    LocationPreferences = wp.LocationPreferences.Select(lp => new BaseModel<Guid>
                    {
                        Id = lp.City.Id,
                        Value = lp.City.Value
                    }),
                    Lift = lift10 == null ? null : new BaseModel<Guid> { Id = lift10.Id, Value = lift10.Value },
                    Languages = wp.Languages.Select(l => new BaseModel<Guid>
                    {
                        Id = l.Language.Id,
                        Value = l.Language.Value
                    }),
                    Skills = wp.Skills.Select(skill => new SkillModel { Id = skill.Id, Skill = skill.Skill }).ToList(),
                    Resume = cfr11 == null
                        ? null
                        : new CovenantFileModel
                        {
                            Id = cfr11.Id,
                            Description = cfr11.Description,
                            FileName = cfr11.FileName,
                            PathFile = string.Concat(filesConfiguration.FilesPath, cfr11.FileName)
                        },
                    HaveAnyHealthProblem = wp.HaveAnyHealthProblem,
                    HealthProblem = wp.HealthProblem,
                    OtherHealthProblem = wp.OtherHealthProblem,
                    ContactEmergencyName = wp.ContactEmergencyName,
                    ContactEmergencyLastName = wp.ContactEmergencyLastName,
                    ContactEmergencyPhone = wp.ContactEmergencyPhone,
                    JobExperiences = wp.JobExperiences.Select(je => new WorkerProfileJobExperienceDetailModel
                    {
                        Id = je.Id,
                        Company = je.Company,
                        Duties = je.Duties,
                        Supervisor = je.Supervisor,
                        EndDate = je.EndDate,
                        StartDate = je.StartDate,
                        IsCurrentJobPosition = je.IsCurrentJobPosition
                    }),
                    Email = u.Email,
                    WorkerId = wp.WorkerId,
                    IsSubcontractor = wp.IsSubcontractor,
                    IsContractor = wp.IsContractor,
                    ApprovedToWork = wp.ApprovedToWork,
                    Dnu = wp.Dnu,
                    CreatedBy = wp.CreatedBy,
                    PunchCardId = wp.PunchCardId != null ? wp.PunchCardId : wp.WorkerId.ToString(),
                    FederalTaxCategory = wp.WorkerProfileTaxCategory != null ? wp.WorkerProfileTaxCategory.FederalCategory : null,
                    ProvincialTaxCategory = wp.WorkerProfileTaxCategory != null ? wp.WorkerProfileTaxCategory.ProvincialCategory : null,
                    Cpp = wp.WorkerProfileTaxCategory != null ? wp.WorkerProfileTaxCategory.Cpp : null,
                    Ei = wp.WorkerProfileTaxCategory != null ? wp.WorkerProfileTaxCategory.Ei : null
                })
            .SingleOrDefaultAsync();
    }

    public Task<List<CovenantFileModel>> GetOtherDocuments(Guid profileId) =>
                    _context.WorkerProfileOtherDocuments.Where(e => e.WorkerProfileId == profileId)
            .Select(s => new CovenantFileModel
            {
                Id = s.Id,
                Description = s.Document.Description,
                FileName = s.Document.FileName,
                PathFile = string.Concat(filesConfiguration.FilesPath, s.Document.FileName)
            }).ToListAsync();

    public Task<WorkerProfileBasicInfoModel> GetWorkerProfileBasicInfo(Guid workerProfileId) =>
        (from wp in _context.WorkerProfile.Where(p => p.Id == workerProfileId)
         join cf in _context.CovenantFile on wp.ProfileImageId equals cf.Id into tmp
         from cf in tmp.DefaultIfEmpty()
         select new WorkerProfileBasicInfoModel
         {
             NumberId = wp.NumberId,
             Id = wp.Id,
             FirstName = wp.FirstName,
             MiddleName = wp.MiddleName,
             LastName = wp.LastName,
             SecondLastName = wp.SecondLastName,
             ApprovedToWork = wp.ApprovedToWork,
             ProfileImage = cf == null ? null : new CovenantFileModel
             {
                 Id = cf.Id,
                 FileName = cf.FileName,
                 PathFile = string.Concat(filesConfiguration.FilesPath, cf.FileName)
             },
             HasSocialInsurance = !string.IsNullOrEmpty(wp.SocialInsurance),
             HasSocialInsuranceFile = wp.SocialInsuranceFileId != null,
             HasIdentificationNumber1 = !string.IsNullOrEmpty(wp.IdentificationNumber1),
             HasIdentificationType1File = wp.IdentificationType1FileId != null,
             HasIdentificationNumber2 = !string.IsNullOrEmpty(wp.IdentificationNumber2),
             HasIdentificationType2File = wp.IdentificationType2FileId != null,
             HasResume = wp.ResumeId != null,
             PunchCardId = wp.PunchCardId == null ? wp.WorkerId.ToString() : wp.PunchCardId,
         }).FirstOrDefaultAsync();

    public async Task<List<AgencyWorkerDropdownModel>> GetWorkerProfilesDropdown(IEnumerable<Guid> agencyIds, string searchTerm)
    {
        var workerProfiles = _context.WorkerProfile.Where(wp => agencyIds.Contains(wp.AgencyId));
        var query = workerProfiles
            .Select(wp => new AgencyWorkerDropdownModel
            {
                Id = wp.WorkerId,
                WorkerProfileId = wp.Id,
                SocialInsurance = wp.SocialInsurance,
                FullName = wp.FirstName +
                    (string.IsNullOrWhiteSpace(wp.MiddleName) ? string.Empty : " " + wp.MiddleName) +
                    " " + wp.LastName +
                    (string.IsNullOrWhiteSpace(wp.SecondLastName) ? string.Empty : " " + wp.SecondLastName),
                ApprovedToWork = wp.ApprovedToWork
            });
        var predicate = PredicateBuilder.New<AgencyWorkerDropdownModel>(true);
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            predicate = predicate.And(p => 
                EF.Functions.Like(p.FullName.ToLower(), $"%{searchTerm}%") ||
                EF.Functions.Like(p.SocialInsurance, $"%{searchTerm}%"));
        }
        query = query.Where(predicate);
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<PaginatedList<WorkerProfileListModel>> GetWorkersProfile(Guid agencyId, GetWorkerProfileFilter filter)
    {
        var query = GetAllWorkersProfile(agencyId, filter);
        return await query.ToPaginatedList(filter);
    }

    public IEnumerable<WorkerProfileListModel> GetAllWorkersProfile(Guid agencyId, GetWorkerProfileFilter filter)
    {
        var workers = _context.WorkerProfile
            .Include(wp => wp.Skills)
            .Include(wp => wp.Worker)
            .Include(wp => wp.Notes)
            .AsQueryable();
        var workerRequest = _context.WorkerRequest.Include(wr => wr.Request)
            .Where(wr => wr.WorkerRequestStatus == WorkerRequestStatus.Booked && wr.Request.Status == RequestStatus.InProcess);
        if (filter.SortBy == GetWorkersProfileSortBy.RequestId)
            workerRequest = workerRequest.AddOrderBy(filter, wr => wr.Request.NumberId);
        if (filter.CompanyProfileId.HasValue)
        {
            workers = from wp in workers
                      from wr in workerRequest.Where(wr => wr.WorkerId == wp.WorkerId).Take(1)
                      join c in _context.CompanyProfile.Where(cp => cp.Id == filter.CompanyProfileId) on wr.Request.CompanyId equals c.CompanyId
                      select wp;
        }
        if (!string.IsNullOrWhiteSpace(filter.Skills))
            workers = workers.Where(w => w.Skills.Any(s => s.Skill.ToLower().Contains(filter.Skills.ToLower())));
        var query = from wp in workers
                    join cf in _context.CovenantFile on wp.ProfileImageId equals cf.Id into tmp
                    from cft in tmp.DefaultIfEmpty()
                    join l in _context.Location on wp.LocationId equals l.Id
                    select new WorkerProfileListModel
                    {
                        AgencyId = wp.AgencyId,
                        Id = wp.Id,
                        Address = l.Address + " " + l.City.Value + ", " + l.City.Province.Code + " " + l.PostalCode,
                        WorkerId = wp.WorkerId,
                        FullName =
                            wp.FirstName +
                            (string.IsNullOrWhiteSpace(wp.MiddleName) ? string.Empty : " " + wp.MiddleName) +
                            " " + wp.LastName +
                            (string.IsNullOrWhiteSpace(wp.SecondLastName) ? string.Empty : " " + wp.SecondLastName),
                        Email = wp.Worker.Email,
                        MobileNumber = wp.MobileNumber,
                        NumberId = wp.NumberId,
                        ApprovedToWork = wp.ApprovedToWork,
                        IsSubcontractor = wp.IsSubcontractor,
                        ProfileImage = cft == null ? null : $"{filesConfiguration.FilesPath}{cft.FileName}",
                        Skills = wp.Skills.Where(s => !string.IsNullOrWhiteSpace(s.Skill)).OrderBy(s => s.Skill).Select(s => s.Skill),
                        IsCurrentlyWorking = workerRequest.Any(wr => wr.WorkerId == wp.WorkerId),
                        Requests = workerRequest.Where(wr => wr.WorkerId == wp.WorkerId).Select(wr => new BaseModel<Guid> { Id = wr.RequestId, Value = wr.Request.NumberId.ToString() }),
                        Dnu = wp.Dnu,
                        CreatedAt = wp.CreatedAt,
                        SinNumber = wp.SocialInsurance
                    };
        var predicateNew = ApplyFilterWorkersProfile(agencyId, filter);
        query = query.Where(predicateNew);
        query = ApplySortWorkersProfile(query, filter);
        return query;
    }

    private Expression<Func<WorkerProfileListModel, bool>> ApplyFilterWorkersProfile(Guid agencyId, GetWorkerProfileFilter filter)
    {
        Expression<Func<WorkerProfileListModel, bool>> predicate = wp => wp.AgencyId == agencyId;
        if (filter.ApprovedToWork)
            predicate = predicate.And(wp => wp.ApprovedToWork);
        if (filter.IsSubcontractor.HasValue)
            predicate = predicate.And(wp => wp.IsSubcontractor == filter.IsSubcontractor);
        if (filter.NumberId.HasValue)
            predicate = predicate.And(wp => wp.NumberId == filter.NumberId);
        if (!string.IsNullOrWhiteSpace(filter.FullName))
        {
            var fullName = filter.FullName.ToLower();
            predicate = predicate.And(wp =>
                EF.Functions.Like(wp.FullName.ToLower(), $"%{fullName}%") ||
                EF.Functions.Like(wp.Email.ToLower(), $"%{fullName}%"));
        }
        if (!string.IsNullOrWhiteSpace(filter.Phone))
        {
            var expression = new Regex(@"\s+");
            var phoneWithoutBlankSpaces = expression.Replace(filter.Phone, string.Empty);
            predicate = predicate.And(wp =>
                wp.MobileNumber.Replace("(", string.Empty).Replace(")", string.Empty).Contains(filter.Phone) ||
                wp.MobileNumber.Contains(phoneWithoutBlankSpaces));
        }
        if (!string.IsNullOrWhiteSpace(filter.RequestId))
            predicate = predicate.And(wp => wp.Requests.Any(r => r.Value == filter.RequestId));
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(wp => wp.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && wp.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
        if (filter.Features != null && filter.Features.Any())
        {
            if (filter.Features.Any(f => f == WorkersProfileFeature.Working))
                predicate = predicate.And(wp => wp.IsCurrentlyWorking == true);
            if (filter.Features.Any(f => f == WorkersProfileFeature.NotWorking))
                predicate = predicate.And(wp => wp.IsCurrentlyWorking == false);
            if (filter.Features.Any(f => f == WorkersProfileFeature.Dnu))
                predicate = predicate.And(wp => wp.Dnu == true);
            if (filter.Features.Any(f => f == WorkersProfileFeature.ApprovedToWork))
                predicate = predicate.And(wp => wp.ApprovedToWork);
            if (filter.Features.Any(f => f == WorkersProfileFeature.Subcontractor))
                predicate = predicate.And(wp => wp.IsSubcontractor);
        }
        return predicate;
    }

    private IQueryable<WorkerProfileListModel> ApplySortWorkersProfile(IQueryable<WorkerProfileListModel> query, GetWorkerProfileFilter filter)
    {
        switch (filter.SortBy)
        {
            case GetWorkersProfileSortBy.Name:
                query = query.AddOrderBy(filter, wp => wp.FullName);
                break;
            case GetWorkersProfileSortBy.NumberId:
                query = query.AddOrderBy(filter, wp => wp.NumberId);
                break;
            case GetWorkersProfileSortBy.RequestId:
                query = query.AddOrderBy(filter, wp => wp.Requests.Any() ? wp.Requests.FirstOrDefault().Value : null);
                break;
            case GetWorkersProfileSortBy.CreatedAt:
                query = query.AddOrderBy(filter, wp => wp.CreatedAt);
                break;
            case GetWorkersProfileSortBy.Skills:
                query = query.AddOrderBy(filter, wp => wp.Skills.Any() ? wp.Skills.FirstOrDefault() : null);
                break;
        }
        return query;
    }

    public async Task<bool> InfoIsAlreadyTaken(Expression<Func<WorkerProfile, bool>> expression)
    {
        var result = await _context.WorkerProfile.AnyAsync(expression);
        return result;
    }

    public Task<PaginatedList<WorkerProfileNoteListModel>> GetWorkerProfileNotes(Guid workerProfileId, Pagination pagination) =>
        _context.WorkerProfileNote.Where(e => e.WorkerProfileId == workerProfileId)
            .Select(e => new WorkerProfileNoteListModel
            {
                Note = e.Note,
                CreatedBy = e.CreatedBy,
                CreatedAt = e.CreatedAt
            }).OrderByDescending(o => o.CreatedAt)
            .AsNoTracking().ToPaginatedList(pagination);

    public async Task Create<T>(T entity) where T : class => await _context.AddAsync(entity);

    public void Delete<T>(T entity) where T : class => _context.Set<T>().Remove(entity);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task CreateWorkerProfileHoliday(WorkerProfileHoliday entity)
    {
        var workerProfileHoliday = await _context.WorkerProfileHoliday
            .SingleOrDefaultAsync(h => h.WorkerProfileId == entity.WorkerProfileId && h.HolidayId == entity.HolidayId);
        if (workerProfileHoliday is null)
        {
            await Create(entity);
            return;
        }
        workerProfileHoliday.UpdateStats(entity.StatPaidWorker);
        _context.Update(workerProfileHoliday);
    }

    public async Task<List<WorkerProfileHolidayModel>> GetWorkerProfileHoliday(Guid workerProfileId)
    {
        var previousMonth = DateTime.Now.AddMonths(-1);
        var query = (from h in _context.Holiday.Where(h => h.Date >= previousMonth)
                     join wp in _context.WorkerProfileHoliday.Where(w => w.WorkerProfileId == workerProfileId) on h.Id equals wp.HolidayId into tmp
                     from wp in tmp.DefaultIfEmpty()
                     where _context.WorkerProfile.Any(w => w.Location.City.Province.Country.Code == h.CountryCode && w.Id == workerProfileId)
                     orderby h.Date descending
                     select new WorkerProfileHolidayModel
                     {
                         HolidayId = h.Id,
                         Date = h.Date,
                         StatPaidWorker = wp == null ? 0 : wp.StatPaidWorker
                     });
        return await query.ToListAsync();
    }

    public Task<WorkerProfilePunchCardIdModel> GetWorkerProfilePunchCarId(string punchCardId) =>
        GetWorkerProfilePunchCarId(wp => EF.Functions.ILike(wp.PunchCardId, punchCardId));

    public Task<WorkerProfilePunchCardIdModel> GetWorkerProfilePunchCarId(Guid profileId) =>
        GetWorkerProfilePunchCarId(wp => wp.Id == profileId);

    private Task<WorkerProfilePunchCardIdModel> GetWorkerProfilePunchCarId(Expression<Func<WorkerProfile, bool>> condition) =>
        _context.WorkerProfile.Where(condition)
            .Select(wp => new WorkerProfilePunchCardIdModel { Id = wp.Id, PunchCardId = wp.PunchCardId, WorkerFullName = $"{wp.FirstName} {wp.MiddleName} {wp.LastName} {wp.SecondLastName}" })
            .AsNoTracking()
            .SingleOrDefaultAsync();

    public async Task<PaginatedList<PayStubHistoryModel>> GetWageHistory(Guid workerProfileId, Pagination pagination)
    {
        var payStubs = await _context.PayStubHistories
            .Where(ps => ps.WorkerProfileId == workerProfileId)
            .OrderByDescending(ps => ps.NumberId)
            .Select(ps => new PayStubHistoryModel
            {
                RowNumber = ps.RowNumber,
                Id = ps.Id,
                PayStubNumber = ps.PayStubNumber,
                WeekEnding = ps.WeekEnding,
                TotalEarnings = ps.TotalEarnings,
                Vacations = ps.Vacations,
                PublicHolidays = ps.PublicHolidayPay,
                TotalPaid = ps.TotalPaid,
                Start = ps.DateWorkBegins,
                End = ps.DateWorkEnd
            })
            .ToPaginatedList(pagination);
        if (payStubs.Items.Any())
        {
            var guids = payStubs.Items.Select(arg => arg.Id).ToList();
            var companies = await (from psw in _context.PayStubWageDetail.Where(d => guids.Any(psId => d.PayStubId == psId))
                                   join tst in _context.TimeSheetTotalPayroll on psw.TimeSheetTotalId equals tst.Id
                                   join ts in _context.TimeSheet on tst.TimeSheetId equals ts.Id
                                   join wr in _context.WorkerRequest on ts.WorkerRequestId equals wr.Id
                                   join r in _context.Request on wr.RequestId equals r.Id
                                   join cp in _context.CompanyProfile on r.CompanyId equals cp.CompanyId
                                   select new { psw.PayStubId, cp.FullName }).Distinct().ToListAsync();
            var payStubItem = await _context.PayStubItem.Where(psi => guids.Contains(psi.PayStubId)).ToListAsync();
            foreach (var stub in payStubs.Items)
            {
                stub.Companies = companies.Where(p => p.PayStubId == stub.Id).Select(p => p.FullName).ToList();
                stub.Items = payStubItem.Where(psi => psi.PayStubId == stub.Id).Select(item => new PayStubItemHistoryModel
                {
                    Description = item.Description,
                    Quantity = item.Quantity,
                    Total = item.Total
                });
            }
        }
        return payStubs;
    }

    public async Task<PayStubHistoryAccumulated> GetWageHistoryAccumulated(Guid workerProfileId, int rowNumber)
    {
        var payStubs = await _context.PayStubHistories
            .Where(ps => ps.WorkerProfileId == workerProfileId && ps.RowNumber <= rowNumber)
            .ToListAsync();
        var result = new PayStubHistoryAccumulated
        {
            PublicHolidays = payStubs.Sum(ps => ps.PublicHolidayPay),
            Vacations = payStubs.Sum(ps => ps.Vacations),
            TotalEarnings = payStubs.Sum(ps => ps.TotalEarnings),
            TotalPaid = payStubs.Sum(ps => ps.TotalPaid)
        };
        var guids = payStubs.Select(arg => arg.Id).ToList();
        result.Quantity = await _context.PayStubItem.Where(psi => guids.Contains(psi.PayStubId)).SumAsync(psi => psi.Quantity);
        result.Total = await _context.PayStubItem.Where(psi => guids.Contains(psi.PayStubId)).SumAsync(psi => psi.Total);
        return result;
    }

    public Task<PaginatedList<WorkerContactInfoModel>> GetWorkersAvailableToInvite(AvailableToInvitePagination pagination)
    {
        var workerProfiles = _context.WorkerProfile
            .Where(wO => wO.Dnu == false)
            .Where(wp => wp.AgencyId == pagination.AgencyId);
        return (from wp in workerProfiles
                join u in _context.User.Where(uW =>
                           !EF.Functions.ILike(uW.Email, "%sigook%") &&
                           !EF.Functions.ILike(uW.Email, "%covenant%"))
                       on wp.WorkerId equals u.Id
                join unt in _context.UserNotificationType.Where(uW =>
                        uW.NotificationTypeId == NotificationType.NewRequestNotifyWorker.Id && uW.EmailNotification)
                    on u.Id equals unt.UserId
                where !_context.WorkerRequest.Any(aR => aR.WorkerId == wp.WorkerId &&
                                                        aR.WorkerRequestStatus == WorkerRequestStatus.Booked)
                orderby wp.NumberId
                select new WorkerContactInfoModel
                {
                    WorkerId = wp.WorkerId,
                    FirstName = wp.FirstName,
                    LastName = wp.LastName,
                    Email = u.Email,
                }).ToPaginatedList(pagination);
    }

    public async Task<WorkerProfileOtherDocument> GetOtherDocument(Guid otherDocumentId) => await _context.WorkerProfileOtherDocuments.FirstOrDefaultAsync(wpod => wpod.Id == otherDocumentId);

    public async Task<WorkerProfileLicense> GetLicense(Guid licenseId) => await _context.WorkerProfileLicenses.FirstOrDefaultAsync(wpl => wpl.Id == licenseId);

    public async Task<WorkerProfileCertificate> GetCertificate(Guid certificateId) => await _context.WorkerProfileCertificates.FirstOrDefaultAsync(wpc => wpc.Id == certificateId);

    public async Task<IEnumerable<WorkerSINExpiredModel>> GetWorkersSinExpired(DateTime date)
    {
        var query = from wp in _context.WorkerProfile.Where(p => p.ApprovedToWork && p.SocialInsuranceExpire && p.DueDate < date)
                    join wu in _context.User on wp.WorkerId equals wu.Id
                    join au in _context.User on wp.AgencyId equals au.Id
                    orderby wp.DueDate
                    select new WorkerSINExpiredModel
                    {
                        WorkerFullName = $"{wp.FirstName} {wp.MiddleName} {wp.LastName} {wp.SecondLastName}",
                        WorkerEmail = wu.Email,
                        SocialInsurance = wp.SocialInsurance,
                        DueDate = wp.DueDate,
                        Phone = wp.Phone,
                        PhoneExt = wp.PhoneExt,
                        MobileNumber = wp.MobileNumber,
                        AgencyEmail = au.Email,
                        RecruitmentEmail = wp.Agency.RecruitmentEmail
                    };
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<IEnumerable<WorkerLicenseExpiredModel>> GetWorkerLicensesExpired(DateTime date)
    {
        var query = from wpl in _context.WorkerProfileLicenses.Where(lW => lW.Expires < date)
                    join cf in _context.CovenantFile on wpl.LicenseId equals cf.Id
                    join wp in _context.WorkerProfile.Where(wp => wp.ApprovedToWork) on wpl.WorkerProfileId equals wp.Id
                    join wu in _context.User on wp.WorkerId equals wu.Id
                    join au in _context.User on wp.AgencyId equals au.Id
                    orderby wpl.Expires
                    select new WorkerLicenseExpiredModel
                    {
                        NumberId = wp.NumberId,
                        WorkerFullName = wp.FirstName + " " + wp.MiddleName + " " + wp.LastName + " " + wp.SecondLastName,
                        WorkerEmail = wu.Email,
                        MobileNumber = wp.MobileNumber,
                        LicenseDescription = cf.Description,
                        LicenseNumber = wpl.Number,
                        Expires = wpl.Expires,
                        AgencyEmail = au.Email,
                        RecruitmentEmail = wp.Agency.RecruitmentEmail
                    };
        var result = await query.ToListAsync();
        return result;
    }

    public Task<WorkerProfileTaxCategory> GetWorkerProfileTaxCategory(Guid workerProfileId)
    {
        var category = _context.WorkerProfileTaxCategories
            .FirstOrDefaultAsync(wptc => wptc.WorkerProfileId == workerProfileId);
        return category;
    }
}