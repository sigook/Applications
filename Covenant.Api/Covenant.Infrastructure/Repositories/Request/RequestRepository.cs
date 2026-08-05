using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using CandidateEntity = Covenant.Common.Entities.Candidate.Candidate;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Request.WeeklyBoard;
using Covenant.Common.Models.WebSite;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Mappers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Covenant.Infrastructure.Repositories.Request;

public class RequestRepository(CovenantContext context, IOptions<FilesConfiguration> options) : IRequestRepository
{
    private readonly FilesConfiguration filesConfiguration = options.Value;

    public async Task Create<T>(T entity) where T : class => await context.Set<T>().AddAsync(entity);

    public void Delete<T>(T entity) where T : class => context.Set<T>().Remove(entity);

    public Task Update<T>(T entity) where T : class => Task.FromResult(context.Set<T>().Update(entity));

    public IQueryable<AgencyRequestListModel> GetAllRequestsForAgency(Guid agencyId, GetRequestForAgencyFilter filter)
    {
        var requests = context.Requests.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.Recruiter))
        {
            requests = from r in requests
                       join rr in context.RequestRecruiters.Where(c => c.Recruiter.User.Email.ToLower() == filter.Recruiter.ToLower()) on r.Id equals rr.RequestId
                       select r;
        }
        if (filter.CompanyProfileId.HasValue)
            requests = requests.Where(r => r.CompanyProfileId == filter.CompanyProfileId.Value);
        else
            requests = requests.Where(r => r.CompanyProfile.AgencyId == agencyId);
        if (filter.SalesPersonnelId.HasValue)
        {
            requests = requests.Where(r => r.RequestComission != null
                                           && r.RequestComission.AgencyPersonnelId == filter.SalesPersonnelId.Value);
        }
        if (!string.IsNullOrWhiteSpace(filter.DisplayRecruiters))
        {
            var recruiterTerm = filter.DisplayRecruiters.ToLower();
            requests = requests.Where(r => context.RequestRecruiters
                .Any(rr => rr.RequestId == r.Id && rr.Recruiter.Name.ToLower().Contains(recruiterTerm)));
        }
        var query = from r in requests
                    select new AgencyRequestListModel
                    {
                        Id = r.Id,
                        NumberId = r.NumberId,
                        AgencyId = r.CompanyProfile.AgencyId,
                        JobTitle = r.JobTitle,
                        BillingTitle = r.BillingTitle,
                        CreatedAt = r.CreatedAt,
                        Address = r.JobLocation.Address,
                        City = r.JobLocation.City.Value,
                        ProvinceName = r.JobLocation.City.Province.Value,
                        PostalCode = r.JobLocation.PostalCode,
                        Entrance = r.JobLocation.Entrance,
                        CompanyFullName = r.CompanyProfile.FullName,
                        CompanyProfileId = r.CompanyProfileId,
                        RequestStatus = r.Status,
                        IsAsap = r.IsAsap,
                        WorkerRate = r.WorkerSalary.HasValue ? r.WorkerSalary : r.WorkerRate,
                        WorkerSalary = r.WorkerSalary,
                        DisplayRecruiters = string.Join("|", context.RequestRecruiters
                            .Where(rr => rr.RequestId == r.Id)
                            .Select(rr => rr.Recruiter.Name).Distinct()),
                        WorkersQuantity = r.WorkersQuantity,
                        SalesRepresentative = r.RequestComission != null ? r.RequestComission.AgencyPersonnel.Name : null,
                        WorkersQuantityWorking = r.WorkersQuantityWorking,
                        DisplayShift = r.Shift == null ? null : r.Shift.DisplayShift,
                        NotesCount = r.Notes.Count(n => !n.Note.IsDeleted),
                        VaccinationRequired = r.CompanyProfile.VaccinationRequired.GetValueOrDefault(),
                        PunchCardOptionEnabled = r.PunchCardOptionEnabled,
                        HasPermissionToSeeInternalRequests = r.CompanyProfile.RequiresPermissionToSeeRequests,
                        JobBoards = r.Sources.Select(rs => new RequestSourceDetailModel
                        {
                            SourceId = rs.SourceId,
                            Value = rs.Source.Value,
                            PublishedAt = rs.PublishedAt,
                            ExternalUrl = rs.ExternalUrl
                        }),
                    };
        var predicateNew = ApplyFilterForAgency(filter);
        query = query.Where(predicateNew);
        query = ApplySortForAgency(query, filter);
        return query;
    }

    public async Task<PaginatedList<AgencyRequestListModel>> GetRequestsForAgency(Guid agencyId, GetRequestForAgencyFilter filter)
    {
        var query = GetAllRequestsForAgency(agencyId, filter);
        return await query.ToPaginatedList(filter);
    }

    public async Task<IEnumerable<RequestSourceSummaryModel>> GetRequestSourcesSummaryForAgency(Guid agencyId, GetRequestForAgencyFilter filter)
    {
        // Reuse the same filtered query (without paging) and aggregate the JobBoards projection.
        var query = GetAllRequestsForAgency(agencyId, filter);
        var summary = await query
            .SelectMany(r => r.JobBoards)
            .GroupBy(jb => new { jb.SourceId, jb.Value })
            .Select(g => new RequestSourceSummaryModel
            {
                SourceId = g.Key.SourceId,
                Value = g.Key.Value,
                Count = g.Count()
            })
            .ToListAsync();
        return summary;
    }

    public async Task<IEnumerable<RequestSource>> GetRequestSources(Guid requestId)
    {
        return await context.Set<RequestSource>()
            .Include(rs => rs.Source)
            .Where(rs => rs.RequestId == requestId)
            .ToListAsync();
    }

    public async Task ReplaceRequestSources(Guid requestId, IEnumerable<CreateRequestSourceModel> sources)
    {
        var set = context.Set<RequestSource>();
        var existing = await set.Where(rs => rs.RequestId == requestId).ToListAsync();
        set.RemoveRange(existing);

        if (sources == null) return;
        foreach (var model in sources)
        {
            await set.AddAsync(new RequestSource
            {
                RequestId = requestId,
                SourceId = model.SourceId,
                PublishedAt = model.PublishedAt,
                ExternalUrl = model.ExternalUrl
            });
        }
    }

    private static Expression<Func<AgencyRequestListModel, bool>> ApplyFilterForAgency(GetRequestForAgencyFilter filter)
    {
        var predicate = PredicateBuilder.New<AgencyRequestListModel>(true);
        if (!filter.HasPermissionToSeeInternalRequests)
            predicate = predicate.And(p => p.HasPermissionToSeeInternalRequests == false);
        if (filter.NumberId.HasValue)
            predicate = predicate.And(r => r.NumberId == filter.NumberId.Value);
        if (!string.IsNullOrWhiteSpace(filter.CompanyFullName))
        {
            var criteria = filter.CompanyFullName.ToLower();
            predicate = predicate.And(r => r.CompanyFullName.ToLower().Contains(criteria));
        }
        if (!string.IsNullOrWhiteSpace(filter.Location))
        {
            var locationCriteria = filter.Location.ToLower();
            predicate = predicate.And(r =>
                r.Address.ToLower().Contains(locationCriteria) ||
                r.City.ToLower().Contains(locationCriteria) ||
                r.ProvinceName.ToLower().Contains(locationCriteria) ||
                r.PostalCode.ToLower().Contains(locationCriteria));
        }
        if (!string.IsNullOrWhiteSpace(filter.JobTitle))
            predicate = predicate.And(r => r.JobTitle.ToLower().Contains(filter.JobTitle.ToLower()));
        if (!string.IsNullOrWhiteSpace(filter.SalesRepresentative))
            predicate = predicate.And(r => r.SalesRepresentative.ToLower().Contains(filter.SalesRepresentative.ToLower()));
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(r => r.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && r.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
        if (filter.RateFrom.HasValue && filter.RateTo.HasValue)
            predicate = predicate.And(r => r.WorkerRate >= filter.RateFrom.Value && r.WorkerRate <= filter.RateTo.Value);
        if (filter.Statuses != null && filter.Statuses.Any())
        {
            predicate = predicate.And(r => filter.Statuses.Contains(r.RequestStatus));
        }
        if (filter.JobBoardIds != null && filter.JobBoardIds.Any())
        {
            predicate = predicate.And(r => r.JobBoards.Any(jb => filter.JobBoardIds.Contains(jb.SourceId)));
        }
        if (!string.IsNullOrWhiteSpace(filter.Filter))
            predicate = predicate.And(r =>
            r.NumberId.ToString().Contains(filter.Filter) ||
            r.JobTitle.ToLower().Contains(filter.Filter) ||
            r.CompanyFullName.ToLower().Contains(filter.Filter));
        return predicate;
    }

    private static IQueryable<AgencyRequestListModel> ApplySortForAgency(IQueryable<AgencyRequestListModel> query, GetRequestForAgencyFilter filter)
    {
        switch (filter.SortBy)
        {
            case GetRequestSortBy.Client:
                query = query.AddOrderBy(filter, o => o.CompanyFullName);
                break;
            case GetRequestSortBy.JobTitle:
                query = query.AddOrderBy(filter, o => o.JobTitle);
                break;
            case GetRequestSortBy.CreatedAt:
                query = query.AddOrderBy(filter, o => o.CreatedAt);
                break;
            case GetRequestSortBy.Recruiter:
                query = query.AddOrderBy(filter, o => o.DisplayRecruiters);
                break;
            case GetRequestSortBy.Rate:
                query = query.AddOrderBy(filter, o => o.WorkerRate);
                break;
            case GetRequestSortBy.WorkersQuantity:
                query = query.AddOrderBy(filter, o => o.WorkersQuantity);
                break;
            case GetRequestSortBy.NumberId:
                query = query.AddOrderBy(filter, o => o.NumberId);
                break;
            case GetRequestSortBy.SalesRepresentative:
                query = query.AddOrderBy(filter, o => o.SalesRepresentative);
                break;
        }
        return query;
    }

    public async Task<IEnumerable<JobViewModel>> GetAvailableRequest(IEnumerable<string> countries)
    {
        var openStatus = new RequestStatus[] { RequestStatus.Open };
        var requests = context.Requests.Include(r => r.Shift)
            .Include(r => r.JobLocation).ThenInclude(jl => jl.City).ThenInclude(c => c.Province).ThenInclude(p => p.Country)
            .Where(r => r.CompanyProfile.Active)
            .Where(r => openStatus.Contains(r.Status))
            .Where(r => countries.Contains(r.JobLocation.City.Province.Country.Code))
            .Select(r => new JobViewModel
            {
                RequestId = r.Id,
                NumberId = r.NumberId.ToString(),
                Description = r.Description,
                Requirements = r.Requirements,
                Responsibilities = r.Responsibilities,
                Location = r.JobLocation.City.Value + " " + r.JobLocation.City.Province.Value,
                Salary = r.WorkerRate.HasValue ? r.WorkerRate.Value.ToString("C") : r.WorkerSalary.Value.ToString("C"),
                Title = r.JobTitle,
                Type = Regex.Replace(r.EmploymentType.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).Trim(),
                CreatedAt = r.CreatedAt,
                Shift = r.Shift == null ? string.Empty : r.Shift.DisplayShift
            });
        return await requests.OrderBy(r => r.Title).ToListAsync();
    }

    public async Task<AgencyRequestDetailModel> GetRequestDetailForAgency(Guid id)
    {
        var requests = context.Requests
            .Include(r => r.JobLocation)
            .ThenInclude(jl => jl.City)
            .ThenInclude(c => c.Province)
            .ThenInclude(p => p.Country)
            .Where(c => c.Id == id);
        var query = from r in requests
                    join rcd in context.RequestCancellationDetails on r.Id equals rcd.RequestId
                    into tmpRcd
                    from rcd in tmpRcd.DefaultIfEmpty()
                    select new AgencyRequestDetailModel
                    {
                        Id = r.Id,
                        NumberId = r.NumberId,
                        JobTitle = r.JobTitle,
                        BillingTitle = string.IsNullOrWhiteSpace(r.BillingTitle) ? r.JobTitle : r.BillingTitle,
                        JobCosting = r.JobCosting,
                        UsesRunners = r.UsesRunners,
                        Status = r.Status,
                        CancellationDetail = rcd == null ? null : rcd.OtherReasonCancellationRequest,
                        CompanyLogo = r.CompanyProfile.Logo == null ? null : $"{filesConfiguration.FilesPath}{r.CompanyProfile.Logo.FileName}",
                        CompanyProfileId = r.CompanyProfileId,
                        FullName = r.CompanyProfile.FullName,
                        Description = r.Description,
                        Requirements = r.Requirements,
                        Responsibilities = r.Responsibilities,
                        WorkersQuantity = r.WorkersQuantity,
                        WorkersQuantityWorking = r.WorkersQuantityWorking,
                        AgencyRate = r.AgencyRate,
                        WorkerRate = r.WorkerRate,
                        WorkerSalary = r.WorkerSalary,
                        JobPositionId = r.JobPositionRateId,
                        JobPosition = r.JobPositionRate != null ? r.JobPositionRate.JobPosition : r.JobTitle,
                        HolidayIsPaid = r.HolidayIsPaid,
                        BreakIsPaid = r.BreakIsPaid,
                        DurationBreak = r.DurationBreak,
                        CreatedAt = r.CreatedAt,
                        CreatedBy = r.CreatedBy,
                        StartAt = r.StartAt,
                        FinishAt = r.FinishAt,
                        InvitationSentItAt = r.InvitationSentItAt,
                        Incentive = r.Incentive,
                        IncentiveDescription = r.IncentiveDescription,
                        DurationTerm = r.DurationTerm,
                        EmploymentType = r.EmploymentType,
                        DisplayRecruiters = string.Join("|", context.RequestRecruiters
                            .Where(rr => rr.RequestId == r.Id)
                            .Select(rr => rr.Recruiter.Name).Distinct()),
                        DisplayShift = r.Shift == null ? null : r.Shift.DisplayShift,
                        IsAsap = r.IsAsap,
                        VaccinationRequired = r.CompanyProfile.VaccinationRequired,
                        PunchCardOptionEnabled = r.PunchCardOptionEnabled,
                        InternalRequirements = r.InternalRequirements,
                        SalesRepresentativeId = r.RequestComission != null ? r.RequestComission.AgencyPersonnelId : null,
                        CompanyUserIds = r.RequestCompanyUser.Select(rcu => rcu.CompanyUserId),
                        JobLocation = new LocationDetailModel
                        {
                            Id = r.JobLocation.Id,
                            Address = r.JobLocation.Address,
                            Latitude = r.JobLocation.Latitude,
                            Longitude = r.JobLocation.Longitude,
                            PostalCode = r.JobLocation.PostalCode,
                            Entrance = r.JobLocation.Entrance,
                            MainIntersection = r.JobLocation.MainIntersection,
                            City = new CityModel
                            {
                                Id = r.JobLocation.City.Id,
                                Value = r.JobLocation.City.Value,
                                Code = r.JobLocation.City.Code,
                                Province = new ProvinceModel
                                {
                                    Id = r.JobLocation.City.Province.Id,
                                    Value = r.JobLocation.City.Province.Value,
                                    Code = r.JobLocation.City.Province.Code,
                                    Country = new CountryModel
                                    {
                                        Id = r.JobLocation.City.Province.Country.Id,
                                        Value = r.JobLocation.City.Province.Country.Value,
                                        Code = r.JobLocation.City.Province.Country.Code
                                    }
                                }
                            }
                        },
                    };
        return await query.SingleOrDefaultAsync();
    }

    public async Task<PaginatedList<RequestListModel>> GetRequestsForCompany(Guid companyId, GetRequestForCompanyFilter filter)
    {
        var requests = context.Requests.Where(r => r.CompanyProfile.CompanyId == companyId);
        if (filter.CompanyUserId.HasValue)
            requests = requests.Where(r => r.RequestCompanyUser.Any(rcu => rcu.CompanyUserId == filter.CompanyUserId));
        var query = from request in requests
                    select new RequestListModel
                    {
                        CompanyProfileId = request.CompanyProfileId,
                        Id = request.Id,
                        NumberId = request.NumberId,
                        JobTitle = request.JobTitle,
                        CreatedAt = request.CreatedAt,
                        WorkersQuantity = request.WorkersQuantity,
                        WorkersQuantityWorking = request.WorkersQuantityWorking,
                        Location = request.JobLocation.Address + " " + request.JobLocation.City.Value + " " + request.JobLocation.City.Province.Value + " " + request.JobLocation.PostalCode,
                        Entrance = request.JobLocation.Entrance,
                        CompanyFullName = request.CompanyProfile.Agency.FullName,
                        RequestStatus = request.Status,
                        IsAsap = request.IsAsap,
                        IsDirectHiring = request.WorkerSalary.HasValue,
                        DisplayShift = request.Shift == null ? null : request.Shift.DisplayShift
                    };
        var predicateNew = ApplyFilterForCompany(filter);
        query = query.Where(predicateNew);
        query = ApplySortForCompany(query, filter);
        return await query.ToPaginatedList(filter);
    }

    private static Expression<Func<RequestListModel, bool>> ApplyFilterForCompany(GetRequestForCompanyFilter filter)
    {
        // Companies can see Open and Filled orders (InProgress was removed 2026-01-28)
        var statusToVisualize = new RequestStatus[] { RequestStatus.Open, RequestStatus.Filled };
        Expression<Func<RequestListModel, bool>> predicate = r => statusToVisualize.Contains(r.RequestStatus);
        if (filter.NumberId.HasValue)
            predicate = predicate.And(r => r.NumberId == filter.NumberId.Value);
        if (!string.IsNullOrWhiteSpace(filter.JobTitle))
            predicate = predicate.And(r => r.JobTitle.ToLower().Contains(filter.JobTitle.ToLower()));
        if (!string.IsNullOrWhiteSpace(filter.Location))
        {
            var location = filter.Location.ToLower();
            predicate = predicate.And(r =>
                r.Location.ToLower().Contains(location) ||
                r.Entrance.ToLower().Contains(location));
        }
        return predicate;
    }

    private IQueryable<RequestListModel> ApplySortForCompany(IQueryable<RequestListModel> query, GetRequestForCompanyFilter filter)
    {
        switch (filter.SortBy)
        {
            case GetRequestSortBy.NumberId:
                query = query.AddOrderBy(filter, r => r.NumberId);
                break;
            case GetRequestSortBy.JobTitle:
                query = query.AddOrderBy(filter, r => r.JobTitle);
                break;
            case GetRequestSortBy.WorkersQuantity:
                query = query.AddOrderBy(filter, r => r.WorkersQuantity);
                break;
        }
        return query;
    }

    public Task<CompanyRequestDetailModel> GetRequestDetailForCompany(Guid id) =>
        (from r in context.Requests.Where(c => c.Id == id)
         select new CompanyRequestDetailModel
         {
             Id = r.Id,
             JobTitle = r.JobTitle,
             WorkersQuantity = r.WorkersQuantity,
             WorkersQuantityWorking = r.WorkersQuantityWorking,
             Description = r.Description,
             DurationBreak = r.DurationBreak,
             BreakIsPaid = r.BreakIsPaid,
             JobIsOnBranchOffice = r.JobIsOnBranchOffice,
             Incentive = r.Incentive,
             HolidayIsPaid = r.HolidayIsPaid,
             IncentiveDescription = r.IncentiveDescription,
             Requirements = r.Requirements,
             Responsibilities = r.Responsibilities,
             IsAsap = r.IsAsap,
             JobLocation = new LocationDetailModel
             {
                 Id = r.JobLocation.Id,
                 Address = r.JobLocation.Address,
                 PostalCode = r.JobLocation.PostalCode,
                 Entrance = r.JobLocation.Entrance,
                 MainIntersection = r.JobLocation.MainIntersection,
                 Latitude = r.JobLocation.Latitude,
                 Longitude = r.JobLocation.Longitude,
                 City = new CityModel
                 {
                     Id = r.JobLocation.City.Id,
                     Value = r.JobLocation.City.Value,
                     Code = r.JobLocation.City.Code,
                     Province = new ProvinceModel
                     {
                         Id = r.JobLocation.City.Province.Id,
                         Value = r.JobLocation.City.Province.Value,
                         Code = r.JobLocation.City.Province.Code,
                         Country = new CountryModel
                         {
                             Id = r.JobLocation.City.Province.Country.Id,
                             Value = r.JobLocation.City.Province.Country.Value,
                             Code = r.JobLocation.City.Province.Country.Code
                         }
                     }
                 }
             },
             JobPositionRate = new JobPositionDetailModel
             {
                 Value = r.JobPositionRate != null ? r.JobPositionRate.JobPosition : r.JobTitle
             },
             AgencyRate = r.AgencyRate,
             WorkerSalary = r.WorkerSalary,
             CreatedAt = r.CreatedAt,
             Status = r.Status,
             DurationTerm = r.DurationTerm,
             DisplayShift = r.Shift == null ? null : r.Shift.DisplayShift,
             StartAt = r.StartAt,
             FinishAt = r.FinishAt
         }).SingleOrDefaultAsync();

    public Task<Common.Entities.Request.Request> GetRequest(Expression<Func<Common.Entities.Request.Request, bool>> condition) =>
        context.Requests.Where(condition)
            .Include(r => r.JobPositionRate)
            .Include(r => r.JobLocation).ThenInclude(l => l.City).ThenInclude(c => c.Province).ThenInclude(p => p.Country)
            .Include(r => r.Workers)
            .Include(r => r.Shift)
            .Include(i => i.Recruiters).ThenInclude(ti => ti.Recruiter).ThenInclude(u => u.User)
            .Include(i => i.CompanyProfile).ThenInclude(cp => cp.Agency)
            .SingleOrDefaultAsync();

    public async Task<PaginatedList<AgencyWorkerRequestModel>> GetWorkersRequestByRequestId(Guid requestId, GetWorkersRequestFilter filter)
    {
        var query = from wr in context.WorkerRequests
                    select new AgencyWorkerRequestModel
                    {
                        Id = wr.Id,
                        RequestId = wr.RequestId,
                        NumberId = wr.WorkerProfile.NumberId,
                        WorkerId = wr.WorkerProfile.WorkerId,
                        WorkerProfileId = wr.WorkerProfileId,
                        Name =
                            wr.WorkerProfile.FirstName +
                            (string.IsNullOrWhiteSpace(wr.WorkerProfile.MiddleName) ? string.Empty : " " + wr.WorkerProfile.MiddleName) +
                            " " + wr.WorkerProfile.LastName +
                            (string.IsNullOrWhiteSpace(wr.WorkerProfile.SecondLastName) ? string.Empty : " " + wr.WorkerProfile.SecondLastName),
                        ProfileImage = wr.WorkerProfile.ProfileImage == null ? null : $"{filesConfiguration.FilesPath}{wr.WorkerProfile.ProfileImage.FileName}",
                        WorkerRequestStatus = wr.WorkerRequestStatus,
                        RejectComments = wr.RejectComments,
                        RejectedAt = wr.RejectedAt,
                        RejectedBy = wr.RejectedBy,
                        ApprovedToWork = wr.WorkerProfile.ApprovedToWork,
                        IsSubcontractor = wr.WorkerProfile.IsSubcontractor,
                        SocialInsurance = wr.WorkerProfile.SocialInsurance,
                        DueDate = wr.WorkerProfile.DueDate,
                        SocialInsuranceExpire = wr.WorkerProfile.SocialInsuranceExpire,
                        MobileNumber = wr.WorkerProfile.MobileNumber,
                        CreatedBy = wr.CreatedBy,
                        CreatedAt = wr.CreatedAt,
                        StartWorking = wr.StartWorking,
                        NotesCount = wr.Notes.Count(n => !n.Note.IsDeleted),
                        TotalHoursApproved = wr.TimeSheets.Where(c => c.TimeOutApproved != null && c.TimeInApproved != null).Sum(s => (s.TimeOutApproved - s.TimeInApproved).Value.TotalHours),
                        TotalHoursWorker = wr.TimeSheets.Where(c => c.TimeOut != null).Sum(c => (c.TimeOut - c.TimeIn).Value.TotalHours),
                        ExternalId = wr.WorkerProfile.ExternalId
                    };
        var predicateNew = ApplyFilterWorkersRequest(requestId, filter);
        query = query.Where(predicateNew);
        query = ApplySortWorkersRequest(query, filter);
        return await query.ToPaginatedList(filter);
    }

    private Expression<Func<AgencyWorkerRequestModel, bool>> ApplyFilterWorkersRequest(Guid requestId, GetWorkersRequestFilter filter)
    {
        Expression<Func<AgencyWorkerRequestModel, bool>> predicate = wr => wr.RequestId == requestId;
        if (filter.NumberId.HasValue)
            predicate = predicate.And(wr => wr.NumberId == filter.NumberId.Value);
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var name = filter.Name.ToLower();
            predicate = predicate.And(wr => EF.Functions.Like(wr.Name.ToLower(), $"%{name}%"));
        }
        if (filter.Statuses != null && filter.Statuses.Any())
            predicate = predicate.And(wr => filter.Statuses.Contains(wr.WorkerRequestStatus));
        if (!string.IsNullOrWhiteSpace(filter.Phone))
        {
            var expression = new Regex(@"\s+");
            var phoneWithoutBlankSpaces = expression.Replace(filter.Phone, string.Empty);
            predicate = predicate.And(wp =>
                wp.MobileNumber.Replace("(", string.Empty).Replace(")", string.Empty).Contains(filter.Phone) ||
                wp.MobileNumber.Contains(phoneWithoutBlankSpaces));
        }
        if (!string.IsNullOrWhiteSpace(filter.SocialInsurance))
            predicate = predicate.And(wr => wr.SocialInsurance.Contains(filter.SocialInsurance));
        if (filter.StartWorkingFrom.HasValue && filter.StartWorkingTo.HasValue)
            predicate = predicate.And(r => r.StartWorking.Value.Date >= filter.StartWorkingFrom.Value.Date && r.StartWorking.Value.Date <= filter.StartWorkingTo.Value.Date);
        if (!string.IsNullOrWhiteSpace(filter.CreatedBy))
            predicate = predicate.And(c => c.CreatedBy.ToLower().Contains(filter.CreatedBy.ToLower()));
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(c => c.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && c.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
        if (!string.IsNullOrWhiteSpace(filter.RejectedBy))
            predicate = predicate.And(c => c.RejectedBy.ToLower().Contains(filter.RejectedBy.ToLower()));
        if (filter.RejectedAtFrom.HasValue && filter.RejectedAtTo.HasValue)
            predicate = predicate.And(c => c.RejectedAt.Value.Date >= filter.RejectedAtFrom.Value.Date && c.RejectedAt.Value.Date <= filter.RejectedAtTo.Value.Date);
        if (!string.IsNullOrWhiteSpace(filter.ExternalId))
        {
            var externalId = filter.ExternalId.ToLower();
            predicate = predicate.And(wr => EF.Functions.Like(wr.ExternalId.ToLower(), $"%{externalId}%"));
        }
        return predicate;
    }

    private IQueryable<AgencyWorkerRequestModel> ApplySortWorkersRequest(IQueryable<AgencyWorkerRequestModel> query, GetWorkersRequestFilter filter)
    {
        switch (filter.SortBy)
        {
            case GetWorkersRequestSortBy.NumberId:
                query = query.AddOrderBy(filter, wr => wr.NumberId);
                break;
            case GetWorkersRequestSortBy.Name:
                query = query.AddOrderBy(filter, wr => wr.Name);
                break;
            case GetWorkersRequestSortBy.Status:
                query = query.AddOrderBy(filter, wr => wr.WorkerRequestStatus).ThenBy(wr => wr.Name);
                break;
            case GetWorkersRequestSortBy.StartWorking:
                query = query.AddOrderBy(filter, wr => wr.StartWorking).ThenBy(wr => wr.Name);
                break;
            case GetWorkersRequestSortBy.CreatedBy:
                if (!filter.IsDescending)
                    query = query.AddOrderBy(filter, o => o.CreatedAt).ThenBy(o => o.CreatedBy);
                else
                    query = query.AddOrderBy(filter, o => o.CreatedAt).ThenByDescending(o => o.CreatedBy);
                break;
            case GetWorkersRequestSortBy.RejectedBy:
                if (!filter.IsDescending)
                    query = query.AddOrderBy(filter, o => o.RejectedAt).ThenBy(o => o.RejectedBy);
                else
                    query = query.AddOrderBy(filter, o => o.RejectedAt).ThenByDescending(o => o.RejectedBy);
                break;
            case GetWorkersRequestSortBy.ExternalId:
                query = query.AddOrderBy(filter, wr => wr.ExternalId);
                break;
        }
        return query;
    }

    public Task<AgencyWorkerRequestModel> GetWorkerRequestByAgencyId(Guid agencyId, Guid requestId, Guid workerRequestId)
    {
        IQueryable<AgencyWorkerRequestModel> query = from wr in context.WorkerRequests.Where(c => c.Id == workerRequestId && c.RequestId == requestId)
                                                     where wr.WorkerProfile.AgencyId == agencyId
                                                     select new AgencyWorkerRequestModel
                                                     {
                                                         Id = wr.Id,
                                                         WorkerId = wr.WorkerProfile.WorkerId,
                                                         WorkerProfileId = wr.WorkerProfileId,
                                                         Name = $"{wr.WorkerProfile.FirstName} {wr.WorkerProfile.MiddleName} {wr.WorkerProfile.LastName} {wr.WorkerProfile.SecondLastName}",
                                                         WorkerRequestStatus = wr.WorkerRequestStatus,
                                                         RejectComments = wr.RejectComments,
                                                         RejectedAt = wr.RejectedAt,
                                                         ProfileImage = wr.WorkerProfile.ProfileImage == null ? null : $"{filesConfiguration.FilesPath}{wr.WorkerProfile.ProfileImage.FileName}",
                                                         NumberId = wr.WorkerProfile.NumberId,
                                                         ApprovedToWork = wr.WorkerProfile.ApprovedToWork,
                                                         SocialInsurance = wr.WorkerProfile.SocialInsurance,
                                                         DueDate = wr.WorkerProfile.DueDate,
                                                         SocialInsuranceExpire = wr.WorkerProfile.SocialInsuranceExpire,
                                                         ExternalId = wr.WorkerProfile.ExternalId
                                                     };
        return query.AsNoTracking().SingleOrDefaultAsync();
    }

    public Task<PaginatedList<WorkerRequestListModel>> GetRequestsHistoryForWorker(Guid workerId, Pagination pagination) =>
        (from wp in context.WorkerProfiles.Where(c => c.WorkerId == workerId)
         join r in context.Requests.Where(r => r.Status == RequestStatus.Cancelled) on wp.AgencyId equals r.CompanyProfile.AgencyId
         join wr in context.WorkerRequests.Where(c => c.WorkerProfile.WorkerId == workerId) on r.Id equals wr.RequestId
         orderby r.NumberId descending
         select new WorkerRequestListModel
         {
             Id = r.Id,
             NumberId = r.NumberId,
             IsAsap = r.IsAsap,
             AgencyFullName = r.CompanyProfile.Agency.FullName,
             AgencyLogo = r.CompanyProfile.Agency.Logo == null ? null : $"{filesConfiguration.FilesPath}{r.CompanyProfile.Agency.Logo.FileName}",
             CreatedAt = r.CreatedAt,
             JobTitle = r.JobTitle,
             WorkerRate = r.WorkerRate,
             WorkerSalary = r.WorkerSalary,
             WorkerApprovedToWork = wp.ApprovedToWork.ToString(),
             Location = $"{r.JobLocation.Address} {r.JobLocation.City.Value} {r.JobLocation.City.Province.Value} {r.JobLocation.PostalCode}",
             Entrance = r.JobLocation.Entrance,
             Status = wr.WorkerRequestStatus.ToString(),
             WorkersQuantity = r.WorkersQuantity,
             StartAt = r.StartAt,
             FinishAt = r.FinishAt,
             DurationTerm = r.DurationTerm.ToString()
         }).AsNoTracking().ToPaginatedList(pagination);

    public async Task<PaginatedList<WorkerRequestListModel>> GetRequestsForWorker(Guid workerId, Pagination pagination)
    {
        var workerProfile = await context.WorkerProfiles.FirstOrDefaultAsync(wp => wp.WorkerId == workerId);
        var workerRequests = context.WorkerRequests.Where(wr => wr.WorkerProfile.WorkerId == workerId && wr.WorkerRequestStatus == WorkerRequestStatus.Booked);
        var openStatus = new RequestStatus[] { RequestStatus.Open };
        var requests = Enumerable.Empty<WorkerRequestListModel>();
        var ownRequest = context.Requests.Include(wr => wr.CompanyProfile).ThenInclude(cp => cp.Agency)
            .Join(workerRequests, r => r.Id, wr => wr.RequestId, (r, wr) => new { r, wr })
            .Select(lj => new WorkerRequestListModel
            {
                Id = lj.r.Id,
                NumberId = lj.r.NumberId,
                IsAsap = lj.r.IsAsap,
                CreatedAt = lj.r.CreatedAt,
                JobTitle = lj.r.JobTitle,
                AgencyFullName = lj.r.CompanyProfile.Agency.FullName,
                WorkerRate = lj.r.WorkerRate.HasValue ? lj.r.WorkerRate : lj.r.WorkerSalary,
                Location = $"{lj.r.JobLocation.Address} {lj.r.JobLocation.City.Value} {lj.r.JobLocation.City.Province.Value} {lj.r.JobLocation.PostalCode}",
                Entrance = lj.r.JobLocation.Entrance,
                Status = lj.wr.WorkerRequestStatus.ToString(),
                WorkersQuantity = lj.r.WorkersQuantity,
                StartAt = lj.r.StartAt,
                FinishAt = lj.r.FinishAt,
                DurationTerm = lj.r.DurationTerm.ToString()
            }).AsEnumerable();
        if (!ownRequest.Any())
        {
            var requestToExclude = context.WorkerRequests.Where(wr => wr.WorkerProfile.WorkerId == workerId && wr.WorkerRequestStatus == WorkerRequestStatus.Rejected);
            var availableRequest = context.Requests.Include(r => r.CompanyProfile).ThenInclude(cp => cp.Agency)
                .Where(r => openStatus.Contains(r.Status))
                .Where(r => r.CompanyProfile.AgencyId == workerProfile.AgencyId)
                .Where(r => !requestToExclude.Any(rte => rte.RequestId == r.Id))
                .Where(r => !ownRequest.Any(or => or.Id == r.Id))
                .Select(r => new WorkerRequestListModel
                {
                    Id = r.Id,
                    NumberId = r.NumberId,
                    IsAsap = r.IsAsap,
                    CreatedAt = r.CreatedAt,
                    JobTitle = r.JobTitle,
                    AgencyFullName = r.CompanyProfile.Agency.FullName,
                    WorkerRate = r.WorkerSalary.HasValue ? r.WorkerSalary : r.WorkerRate,
                    Location = $"{r.JobLocation.City.Value} {r.JobLocation.City.Province.Value}",
                    Entrance = r.JobLocation.Entrance,
                    Status = null,
                    WorkersQuantity = r.WorkersQuantity,
                    StartAt = r.StartAt,
                    FinishAt = r.FinishAt,
                    DurationTerm = r.DurationTerm.ToString()
                }).AsEnumerable();
            requests = availableRequest.Union(ownRequest);
        }
        else
        {
            requests = ownRequest;
        }
        requests = requests.OrderByDescending(r => r.NumberId);
        var response = await requests.ToPaginatedList(pagination);
        return response;
    }

    public Task<WorkerRequestDetailModel> GetRequestDetailForWorker(Guid workerId, Guid requestId)
    {
        return (from r in context.Requests.Where(c => c.Id == requestId)
                join wp in context.WorkerProfiles.Where(c => c.WorkerId == workerId) on r.CompanyProfile.AgencyId equals wp.AgencyId
                join wr in context.WorkerRequests.Where(c => c.WorkerProfile.WorkerId == workerId && c.RequestId == requestId) on r.Id equals wr.RequestId into tmp
                from wr in tmp.DefaultIfEmpty()
                join ra in context.RequestApplicants on new { rId = r.Id, wpId = wp.Id } equals new { rId = ra.RequestId, wpId = ra.WorkerProfileId.Value }
                    into tmpRa
                from ra in tmpRa.DefaultIfEmpty()
                select new WorkerRequestDetailModel
                {
                    Id = r.Id,
                    JobTitle = r.JobTitle,
                    Status = wr == null ? "None" : wr.WorkerRequestStatus.ToString(),
                    DurationTerm = r.DurationTerm.ToString(),
                    RequestStatus = r.Status.ToString(),
                    AgencyFullName = r.CompanyProfile.Agency.FullName,
                    AgencyLogo = r.CompanyProfile.Agency.Logo == null ? null : filesConfiguration.FilesPath + r.CompanyProfile.Agency.Logo.FileName,
                    Description = r.Description,
                    Requirements = r.Requirements,
                    WorkersQuantity = r.WorkersQuantity,
                    WorkerRate = r.WorkerRate,
                    WorkerSalary = r.WorkerSalary,
                    JobPosition = r.JobPositionRate != null ? r.JobPositionRate.JobPosition : r.JobTitle,
                    HolidayIsPaid = r.HolidayIsPaid,
                    BreakIsPaid = r.BreakIsPaid,
                    CreatedAt = r.CreatedAt,
                    StartAt = r.StartAt,
                    FinishAt = r.FinishAt,
                    Incentive = r.Incentive,
                    IncentiveDescription = r.IncentiveDescription,
                    DurationBreak = r.DurationBreak,
                    Location = r.JobLocation.Address + " " + r.JobLocation.City.Value + " " + r.JobLocation.City.Province.Value + " " + r.JobLocation.PostalCode,
                    IsApplicant = ra != null,
                    PunchCardOptionEnabled = r.PunchCardOptionEnabled,
                    JobLocation = new LocationDetailModel
                    {
                        Id = r.JobLocation.Id,
                        Address = r.JobLocation.Address,
                        Latitude = r.JobLocation.Latitude,
                        Longitude = r.JobLocation.Longitude,
                        PostalCode = r.JobLocation.PostalCode,
                        Entrance = r.JobLocation.Entrance,
                        MainIntersection = r.JobLocation.MainIntersection,
                        City = new CityModel
                        {
                            Id = r.JobLocation.City.Id,
                            Value = r.JobLocation.City.Value,
                            Code = r.JobLocation.City.Code,
                            Province = new ProvinceModel
                            {
                                Id = r.JobLocation.City.Province.Id,
                                Value = r.JobLocation.City.Province.Value,
                                Code = r.JobLocation.City.Province.Code,
                                Country = new CountryModel
                                {
                                    Id = r.JobLocation.City.Province.Country.Id,
                                    Value = r.JobLocation.City.Province.Country.Value,
                                    Code = r.JobLocation.City.Province.Country.Code
                                }
                            }
                        }
                    }
                }).AsNoTracking().SingleOrDefaultAsync();
    }

    public Task<PaginatedList<RequestListModel>> GetRequestsHistoryByWorkerProfileId(Guid workerProfileId, Pagination pagination) =>
        GetWorkerRequestHistory(wp => wp.Id == workerProfileId, pagination);

    private Task<PaginatedList<RequestListModel>> GetWorkerRequestHistory(Expression<Func<WorkerProfile, bool>> filter, Pagination pagination)
    {
        var query = from wp in context.WorkerProfiles.Where(filter)
                    join wr in context.WorkerRequests on wp.Id equals wr.WorkerProfileId
                    join r in context.Requests on new { Ag = wp.AgencyId, Re = wr.RequestId } equals new { Ag = r.CompanyProfile.AgencyId, Re = r.Id }
                    orderby r.NumberId descending
                    select new RequestListModel
                    {
                        Id = r.Id,
                        NumberId = r.NumberId,
                        IsAsap = r.IsAsap,
                        CompanyFullName = r.CompanyProfile.FullName,
                        Logo = r.CompanyProfile.Logo == null ? null : $"{filesConfiguration.FilesPath}{r.CompanyProfile.Logo.FileName}",
                        AgencyFullName = r.CompanyProfile.Agency.FullName,
                        AgencyLogo = r.CompanyProfile.Agency.Logo == null ? null : $"{filesConfiguration.FilesPath}{r.CompanyProfile.Agency.Logo.FileName}",
                        CreatedAt = r.CreatedAt,
                        JobTitle = r.JobTitle,
                        Location = $"{r.JobLocation.Address} {r.JobLocation.City.Value} {r.JobLocation.City.Province.Value} {r.JobLocation.PostalCode}",
                        RequestStatus = r.Status,
                        WorkersQuantity = r.WorkersQuantity,
                        WorkersQuantityWorking = r.WorkersQuantityWorking,
                        StartWorking = (from wrS in context.WorkerRequests.Where(wrW => wrW.RequestId == r.Id)
                                        from tS in wrS.TimeSheets
                                        select tS.Date).DefaultIfEmpty().Min(),
                        FinishWorking = (from wrS in context.WorkerRequests.Where(wrW => wrW.RequestId == r.Id)
                                         from tS in wrS.TimeSheets
                                         select tS.Date).DefaultIfEmpty().Max()
                    };
        return query.ToPaginatedList(pagination);
    }

    public Task<AgencyWorkerRequestModel> GetRequestWorkerByCompanyId(Guid companyId, Guid requestId, Guid workerProfileId)
    {
        var query = (from wr in context.WorkerRequests.Where(c => c.RequestId == requestId && c.WorkerProfileId == workerProfileId)
                     where wr.Request.CompanyProfile.CompanyId == companyId
                     select new AgencyWorkerRequestModel
                     {
                         Id = wr.Id,
                         WorkerId = wr.WorkerProfile.WorkerId,
                         WorkerProfileId = wr.WorkerProfileId,
                         Name = $"{wr.WorkerProfile.FirstName} {wr.WorkerProfile.MiddleName} {wr.WorkerProfile.LastName} {wr.WorkerProfile.SecondLastName}",
                         WorkerRequestStatus = wr.WorkerRequestStatus,
                         ProfileImage = wr.WorkerProfile.ProfileImage == null ? null : $"{filesConfiguration.FilesPath}{wr.WorkerProfile.ProfileImage.FileName}",
                         NumberId = wr.WorkerProfile.NumberId,
                         DueDate = wr.WorkerProfile.DueDate,
                         SocialInsuranceExpire = wr.WorkerProfile.SocialInsuranceExpire,
                         ApprovedToWork = wr.WorkerProfile.ApprovedToWork
                     }).OrderByDescending(c => c.Name).AsNoTracking();
        return query.SingleOrDefaultAsync();
    }

    public Task<ShiftModel> GetRequestShift(Guid requestId) =>
        context.Requests.Where(c => c.Id == requestId)
            .Select(s => new ShiftModel
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
            }).SingleOrDefaultAsync();

    public Task<RequestCancellationDetail> GetRequestCancellationDetail(Guid requestId) =>
        context.RequestCancellationDetails.Where(s => s.RequestId == requestId)
            .Include(i => i.ReasonCancellationRequest)
            .SingleOrDefaultAsync();

    public Task<RequestFinalizationDetail> GetRequestFinalizationDetail(Guid requestId) => context.RequestFinalizationDetails.SingleOrDefaultAsync(s => s.RequestId == requestId);

    public Task SaveChangesAsync() => context.SaveChangesAsync();

    public Task<RequestContactPersonDetailModel> GetRequestedByDetail(Guid requestId, Guid contactPersonId) =>
        context.RequestRequestedBys.Where(c => c.RequestId == requestId && c.ContactPersonId == contactPersonId)
            .Select(s => new RequestContactPersonDetailModel
            {
                Id = s.ContactPerson.Id,
                Title = s.ContactPerson.Title,
                FirstName = s.ContactPerson.FirstName,
                MiddleName = s.ContactPerson.MiddleName,
                LastName = s.ContactPerson.LastName,
                Position = s.ContactPerson.Position,
                Email = s.ContactPerson.Email,
                MobileNumber = s.ContactPerson.MobileNumber,
                OfficeNumber = s.ContactPerson.OfficeNumber,
                OfficeNumberExt = s.ContactPerson.OfficeNumberExt
            }).SingleOrDefaultAsync();

    public Task<RequestContactPersonDetailModel> GetReportToDetail(Guid requestId, Guid contactPersonId) =>
        context.RequestReportTos.Where(c => c.RequestId == requestId && c.ContactPersonId == contactPersonId)
            .Select(s => new RequestContactPersonDetailModel
            {
                Id = s.ContactPerson.Id,
                Title = s.ContactPerson.Title,
                FirstName = s.ContactPerson.FirstName,
                MiddleName = s.ContactPerson.MiddleName,
                LastName = s.ContactPerson.LastName,
                Position = s.ContactPerson.Position,
                Email = s.ContactPerson.Email,
                MobileNumber = s.ContactPerson.MobileNumber,
                OfficeNumber = s.ContactPerson.OfficeNumber,
                OfficeNumberExt = s.ContactPerson.OfficeNumberExt
            }).SingleOrDefaultAsync();

    public Task<PaginatedList<RequestContactPersonModel>> GetRequestedByList(Guid requestId, Pagination pagination) =>
        context.RequestRequestedBys.Where(c => c.RequestId == requestId)
            .Select(s => new RequestContactPersonModel
            {
                Id = s.ContactPerson.Id,
                Title = s.ContactPerson.Title,
                FirstName = s.ContactPerson.FirstName,
                MiddleName = s.ContactPerson.MiddleName,
                LastName = s.ContactPerson.LastName,
            }).ToPaginatedList(pagination);

    public Task<RequestRequestedBy> GetRequestedBy(Guid requestId, Guid contactPersonId) => context.RequestRequestedBys.SingleOrDefaultAsync(c => c.RequestId == requestId && c.ContactPersonId == contactPersonId);

    public Task<RequestReportTo> GetReportTo(Guid requestId, Guid contactPersonId) => context.RequestReportTos.SingleOrDefaultAsync(c => c.RequestId == requestId && c.ContactPersonId == contactPersonId);

    public Task<PaginatedList<RequestContactPersonModel>> GetReportToList(Guid requestId, Pagination pagination) =>
        context.RequestReportTos.Where(c => c.RequestId == requestId)
            .Select(s => new RequestContactPersonModel
            {
                Id = s.ContactPerson.Id,
                Title = s.ContactPerson.Title,
                FirstName = s.ContactPerson.FirstName,
                MiddleName = s.ContactPerson.MiddleName,
                LastName = s.ContactPerson.LastName,
            }).ToPaginatedList(pagination);

    public Task<PaginatedList<NoteModel>> GetNotes(Guid requestId, Pagination pagination) =>
        context.RequestNotes.Where(w => w.RequestId == requestId && !w.Note.IsDeleted)
            .Select(RequestExtensionsMapping.SelectNote).OrderByDescending(c => c.CreatedAt).ToPaginatedList(pagination);

    public Task<NoteModel> GetNoteDetail(Guid requestId, Guid id) =>
        context.RequestNotes.Where(w => w.RequestId == requestId && w.NoteId == id)
            .Select(RequestExtensionsMapping.SelectNote).SingleOrDefaultAsync();

    public Task<RequestNote> GetNote(Guid requestId, Guid id) =>
        context.RequestNotes.Where(c => c.RequestId == requestId && c.NoteId == id)
            .Include(c => c.Note).SingleOrDefaultAsync();

    public async Task<IEnumerable<WeeklyBoardAssignmentModel>> GetWeeklyBoardAssignments(Guid agencyId, DateTime weekStart, DateTime weekEnd) =>
        await (from rr in context.RequestRecruiters
               where rr.Request.CompanyProfile.AgencyId == agencyId
                     && rr.WorkDate != null
                     && rr.WorkDate >= weekStart.Date
                     && rr.WorkDate <= weekEnd.Date
               select new WeeklyBoardAssignmentModel
               {
                   RecruiterId = rr.RecruiterId,
                   RecruiterName = rr.Recruiter.Name,
                   RequestId = rr.RequestId,
                   NumberId = rr.Request.NumberId,
                   CompanyName = rr.Request.CompanyProfile.FullName,
                   JobTitle = rr.Request.JobTitle,
                   City = rr.Request.JobLocation.City.Value,
                   ProvinceCode = rr.Request.JobLocation.City.Province.Code,
                   WorkDate = rr.WorkDate.Value,
                   Status = rr.Request.Status,
                   IsAsap = rr.Request.IsAsap,
                   WorkerSalary = rr.Request.WorkerSalary,
                   UsesRunners = rr.Request.UsesRunners,
                   RunnersSent = rr.Runners.Count,
                   Runners = rr.Runners.Select(r => new WeeklyBoardRunnerModel
                   {
                       RunnerId = r.Id,
                       WorkerProfileId = r.WorkerProfileId,
                       FullName = r.WorkerProfile.FirstName +
                           (string.IsNullOrWhiteSpace(r.WorkerProfile.MiddleName) ? string.Empty : " " + r.WorkerProfile.MiddleName) +
                           " " + r.WorkerProfile.LastName +
                           (string.IsNullOrWhiteSpace(r.WorkerProfile.SecondLastName) ? string.Empty : " " + r.WorkerProfile.SecondLastName),
                       Email = r.WorkerProfile.Worker.Email,
                       Type = r.Type,
                       Status = r.Status,
                       SentAt = r.CreatedAt
                   }).ToList()
               }).ToListAsync();

    public async Task<IEnumerable<WeeklyBoardRunnerModel>> GetOrderRunners(Guid agencyId, Guid requestId) =>
        await (from r in context.Runners
               where r.RequestId == requestId && r.Request.CompanyProfile.AgencyId == agencyId
               orderby r.CreatedAt
               select new WeeklyBoardRunnerModel
               {
                   RunnerId = r.Id,
                   WorkerProfileId = r.WorkerProfileId,
                   FullName = r.WorkerProfile.FirstName +
                       (string.IsNullOrWhiteSpace(r.WorkerProfile.MiddleName) ? string.Empty : " " + r.WorkerProfile.MiddleName) +
                       " " + r.WorkerProfile.LastName +
                       (string.IsNullOrWhiteSpace(r.WorkerProfile.SecondLastName) ? string.Empty : " " + r.WorkerProfile.SecondLastName),
                   Email = r.WorkerProfile.Worker.Email,
                   Type = r.Type,
                   Status = r.Status,
                   SentAt = r.CreatedAt
               }).ToListAsync();

    public async Task<Dictionary<Guid, string>> GetWorkerProfileNames(IEnumerable<Guid> workerProfileIds) =>
        await context.WorkerProfiles
            .Where(w => workerProfileIds.Contains(w.Id))
            .ToDictionaryAsync(
                w => w.Id,
                w => w.FirstName +
                    (string.IsNullOrWhiteSpace(w.MiddleName) ? string.Empty : " " + w.MiddleName) +
                    " " + w.LastName +
                    (string.IsNullOrWhiteSpace(w.SecondLastName) ? string.Empty : " " + w.SecondLastName));

    public async Task<IEnumerable<WeeklyBoardAssignmentModel>> GetWeeklyBoardAssignmentsForRecruiter(Guid agencyId, Guid recruiterId, DateTime weekStart, DateTime weekEnd) =>
        await (from rr in context.RequestRecruiters
               where rr.Request.CompanyProfile.AgencyId == agencyId
                     && rr.RecruiterId == recruiterId
                     && rr.WorkDate != null
                     && rr.WorkDate >= weekStart.Date
                     && rr.WorkDate <= weekEnd.Date
               select new WeeklyBoardAssignmentModel
               {
                   RecruiterId = rr.RecruiterId,
                   RecruiterName = rr.Recruiter.Name,
                   RequestId = rr.RequestId,
                   NumberId = rr.Request.NumberId,
                   CompanyName = rr.Request.CompanyProfile.FullName,
                   JobTitle = rr.Request.JobTitle,
                   City = rr.Request.JobLocation.City.Value,
                   ProvinceCode = rr.Request.JobLocation.City.Province.Code,
                   WorkDate = rr.WorkDate.Value,
                   Status = rr.Request.Status,
                   IsAsap = rr.Request.IsAsap,
                   WorkerSalary = rr.Request.WorkerSalary,
                   UsesRunners = rr.Request.UsesRunners,
                   RunnersSent = rr.Runners.Count,
                   Runners = rr.Runners.Select(r => new WeeklyBoardRunnerModel
                   {
                       RunnerId = r.Id,
                       WorkerProfileId = r.WorkerProfileId,
                       FullName = r.WorkerProfile.FirstName +
                           (string.IsNullOrWhiteSpace(r.WorkerProfile.MiddleName) ? string.Empty : " " + r.WorkerProfile.MiddleName) +
                           " " + r.WorkerProfile.LastName +
                           (string.IsNullOrWhiteSpace(r.WorkerProfile.SecondLastName) ? string.Empty : " " + r.WorkerProfile.SecondLastName),
                       Email = r.WorkerProfile.Worker.Email,
                       Type = r.Type,
                       Status = r.Status,
                       SentAt = r.CreatedAt
                   }).ToList()
               }).ToListAsync();

    public Task<RequestRecruiter> GetRequestRecruiter(Guid agencyId, Guid requestId, Guid recruiterId, DateTime workDate) =>
        context.RequestRecruiters
            .Include(rr => rr.Runners)
            .SingleOrDefaultAsync(rr => rr.RequestId == requestId
                && rr.RecruiterId == recruiterId
                && rr.WorkDate == workDate.Date
                && rr.Request.CompanyProfile.AgencyId == agencyId);

    public async Task BulkReplaceRecruiters(IEnumerable<Guid> requestIds, IEnumerable<Guid> recruiterIds, DateTime createdAt)
    {
        var requestIdList = requestIds.ToList();
        await context.RequestRecruiters.Where(r => requestIdList.Contains(r.RequestId)).ExecuteDeleteAsync();
        var recruiterIdList = recruiterIds?.ToList() ?? [];
        if (recruiterIdList.Count == 0) return;
        var entities = requestIdList
            .SelectMany(rid => recruiterIdList.Select(recId => new RequestRecruiter(rid, recId, createdAt)))
            .ToList();
        await context.RequestRecruiters.AddRangeAsync(entities);
    }

    public Task<RequestSkill> GetSkill(Guid requestId, Guid id) => context.RequestSkills.SingleOrDefaultAsync(c => c.RequestId == requestId && c.Id == id);

    public async Task<IEnumerable<SkillModel>> GetSkills(Guid requestId) =>
        await context.RequestSkills.Where(c => c.RequestId == requestId)
            .Select(p => new SkillModel { Id = p.Id, Skill = p.Skill })
            .ToListAsync();

    public Task<RequestApplicant> GetRequestApplicant(Expression<Func<RequestApplicant, bool>> expression) => context.RequestApplicants.SingleOrDefaultAsync(expression);

    public async Task<IEnumerable<RequestApplicant>> GetRequestApplicants(Expression<Func<RequestApplicant, bool>> expression)
    {
        var applicants = context.RequestApplicants.Where(expression);
        return await applicants.ToListAsync();
    }

    public async Task<PaginatedList<RequestApplicantDetailModel>> GetRequestApplicants(Guid requestId, GetRequestApplicantFilter filter)
    {
        var query = from rc in context.RequestApplicants.Where(c => c.RequestId == requestId)
                    select new RequestApplicantDetailModel
                    {
                        Id = rc.Id,
                        Comments = rc.Comments,
                        CandidateId = rc.CandidateId,
                        WorkerProfileId = rc.WorkerProfileId,
                        WorkerId = rc.Candidate != null ? null : rc.WorkerProfile.WorkerId,
                        CreatedBy = rc.CreatedBy,
                        CreatedAt = rc.CreatedAt,
                        Name = rc.Candidate != null
                            ? rc.Candidate.Name
                            : rc.WorkerProfile.FirstName + " " + rc.WorkerProfile.MiddleName + " " + rc.WorkerProfile.LastName + " " + rc.WorkerProfile.SecondLastName,
                        PhoneNumber = rc.Candidate != null
                            ? rc.Candidate.PhoneNumbers.FirstOrDefault().PhoneNumber
                            : rc.WorkerProfile.MobileNumber != null ? rc.WorkerProfile.MobileNumber : rc.WorkerProfile.Phone,
                        Email = rc.Candidate != null ? rc.Candidate.Email : rc.WorkerProfile.Worker.Email
                    };
        var predicateNew = ApplyFilterRequestApplicants(filter);
        query = query.Where(predicateNew);
        query = ApplySortRequestApplicants(query, filter);
        var result = await query.ToPaginatedList(filter);
        return result;
    }

    private Expression<Func<RequestApplicantDetailModel, bool>> ApplyFilterRequestApplicants(GetRequestApplicantFilter filter)
    {
        var predicate = PredicateBuilder.New<RequestApplicantDetailModel>(true);
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var fullName = filter.Name.ToLower();
            predicate = predicate.And(ra =>
                ra.Name.ToLower().Contains(fullName) ||
                ra.Email.ToLower().Contains(fullName));
        }
        if (!string.IsNullOrWhiteSpace(filter.Phone))
        {
            var expression = new Regex(@"\s+");
            var phoneWithoutBlankSpaces = expression.Replace(filter.Phone, string.Empty);
            predicate = predicate.And(ra =>
                ra.PhoneNumber.Replace("(", string.Empty).Replace(")", string.Empty).Contains(filter.Phone) ||
                ra.PhoneNumber.Contains(phoneWithoutBlankSpaces));
        }
        if (!string.IsNullOrWhiteSpace(filter.CreatedBy))
            predicate = predicate.And(ra => ra.CreatedBy.ToLower().Contains(filter.CreatedBy.ToLower()));
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(ra => ra.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && ra.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
        return predicate;
    }

    private IQueryable<RequestApplicantDetailModel> ApplySortRequestApplicants(IQueryable<RequestApplicantDetailModel> query, GetRequestApplicantFilter filter)
    {
        switch (filter.SortBy)
        {
            case GetRequestApplicantSortBy.Name:
                query = query.AddOrderBy(filter, ra => ra.Name);
                break;
            case GetRequestApplicantSortBy.CreatedAt:
                query = query.AddOrderBy(filter, ra => ra.CreatedAt);
                break;
        }
        return query;
    }

    private static Expression<Func<WorkerProfile, bool>> BuildWorkerSearchPredicate(
        Guid agencyId, string searchLower, bool isNumericSearch, long numberId,
        Expression<Func<WorkerProfile, bool>> exclusion)
    {
        var predicate = PredicateBuilder.New<WorkerProfile>(wp => wp.AgencyId == agencyId);
        predicate = predicate.And(exclusion);

        var search = PredicateBuilder.New<WorkerProfile>(false);
        search = search.Or(wp => (wp.FirstName + " " + wp.LastName).ToLower().Contains(searchLower));
        if (isNumericSearch)
            search = search.Or(wp => wp.NumberId == numberId);

        return predicate.And(search);
    }

    private static Expression<Func<CandidateEntity, bool>> BuildCandidateSearchPredicate(
        Guid agencyId, string searchLower, bool isNumericSearch, long numberId,
        Expression<Func<CandidateEntity, bool>> exclusion)
    {
        var predicate = PredicateBuilder.New<CandidateEntity>(c => c.AgencyId == agencyId);
        predicate = predicate.And(exclusion);

        var search = PredicateBuilder.New<CandidateEntity>(false);
        search = search.Or(c => c.Name.ToLower().Contains(searchLower));
        search = search.Or(c => c.Email != null && c.Email.ToLower().Contains(searchLower));
        if (isNumericSearch)
            search = search.Or(c => c.NumberId == numberId);

        return predicate.And(search);
    }

    private IQueryable<ApplicantSearchResultModel> BuildWorkerSearchQuery(Expression<Func<WorkerProfile, bool>> workerPredicate) =>
        from wp in context.WorkerProfiles.Where(workerPredicate)
        select new ApplicantSearchResultModel
        {
            WorkerProfileId = wp.Id,
            CandidateId = null,
            NumberId = wp.NumberId,
            Name = wp.FirstName +
                (string.IsNullOrWhiteSpace(wp.MiddleName) ? string.Empty : " " + wp.MiddleName) +
                " " + wp.LastName +
                (string.IsNullOrWhiteSpace(wp.SecondLastName) ? string.Empty : " " + wp.SecondLastName),
            Email = wp.Worker.Email,
            Type = nameof(UserType.Worker),
            ApprovedToWork = wp.ApprovedToWork
        };

    private Task<List<ApplicantSearchResultModel>> SearchWorkers(Expression<Func<WorkerProfile, bool>> workerPredicate) =>
        BuildWorkerSearchQuery(workerPredicate)
            .OrderBy(a => a.Name)
            .Take(20)
            .AsNoTracking()
            .ToListAsync();

    private async Task<List<ApplicantSearchResultModel>> SearchWorkersAndCandidates(
        Expression<Func<WorkerProfile, bool>> workerPredicate,
        Expression<Func<CandidateEntity, bool>> candidatePredicate)
    {
        var workers = BuildWorkerSearchQuery(workerPredicate);

        var candidates = from c in context.Candidates.Where(candidatePredicate)
                         select new ApplicantSearchResultModel
                         {
                             WorkerProfileId = null,
                             CandidateId = c.Id,
                             NumberId = c.NumberId,
                             Name = c.Name,
                             Email = c.Email,
                             Type = nameof(UserType.Candidate),
                             ApprovedToWork = true
                         };

        return await workers.Concat(candidates)
            .OrderBy(a => a.Name)
            .Take(20)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<ApplicantSearchResultModel>> SearchApplicants(Guid agencyId, Guid requestId, string searchTerm)
    {
        var searchLower = searchTerm.ToLower();
        var isNumericSearch = long.TryParse(searchTerm, out var numberId);

        var existingApplicants = context.RequestApplicants.Where(ra => ra.RequestId == requestId);
        var bookedWorkers = context.WorkerRequests.Where(wr => wr.RequestId == requestId && wr.WorkerRequestStatus == WorkerRequestStatus.Booked);

        var workerPredicate = BuildWorkerSearchPredicate(agencyId, searchLower, isNumericSearch, numberId,
            wp => !existingApplicants.Any(ra => ra.WorkerProfileId == wp.Id) && !bookedWorkers.Any(wr => wr.WorkerProfileId == wp.Id));
        var candidatePredicate = BuildCandidateSearchPredicate(agencyId, searchLower, isNumericSearch, numberId,
            c => !existingApplicants.Any(ra => ra.CandidateId == c.Id));

        return SearchWorkersAndCandidates(workerPredicate, candidatePredicate);
    }

    public Task<List<ApplicantSearchResultModel>> SearchRunnerProspects(Guid agencyId, Guid requestId, string searchTerm)
    {
        var searchLower = searchTerm.ToLower();
        var isNumericSearch = long.TryParse(searchTerm, out var numberId);

        var existingRunners = context.Runners.Where(r => r.RequestId == requestId);

        var workerPredicate = BuildWorkerSearchPredicate(agencyId, searchLower, isNumericSearch, numberId,
            wp => !existingRunners.Any(r => r.WorkerProfileId == wp.Id));

        return SearchWorkers(workerPredicate);
    }

    public async Task<RequestComission> GetRequestComission(Guid requestId) => await context.RequestComissions.FirstOrDefaultAsync(rc => rc.RequestId == requestId);

    public async Task<IEnumerable<RequestCompanyUser>> GetRequestCompanyUsers(Guid requestId) => await context.RequestCompanyUsers.Where(rcu => rcu.RequestId == requestId).ToListAsync();

    public async Task<IEnumerable<CompanyProfileListModel>> GetCompaniesWithRequests(IEnumerable<Guid> agencyIds)
    {
        var query = from r in context.Requests
                    where agencyIds.Contains(r.CompanyProfile.AgencyId)
                    select new CompanyProfileListModel
                    {
                        Id = r.CompanyProfileId,
                        CompanyId = r.CompanyProfile.CompanyId,
                        FullName = r.CompanyProfile.FullName,
                    };
        var result = await query.Distinct().OrderBy(q => q.FullName).ToListAsync();
        return result;
    }

    public async Task<IEnumerable<Common.Entities.Request.Request>> GetRequests(IEnumerable<Guid> ids)
    {
        var requests = context.Requests.Where(r => ids.Contains(r.Id));
        return await requests.ToListAsync();
    }

    public async Task<bool> ExistsRequestByNumber(int requestId)
    {
        var result = await context.Requests.AnyAsync(r => r.NumberId == requestId);
        return result;
    }

    public async Task<bool> RequestBelongsToCompany(Guid requestId, Guid companyId)
    {
        return await context.Requests.AnyAsync(r => r.Id == requestId && r.CompanyProfile.CompanyId == companyId);
    }
}