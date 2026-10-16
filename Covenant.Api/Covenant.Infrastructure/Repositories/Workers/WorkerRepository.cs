using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Worker.Agency;
using Covenant.Common.Models;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Covenant.Infrastructure.Repositories.Workers;

public class WorkerRepository(CovenantContext context, IOptions<FilesConfiguration> options) : BaseRepository<CovenantContext>(context), IWorkerRepository
{
    private readonly FilesConfiguration filesConfiguration = options.Value;

    public Task<WorkerProfile> GetProfile(Expression<Func<WorkerProfile, bool>> condition) =>
        Context.WorkerProfiles.Where(condition)
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
        return (from wp in Context.WorkerProfiles.Where(condition)
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
                    Context.WorkerProfileOtherDocuments.Where(e => e.WorkerProfileId == profileId)
            .Select(s => new CovenantFileModel
            {
                Id = s.Id,
                Description = s.Document.Description,
                FileName = s.Document.FileName,
                PathFile = string.Concat(filesConfiguration.FilesPath, s.Document.FileName)
            }).ToListAsync();

    public Task<WorkerProfileBasicInfoModel> GetWorkerProfileBasicInfo(Guid workerProfileId) =>
        (from wp in Context.WorkerProfiles.Where(p => p.Id == workerProfileId)
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
        var workerProfiles = Context.WorkerProfiles.Where(wp => agencyIds.Contains(wp.AgencyId));
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

    public IQueryable<WorkerProfileListModel> GetAllWorkersProfile(Guid agencyId, GetWorkerProfileFilter filter)
    {
        var workers = Context.WorkerProfiles
            .Include(wp => wp.Skills)
            .Include(wp => wp.Worker)
            .Include(wp => wp.Notes)
            .AsQueryable();
        if (filter.CompanyProfileId.HasValue)
        {
            workers = workers.Where(wp => wp.WorkerRequests.Any(wr =>
                wr.WorkerRequestStatus == WorkerRequestStatus.Booked &&
                wr.Request.Status != RequestStatus.Cancelled &&
                wr.Request.CompanyProfileId == filter.CompanyProfileId));
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
                        IsCurrentlyWorking = wp.WorkerRequests.Any(wr =>
                            wr.WorkerRequestStatus == WorkerRequestStatus.Booked &&
                            wr.Request.Status != RequestStatus.Cancelled),
                        Requests = wp.WorkerRequests
                            .Where(wr => wr.WorkerRequestStatus == WorkerRequestStatus.Booked && wr.Request.Status != RequestStatus.Cancelled)
                            .OrderBy(wr => wr.Request.NumberId)
                            .Select(wr => new BaseModel<Guid> { Id = wr.RequestId, Value = wr.Request.NumberId.ToString() }),
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

    private static IQueryable<WorkerProfileListModel> ApplySortWorkersProfile(IQueryable<WorkerProfileListModel> query, GetWorkerProfileFilter filter)
    {
        var ordered = filter.SortBy switch
        {
            GetWorkersProfileSortBy.Name => query.AddOrderBy(filter, wp => wp.FullName),
            GetWorkersProfileSortBy.NumberId => query.AddOrderBy(filter, wp => wp.NumberId),
            GetWorkersProfileSortBy.RequestId => query.AddOrderBy(filter, wp => wp.Requests.Min(r => r.Value)),
            GetWorkersProfileSortBy.CreatedAt => query.AddOrderBy(filter, wp => wp.CreatedAt),
            GetWorkersProfileSortBy.Skills => query.AddOrderBy(filter, wp => wp.Skills.Any() ? wp.Skills.FirstOrDefault() : null),
            GetWorkersProfileSortBy.ExternalId => query.AddOrderBy(filter, wp => wp.ExternalId),
            _ => query.OrderByDescending(wp => wp.NumberId)
        };
        return ordered.ThenBy(wp => wp.Id);
    }

    public async Task<bool> InfoIsAlreadyTaken(Expression<Func<WorkerProfile, bool>> expression)
    {
        var result = await Context.WorkerProfiles.AnyAsync(expression);
        return result;
    }

    public async Task<bool> SocialInsuranceIsAlreadyTaken(string socialInsurance, Guid? excludeProfileId = null)
    {
        if (string.IsNullOrEmpty(socialInsurance)) return false;
        return await InfoIsAlreadyTaken(x => (excludeProfileId == null || x.Id != excludeProfileId) && x.SocialInsurance == socialInsurance);
    }

    public Task<PaginatedList<WorkerProfileNoteListModel>> GetWorkerProfileNotes(Guid workerProfileId, Pagination pagination) =>
        Context.WorkerProfileNotes.Where(e => e.WorkerProfileId == workerProfileId)
            .Select(e => new WorkerProfileNoteListModel
            {
                Note = e.Note,
                CreatedBy = e.CreatedBy,
                CreatedAt = e.CreatedAt
            }).OrderByDescending(o => o.CreatedAt)
            .AsNoTracking().ToPaginatedList(pagination);

    public Task<PaginatedList<WorkerCommentModel>> GetComments(Expression<Func<WorkerComment, bool>> condition, Pagination pagination) =>
        Context.WorkerComments.Where(condition)
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

    public async Task CreateWorkerProfileHoliday(WorkerProfileHoliday entity)
    {
        var workerProfileHoliday = await Context.WorkerProfileHolidays
            .SingleOrDefaultAsync(h => h.WorkerProfileId == entity.WorkerProfileId && h.HolidayId == entity.HolidayId);
        if (workerProfileHoliday is null)
        {
            await Create(entity);
            return;
        }
        workerProfileHoliday.UpdateStats(entity.StatPaidWorker);
        Context.Update(workerProfileHoliday);
    }

    public async Task<List<WorkerProfileHolidayModel>> GetWorkerProfileHoliday(Guid workerProfileId)
    {
        var previousMonth = DateTime.Now.AddMonths(-1);
        var query = (from h in Context.Holidays.Where(h => h.Date >= previousMonth)
                     join wp in Context.WorkerProfileHolidays.Where(w => w.WorkerProfileId == workerProfileId) on h.Id equals wp.HolidayId into tmp
                     from wp in tmp.DefaultIfEmpty()
                     where Context.WorkerProfiles.Any(w => w.Location.City.Province.Country.Code == h.CountryCode && w.Id == workerProfileId)
                     orderby h.Date descending
                     select new WorkerProfileHolidayModel
                     {
                         HolidayId = h.Id,
                         Date = h.Date,
                         StatPaidWorker = wp == null ? 0 : wp.StatPaidWorker
                     });
        return await query.ToListAsync();
    }

    public async Task<PaginatedList<PayStubHistoryModel>> GetWageHistory(Guid agencyId, Guid workerProfileId, GetWorkerPayStubHistoryFilter filter)
    {
        var query = Context.PayStubs
            .Where(ps => ps.WorkerProfileId == workerProfileId && ps.WorkerProfile.AgencyId == agencyId)
            .Select(ps => new PayStubHistoryModel
            {
                Id = ps.Id,
                NumberId = ps.NumberId,
                PayStubNumber = ps.PayStubNumber,
                WeekEnding = ps.WeekEnding,
                TotalEarnings = ps.TotalEarnings,
                Vacations = ps.Vacations,
                TotalPaid = ps.TotalPaid,
                Start = ps.DateWorkBegins,
                End = ps.DateWorkEnd
            });
        query = query.Where(ApplyFilterWageHistory(filter));
        query = ApplySortWageHistory(query, filter);
        var payStubs = await query.ToPaginatedList(filter);
        if (payStubs.Items.Any())
        {
            await FillWageHistoryAccumulated(workerProfileId, payStubs.Items);
            var guids = payStubs.Items.Select(arg => arg.Id).ToList();
            var companies = await (from psw in Context.PayStubWageDetails.Where(d => guids.Any(psId => d.PayStubId == psId))
                                   select new
                                   {
                                       psw.PayStubId,
                                       psw.TimeSheetTotal.TimeSheet.WorkerRequest.Request.CompanyProfile.FullName
                                   }).Distinct().ToListAsync();
            var payStubItem = await Context.PayStubItems.Where(psi => guids.Contains(psi.PayStubId)).ToListAsync();
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

    private async Task FillWageHistoryAccumulated(Guid workerProfileId, List<PayStubHistoryModel> page)
    {
        var history = await Context.PayStubs
            .Where(p => p.WorkerProfileId == workerProfileId)
            .OrderBy(p => p.NumberId).ThenBy(p => p.Id)
            .Select(p => new PayStubHistoryAmounts(
                p.Id,
                p.Vacations,
                p.TotalEarnings,
                p.TotalPaid,
                p.Items.Sum(i => i.Quantity),
                p.Items.Sum(i => i.Total)))
            .ToListAsync();
        var byId = page.ToDictionary(p => p.Id);
        double quantity = 0;
        decimal total = 0, vacations = 0, totalEarnings = 0, totalPaid = 0;
        foreach (var stub in history)
        {
            quantity += stub.Quantity;
            total += stub.Total;
            vacations += stub.Vacations;
            totalEarnings += stub.TotalEarnings;
            totalPaid += stub.TotalPaid;
            if (!byId.TryGetValue(stub.Id, out var row)) continue;
            row.AccumulatedQuantity = quantity;
            row.AccumulatedTotal = total;
            row.AccumulatedVacations = vacations;
            row.AccumulatedTotalEarnings = totalEarnings;
            row.AccumulatedTotalPaid = totalPaid;
        }
    }

    private sealed record PayStubHistoryAmounts(Guid Id, decimal Vacations, decimal TotalEarnings, decimal TotalPaid, double Quantity, decimal Total);

    private static Expression<Func<PayStubHistoryModel, bool>> ApplyFilterWageHistory(GetWorkerPayStubHistoryFilter filter)
    {
        var predicate = PredicateBuilder.New<PayStubHistoryModel>(true);
        if (!string.IsNullOrWhiteSpace(filter.PayStubNumber))
            predicate = predicate.And(p => p.PayStubNumber.Contains(filter.PayStubNumber));
        if (filter.WeekEndingFrom.HasValue && filter.WeekEndingTo.HasValue)
            predicate = predicate.And(p => p.WeekEnding.Date >= filter.WeekEndingFrom.Value.Date && p.WeekEnding.Date <= filter.WeekEndingTo.Value.Date);
        return predicate;
    }

    private static IQueryable<PayStubHistoryModel> ApplySortWageHistory(IQueryable<PayStubHistoryModel> query, GetWorkerPayStubHistoryFilter filter)
    {
        switch (filter.SortBy)
        {
            case WorkerPayStubHistorySortBy.PayStubNumber:
                return filter.IsDescending
                    ? query.OrderByDescending(p => p.NumberId).ThenByDescending(p => p.Id)
                    : query.OrderBy(p => p.NumberId).ThenBy(p => p.Id);
            case WorkerPayStubHistorySortBy.WeekEnding:
                return query.AddOrderBy(filter, p => p.WeekEnding).ThenBy(p => p.Id);
            case WorkerPayStubHistorySortBy.TotalEarnings:
                return query.AddOrderBy(filter, p => p.TotalEarnings).ThenBy(p => p.Id);
            case WorkerPayStubHistorySortBy.TotalPaid:
                return query.AddOrderBy(filter, p => p.TotalPaid).ThenBy(p => p.Id);
            default:
                return query.OrderByDescending(p => p.NumberId).ThenByDescending(p => p.Id);
        }
    }

    public Task<List<WorkerContactInfoModel>> GetWorkersAvailableToInvite(Guid agencyId, Guid provinceId)
    {
        var eligibleProfiles = Context.WorkerProfiles
            .Where(wp => !wp.Dnu)
            .Where(wp => wp.AgencyId == agencyId)
            .Where(wp => wp.Location.City.ProvinceId == provinceId)
            .Where(wp => !string.IsNullOrEmpty(wp.FirstName));

        var contactableUsers = Context.Users
            .Where(u => !string.IsNullOrEmpty(u.Email))
            .Where(u => u.Email.Contains("@"));

        var subscribedUserIds = Context.UserNotificationTypes
            .Where(unt => unt.NotificationTypeId == NotificationType.NewRequestNotifyWorker.Id)
            .Where(unt => unt.EmailNotification)
            .Select(unt => unt.UserId);

        var bookedWorkerProfileIds = Context.WorkerRequests
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

    public async Task<WorkerProfileOtherDocument> GetOtherDocument(Guid otherDocumentId) => await Context.WorkerProfileOtherDocuments.FirstOrDefaultAsync(wpod => wpod.Id == otherDocumentId);

    public async Task<WorkerProfileLicense> GetLicense(Guid licenseId) => await Context.WorkerProfileLicenses.FirstOrDefaultAsync(wpl => wpl.Id == licenseId);

    public async Task<WorkerProfileCertificate> GetCertificate(Guid certificateId) => await Context.WorkerProfileCertificates.FirstOrDefaultAsync(wpc => wpc.Id == certificateId);

    public async Task<IEnumerable<WorkerSINExpiredModel>> GetWorkersSinExpired(DateTime date)
    {
        var query = from wp in Context.WorkerProfiles.Where(p => p.ApprovedToWork && p.SocialInsuranceExpire && p.DueDate < date)
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
        var query = from wpl in Context.WorkerProfileLicenses.Where(lW => lW.Expires < date && lW.WorkerProfile.ApprovedToWork)
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
        var category = Context.WorkerProfileTaxCategories
            .FirstOrDefaultAsync(wptc => wptc.WorkerProfileId == workerProfileId);
        return category;
    }
}