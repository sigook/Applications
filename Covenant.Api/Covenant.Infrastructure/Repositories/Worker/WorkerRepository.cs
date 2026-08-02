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
using Covenant.Infrastructure.Contexts;
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

    public Task UpdateProfile(WorkerProfile entity) => Task.FromResult(_context.WorkerProfiles.Update(entity));

    public Task<WorkerProfile> GetProfile(Expression<Func<WorkerProfile, bool>> condition) =>
        _context.WorkerProfiles.Where(condition)
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

    public Task<WorkerProfileDetailModel> GetWorkerProfileDetail(Expression<Func<WorkerProfile, bool>> condition)
    {
        return (from wp in _context.WorkerProfiles.Where(condition)
                select new WorkerProfileDetailModel
                {
                    Id = wp.Id,
                    NumberId = wp.NumberId,
                    ProfileImage = wp.ProfileImage == null
                        ? new CovenantFileModel
                        {
                            FileName = "worker.png",
                            PathFile = string.Concat(filesConfiguration.FilesPath, "worker.png")
                        }
                        : new CovenantFileModel
                        {
                            Id = wp.ProfileImage.Id,
                            Description = wp.ProfileImage.Description,
                            FileName = wp.ProfileImage.FileName,
                            PathFile = string.Concat(filesConfiguration.FilesPath, wp.ProfileImage.FileName)
                        },
                    FirstName = wp.FirstName,
                    MiddleName = wp.MiddleName,
                    LastName = wp.LastName,
                    SecondLastName = wp.SecondLastName,
                    BirthDay = wp.BirthDay,
                    Gender = wp.Gender == null ? null : new BaseModel<Guid> { Id = wp.Gender.Id, Value = wp.Gender.Value },
                    SocialInsurance = wp.SocialInsurance,
                    SocialInsuranceExpire = wp.SocialInsuranceExpire,
                    DueDate = wp.DueDate,
                    SocialInsuranceFile = wp.SocialInsuranceFile == null
                        ? null
                        : new CovenantFileModel
                        {
                            Id = wp.SocialInsuranceFile.Id,
                            Description = wp.SocialInsuranceFile.Description,
                            FileName = wp.SocialInsuranceFile.FileName,
                            PathFile = string.Concat(filesConfiguration.FilesPath, wp.SocialInsuranceFile.FileName)
                        },
                    IdentificationNumber1 = wp.IdentificationNumber1,
                    IdentificationNumber2 = wp.IdentificationNumber2,
                    HavePoliceCheckBackground = wp.HavePoliceCheckBackground,
                    IdentificationType1File = wp.IdentificationType1File == null
                        ? null
                        : new CovenantFileModel
                        {
                            Id = wp.IdentificationType1File.Id,
                            Description = wp.IdentificationType1File.Description,
                            FileName = wp.IdentificationType1File.FileName,
                            PathFile = string.Concat(filesConfiguration.FilesPath, wp.IdentificationType1File.FileName)
                        },
                    IdentificationType2File = wp.IdentificationType2File == null
                        ? null
                        : new CovenantFileModel
                        {
                            Id = wp.IdentificationType2File.Id,
                            Description = wp.IdentificationType2File.Description,
                            FileName = wp.IdentificationType2File.FileName,
                            PathFile = string.Concat(filesConfiguration.FilesPath, wp.IdentificationType2File.FileName)
                        },
                    IdentificationType1 = wp.IdentificationType1 == null ? null : new BaseModel<Guid> { Id = wp.IdentificationType1.Id, Value = wp.IdentificationType1.Value },
                    IdentificationType2 = wp.IdentificationType2 == null ? null : new BaseModel<Guid> { Id = wp.IdentificationType2.Id, Value = wp.IdentificationType2.Value },
                    PoliceCheckBackGround = wp.PoliceCheckBackGround == null
                        ? null
                        : new CovenantFileModel
                        {
                            Id = wp.PoliceCheckBackGround.Id,
                            Description = wp.PoliceCheckBackGround.Description,
                            FileName = wp.PoliceCheckBackGround.FileName,
                            PathFile = string.Concat(filesConfiguration.FilesPath, wp.PoliceCheckBackGround.FileName)
                        },
                    MobileNumber = wp.MobileNumber,
                    Phone = wp.Phone,
                    PhoneExt = wp.PhoneExt,
                    Location = wp.Location == null
                        ? null
                        : new LocationDetailModel
                        {
                            Id = wp.Location.Id,
                            Address = wp.Location.Address,
                            Latitude = wp.Location.Latitude,
                            Longitude = wp.Location.Longitude,
                            PostalCode = wp.Location.PostalCode,
                            City = new CityModel
                            {
                                Id = wp.Location.City.Id,
                                Value = wp.Location.City.Value,
                                Code = wp.Location.City.Code,
                                Province = new ProvinceModel
                                {
                                    Id = wp.Location.City.Province.Id,
                                    Value = wp.Location.City.Province.Value,
                                    Code = wp.Location.City.Province.Code,
                                    Country = new CountryModel
                                    {
                                        Id = wp.Location.City.Province.Country.Id,
                                        Value = wp.Location.City.Province.Country.Value,
                                        Code = wp.Location.City.Province.Country.Code
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
                    Lift = wp.Lift == null ? null : new BaseModel<Guid> { Id = wp.Lift.Id, Value = wp.Lift.Value },
                    Languages = wp.Languages.Select(l => new BaseModel<Guid>
                    {
                        Id = l.Language.Id,
                        Value = l.Language.Value
                    }),
                    Skills = wp.Skills.Select(skill => new SkillModel { Id = skill.Id, Skill = skill.Skill }).ToList(),
                    Resume = wp.Resume == null
                        ? null
                        : new CovenantFileModel
                        {
                            Id = wp.Resume.Id,
                            Description = wp.Resume.Description,
                            FileName = wp.Resume.FileName,
                            PathFile = string.Concat(filesConfiguration.FilesPath, wp.Resume.FileName)
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
                    Email = wp.Worker.Email,
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
                    Ei = wp.WorkerProfileTaxCategory != null ? wp.WorkerProfileTaxCategory.Ei : null,
                    ExternalId = wp.ExternalId,
                    WcCode = wp.WcCode
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
        (from wp in _context.WorkerProfiles.Where(p => p.Id == workerProfileId)
         select new WorkerProfileBasicInfoModel
         {
             NumberId = wp.NumberId,
             Id = wp.Id,
             FirstName = wp.FirstName,
             MiddleName = wp.MiddleName,
             LastName = wp.LastName,
             SecondLastName = wp.SecondLastName,
             ApprovedToWork = wp.ApprovedToWork,
             ProfileImage = wp.ProfileImage == null ? null : new CovenantFileModel
             {
                 Id = wp.ProfileImage.Id,
                 FileName = wp.ProfileImage.FileName,
                 PathFile = string.Concat(filesConfiguration.FilesPath, wp.ProfileImage.FileName)
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
        var workerProfiles = _context.WorkerProfiles.Where(wp => agencyIds.Contains(wp.AgencyId));
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
                Email = wp.Worker.Email,
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
        var workers = _context.WorkerProfiles
            .Include(wp => wp.Skills)
            .Include(wp => wp.Worker)
            .Include(wp => wp.Notes)
            .AsQueryable();
        var workerRequest = _context.WorkerRequests.Include(wr => wr.Request)
            .Where(wr => wr.WorkerRequestStatus == WorkerRequestStatus.Booked && wr.Request.Status != RequestStatus.Cancelled);
        if (filter.SortBy == GetWorkersProfileSortBy.RequestId)
            workerRequest = workerRequest.AddOrderBy(filter, wr => wr.Request.NumberId);
        if (filter.CompanyProfileId.HasValue)
        {
            workers = from wp in workers
                      from wr in workerRequest.Where(wr => wr.WorkerProfileId == wp.Id).Take(1)
                      where wr.Request.CompanyProfileId == filter.CompanyProfileId
                      select wp;
        }
        if (!string.IsNullOrWhiteSpace(filter.Skills))
            workers = workers.Where(w => w.Skills.Any(s => s.Skill.ToLower().Contains(filter.Skills.ToLower())));
        var query = from wp in workers
                    select new WorkerProfileListModel
                    {
                        AgencyId = wp.AgencyId,
                        Id = wp.Id,
                        Address = wp.Location.Address + " " + wp.Location.City.Value + ", " + wp.Location.City.Province.Code + " " + wp.Location.PostalCode,
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
                        ProfileImage = wp.ProfileImage == null ? null : $"{filesConfiguration.FilesPath}{wp.ProfileImage.FileName}",
                        Skills = wp.Skills.Where(s => !string.IsNullOrWhiteSpace(s.Skill)).OrderBy(s => s.Skill).Select(s => s.Skill),
                        IsCurrentlyWorking = workerRequest.Any(wr => wr.WorkerProfileId == wp.Id),
                        Requests = workerRequest.Where(wr => wr.WorkerProfileId == wp.Id).Select(wr => new BaseModel<Guid> { Id = wr.RequestId, Value = wr.Request.NumberId.ToString() }),
                        Dnu = wp.Dnu,
                        CreatedAt = wp.CreatedAt,
                        SinNumber = wp.SocialInsurance,
                        ExternalId = wp.ExternalId
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
        if (!string.IsNullOrWhiteSpace(filter.ExternalId))
        {
            var externalId = filter.ExternalId.ToLower();
            predicate = predicate.And(wp => EF.Functions.Like(wp.ExternalId.ToLower(), $"%{externalId}%"));
        }
        if (!string.IsNullOrWhiteSpace(filter.Location))
        {
            var location = filter.Location.ToLower();
            predicate = predicate.And(wp => EF.Functions.Like(wp.Address.ToLower(), $"%{location}%"));
        }
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
            case GetWorkersProfileSortBy.ExternalId:
                query = query.AddOrderBy(filter, wp => wp.ExternalId);
                break;
        }
        return query;
    }

    public async Task<bool> InfoIsAlreadyTaken(Expression<Func<WorkerProfile, bool>> expression)
    {
        var result = await _context.WorkerProfiles.AnyAsync(expression);
        return result;
    }

    public Task<PaginatedList<WorkerProfileNoteListModel>> GetWorkerProfileNotes(Guid workerProfileId, Pagination pagination) =>
        _context.WorkerProfileNotes.Where(e => e.WorkerProfileId == workerProfileId)
            .Select(e => new WorkerProfileNoteListModel
            {
                Note = e.Note,
                CreatedBy = e.CreatedBy,
                CreatedAt = e.CreatedAt
            }).OrderByDescending(o => o.CreatedAt)
            .AsNoTracking().ToPaginatedList(pagination);

    public Task<PaginatedList<WorkerCommentModel>> GetComments(Expression<Func<WorkerComment, bool>> condition, Pagination pagination) =>
        _context.WorkerComments.Where(condition)
            .Select(c => new WorkerCommentModel
            {
                Id = c.Id,
                Comment = c.Comment,
                Rate = c.Rate,
                NumberId = c.NumberId,
                CreatedAt = c.CreatedAt
            })
            .OrderByDescending(c => c.CreatedAt)
            .AsNoTracking().ToPaginatedList(pagination);

    public async Task Create<T>(T entity) where T : class => await _context.AddAsync(entity);

    public void Delete<T>(T entity) where T : class => _context.Set<T>().Remove(entity);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task CreateWorkerProfileHoliday(WorkerProfileHoliday entity)
    {
        var workerProfileHoliday = await _context.WorkerProfileHolidays
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
        var query = (from h in _context.Holidays.Where(h => h.Date >= previousMonth)
                     join wp in _context.WorkerProfileHolidays.Where(w => w.WorkerProfileId == workerProfileId) on h.Id equals wp.HolidayId into tmp
                     from wp in tmp.DefaultIfEmpty()
                     where _context.WorkerProfiles.Any(w => w.Location.City.Province.Country.Code == h.CountryCode && w.Id == workerProfileId)
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
        _context.WorkerProfiles.Where(condition)
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
                TotalPaid = ps.TotalPaid,
                Start = ps.DateWorkBegins,
                End = ps.DateWorkEnd
            })
            .ToPaginatedList(pagination);
        if (payStubs.Items.Any())
        {
            var guids = payStubs.Items.Select(arg => arg.Id).ToList();
            var companies = await (from psw in _context.PayStubWageDetails.Where(d => guids.Any(psId => d.PayStubId == psId))
                                   select new
                                   {
                                       psw.PayStubId,
                                       psw.TimeSheetTotal.TimeSheet.WorkerRequest.Request.CompanyProfile.FullName
                                   }).Distinct().ToListAsync();
            var payStubItem = await _context.PayStubItems.Where(psi => guids.Contains(psi.PayStubId)).ToListAsync();
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
            Vacations = payStubs.Sum(ps => ps.Vacations),
            TotalEarnings = payStubs.Sum(ps => ps.TotalEarnings),
            TotalPaid = payStubs.Sum(ps => ps.TotalPaid)
        };
        var guids = payStubs.Select(arg => arg.Id).ToList();
        result.Quantity = await _context.PayStubItems.Where(psi => guids.Contains(psi.PayStubId)).SumAsync(psi => psi.Quantity);
        result.Total = await _context.PayStubItems.Where(psi => guids.Contains(psi.PayStubId)).SumAsync(psi => psi.Total);
        return result;
    }

    public Task<List<WorkerContactInfoModel>> GetWorkersAvailableToInvite(Guid agencyId, Guid provinceId)
    {
        var eligibleProfiles = _context.WorkerProfiles
            .Where(wp => !wp.Dnu)
            .Where(wp => wp.AgencyId == agencyId)
            .Where(wp => wp.Location.City.ProvinceId == provinceId)
            .Where(wp => !string.IsNullOrEmpty(wp.FirstName));

        var contactableUsers = _context.Users
            .Where(u => !string.IsNullOrEmpty(u.Email))
            .Where(u => u.Email.Contains("@"));

        var subscribedUserIds = _context.UserNotificationTypes
            .Where(unt => unt.NotificationTypeId == NotificationType.NewRequestNotifyWorker.Id)
            .Where(unt => unt.EmailNotification)
            .Select(unt => unt.UserId);

        var bookedWorkerProfileIds = _context.WorkerRequests
            .Where(wr => wr.WorkerRequestStatus == WorkerRequestStatus.Booked)
            .Select(wr => wr.WorkerProfileId);

        return eligibleProfiles
            .Join(contactableUsers, wp => wp.WorkerId, u => u.Id, (wp, u) => new { wp, u })
            .Where(x => subscribedUserIds.Contains(x.u.Id))
            .Where(x => !bookedWorkerProfileIds.Contains(x.wp.Id))
            .OrderBy(x => x.wp.NumberId)
            .Select(x => new WorkerContactInfoModel
            {
                WorkerId = x.wp.WorkerId,
                FirstName = x.wp.FirstName,
                LastName = x.wp.LastName,
                Email = x.u.Email,
            })
            .ToListAsync();
    }

    public async Task<WorkerProfileOtherDocument> GetOtherDocument(Guid otherDocumentId) => await _context.WorkerProfileOtherDocuments.FirstOrDefaultAsync(wpod => wpod.Id == otherDocumentId);

    public async Task<WorkerProfileLicense> GetLicense(Guid licenseId) => await _context.WorkerProfileLicenses.FirstOrDefaultAsync(wpl => wpl.Id == licenseId);

    public async Task<WorkerProfileCertificate> GetCertificate(Guid certificateId) => await _context.WorkerProfileCertificates.FirstOrDefaultAsync(wpc => wpc.Id == certificateId);

    public async Task<IEnumerable<WorkerSINExpiredModel>> GetWorkersSinExpired(DateTime date)
    {
        var query = from wp in _context.WorkerProfiles.Where(p => p.ApprovedToWork && p.SocialInsuranceExpire && p.DueDate < date)
                    orderby wp.DueDate
                    select new WorkerSINExpiredModel
                    {
                        WorkerFullName = $"{wp.FirstName} {wp.MiddleName} {wp.LastName} {wp.SecondLastName}",
                        WorkerEmail = wp.Worker.Email,
                        SocialInsurance = wp.SocialInsurance,
                        DueDate = wp.DueDate,
                        Phone = wp.Phone,
                        PhoneExt = wp.PhoneExt,
                        MobileNumber = wp.MobileNumber,
                        AgencyEmail = wp.Agency.User.Email,
                        RecruitmentEmail = wp.Agency.RecruitmentEmail
                    };
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<IEnumerable<WorkerLicenseExpiredModel>> GetWorkerLicensesExpired(DateTime date)
    {
        var query = from wpl in _context.WorkerProfileLicenses.Where(lW => lW.Expires < date && lW.WorkerProfile.ApprovedToWork)
                    orderby wpl.Expires
                    select new WorkerLicenseExpiredModel
                    {
                        NumberId = wpl.WorkerProfile.NumberId,
                        WorkerFullName = wpl.WorkerProfile.FirstName + " " + wpl.WorkerProfile.MiddleName + " " + wpl.WorkerProfile.LastName + " " + wpl.WorkerProfile.SecondLastName,
                        WorkerEmail = wpl.WorkerProfile.Worker.Email,
                        MobileNumber = wpl.WorkerProfile.MobileNumber,
                        LicenseDescription = wpl.License.Description,
                        LicenseNumber = wpl.Number,
                        Expires = wpl.Expires,
                        AgencyEmail = wpl.WorkerProfile.Agency.User.Email,
                        RecruitmentEmail = wpl.WorkerProfile.Agency.RecruitmentEmail
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