using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.WebSite;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Mappers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Covenant.Infrastructure.Repositories.Request;

public class RequestRepository : IRequestRepository
{
    private readonly CovenantContext _context;
    private readonly FilesConfiguration filesConfiguration;
    private readonly ITimeService _timeService;

    public RequestRepository(CovenantContext context,
        IOptions<FilesConfiguration> options,
        ITimeService timeService)
    {
        _context = context;
        filesConfiguration = options.Value;
        _timeService = timeService;
    }

    public async Task Create<T>(T entity) where T : class => await _context.Set<T>().AddAsync(entity);

    public void Delete<T>(T entity) where T : class => _context.Set<T>().Remove(entity);

    public Task Update<T>(T entity) where T : class => Task.FromResult(_context.Set<T>().Update(entity));

    public IEnumerable<AgencyRequestListModel> GetAllRequestsForAgency(Guid agencyId, GetRequestForAgencyFilter filter)
    {
        var requests = _context.Request.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.Recruiter))
        {
            requests = from r in requests
                       join rr in _context.RequestRecruiter.Where(c => c.Recruiter.User.Email.ToLower() == filter.Recruiter.ToLower()) on r.Id equals rr.RequestId
                       select r;
        }
        if (filter.CompanyId.HasValue)
            requests = requests.Where(r => r.CompanyId == filter.CompanyId.Value);
        else
            requests = requests.Where(r => r.AgencyId == agencyId);
        var query = from r in requests
                    join cp in _context.CompanyProfile on r.CompanyId equals cp.CompanyId
                    join cf in _context.CovenantFile on cp.LogoId equals cf.Id into tmp
                    from cfl in tmp.DefaultIfEmpty()
                    join rc in _context.RequestComissions on r.Id equals rc.RequestId into tmp1
                    from rrc in tmp1.DefaultIfEmpty()
                    select new AgencyRequestListModel
                    {
                        Id = r.Id,
                        NumberId = r.NumberId,
                        AgencyId = r.AgencyId,
                        JobTitle = r.JobTitle,
                        BillingTitle = r.BillingTitle,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        FinishAt = r.FinishAt,
                        StartAt = r.StartAt,
                        DurationTerm = r.DurationTerm.ToString(),
                        EmploymentType = r.EmploymentType.ToString(),
                        Address = r.JobLocation.Address,
                        City = r.JobLocation.City.Value,
                        ProvinceCode = r.JobLocation.City.Province.Code,
                        PostalCode = r.JobLocation.PostalCode,
                        Entrance = r.JobLocation.Entrance,
                        CompanyFullName = cp.BusinessName,
                        CompanyProfileId = cp.Id,
                        Status = r.Status.ToString(),
                        RequestStatus = r.Status,
                        IsOpen = r.IsOpen,
                        IsAsap = r.IsAsap,
                        WorkerRate = r.WorkerSalary.HasValue ? r.WorkerSalary : r.WorkerRate,
                        WorkerSalary = r.WorkerSalary,
                        DisplayRecruiters = r.DisplayRecruiters,
                        WorkersQuantity = r.WorkersQuantity,
                        SalesRepresentative = rrc != null ? rrc.AgencyPersonnel.Name : null,
                        WorkersQuantityWorking = r.WorkersQuantityWorking,
                        DisplayShift = r.Shift == null ? null : r.Shift.DisplayShift,
                        NotesCount = r.Notes.Count(n => !n.Note.IsDeleted),
                        VaccinationRequired = cp.VaccinationRequired.GetValueOrDefault(),
                        PunchCardOptionEnabled = r.PunchCardOptionEnabled,
                        HasPermissionToSeeInternalOrders = cp.RequiresPermissionToSeeOrders,
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

    private Expression<Func<AgencyRequestListModel, bool>> ApplyFilterForAgency(GetRequestForAgencyFilter filter)
    {
        var predicate = PredicateBuilder.New<AgencyRequestListModel>(true);
        if (!filter.HasPermissionToSeeInternalOrders)
            predicate = predicate.And(p => p.HasPermissionToSeeInternalOrders == false);
        if (filter.NumberId.HasValue)
            predicate = predicate.And(r => r.NumberId == filter.NumberId.Value);
        if (!string.IsNullOrWhiteSpace(filter.CompanyFullName))
        {
            var criteria = filter.CompanyFullName.ToLower();
            predicate = predicate.And(r =>
                r.CompanyFullName.ToLower().Contains(criteria) ||
                r.Address.ToLower().Contains(criteria) ||
                r.City.ToLower().Contains(criteria) ||
                r.ProvinceCode.ToLower().Contains(criteria) ||
                r.PostalCode.ToLower().Contains(criteria));
        }
        if (!string.IsNullOrWhiteSpace(filter.JobTitle))
            predicate = predicate.And(r => r.JobTitle.ToLower().Contains(filter.JobTitle.ToLower()));
        if (!string.IsNullOrWhiteSpace(filter.DisplayRecruiters))
            predicate = predicate.And(r => r.DisplayRecruiters.ToLower().Contains(filter.DisplayRecruiters.ToLower()));
        if (!string.IsNullOrWhiteSpace(filter.SalesRepresentative))
            predicate = predicate.And(r => r.SalesRepresentative.ToLower().Contains(filter.SalesRepresentative.ToLower()));
        if (filter.LastUpdateFrom.HasValue && filter.LastUpdateTo.HasValue)
            predicate = predicate.And(r => r.UpdatedAt.Value.Date >= filter.LastUpdateFrom.Value.Date && r.UpdatedAt.Value.Date <= filter.LastUpdateTo.Value.Date);
        if (filter.StartAtFrom.HasValue && filter.StartAtTo.HasValue)
            predicate = predicate.And(r => r.StartAt.Value.Date >= filter.StartAtFrom.Value.Date && r.StartAt.Value.Date <= filter.StartAtTo.Value.Date);
        if (filter.RateFrom.HasValue && filter.RateTo.HasValue)
            predicate = predicate.And(r => r.WorkerRate >= filter.RateFrom.Value && r.WorkerRate <= filter.RateTo.Value);
        if (filter.Statuses != null && filter.Statuses.Any())
        {
            if (filter.Statuses.Any(s => s != RequestStatus.Open && s != RequestStatus.NoOpen))
                predicate = predicate.And(r => filter.Statuses.Contains(r.RequestStatus));
            if (filter.Statuses.Count(s => s == RequestStatus.Open || s == RequestStatus.NoOpen) < 2)
            {
                if (filter.Statuses.Any(s => s == RequestStatus.Open))
                    predicate = predicate.And(r => r.IsOpen);
                else if (filter.Statuses.Any(s => s == RequestStatus.NoOpen))
                    predicate = predicate.And(r => !r.IsOpen);
            }
        }
        if (!string.IsNullOrWhiteSpace(filter.Filter))
            predicate = predicate.And(r =>
            r.NumberId.ToString().Contains(filter.Filter) ||
            r.JobTitle.ToLower().Contains(filter.Filter) ||
            r.CompanyFullName.ToLower().Contains(filter.Filter));
        return predicate;
    }

    private IQueryable<AgencyRequestListModel> ApplySortForAgency(IQueryable<AgencyRequestListModel> query, GetRequestForAgencyFilter filter)
    {
        switch (filter.SortBy)
        {
            case GetRequestSortBy.Client:
                query = query.AddOrderBy(filter, o => o.CompanyFullName);
                break;
            case GetRequestSortBy.JobTitle:
                query = query.AddOrderBy(filter, o => o.JobTitle);
                break;
            case GetRequestSortBy.StartAt:
                query = query.AddOrderBy(filter, o => o.StartAt);
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
            case GetRequestSortBy.UpdatedAt:
                query = query.AddOrderBy(filter, o => o.UpdatedAt);
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
        var openStatus = new RequestStatus[] { RequestStatus.Requested, RequestStatus.InProcess };
        var requests = _context.Request.Include(r => r.Shift)
            .Include(r => r.JobLocation).ThenInclude(jl => jl.City).ThenInclude(c => c.Province).ThenInclude(p => p.Country)
            .Join(_context.CompanyProfile.Where(cp => cp.Active), r => r.CompanyId, cp => cp.CompanyId, (r, cp) => r)
            .Where(r => openStatus.Contains(r.Status))
            .Where(r => r.IsOpen)
            .Where(r => countries.Contains(r.JobLocation.City.Province.Country.Code))
            .Select(r => new JobViewModel
            {
                RequestId = r.Id,
                NumberId = r.NumberId.ToString(),
                Description = r.Description,
                Requirements = r.Requirements,
                Responsibilities = r.Responsibilities,
                Location = r.JobLocation.City.Value + ", " + r.JobLocation.City.Province.Code,
                Salary = r.WorkerRate.HasValue ? r.WorkerRate.Value.ToString("C") : r.WorkerSalary.Value.ToString("C"),
                Title = r.JobTitle,
                Type = Regex.Replace(r.EmploymentType.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).Trim(),
                CreatedAt = r.CreatedAt,
                Shift = r.Shift == null ? string.Empty : r.Shift.DisplayShift
            });
        return await requests.OrderBy(r => r.Title).ToListAsync();
    }

    public Task<PaginatedList<WorkerRequestAgencyBoardModel>> GetWorkersRequestBoard(Guid agencyId, Pagination pagination)
    {
        return (from wr in _context.WorkerRequest
                join r in _context.Request.Where(wR => wR.AgencyId == agencyId) on wr.RequestId equals r.Id
                join wp in _context.WorkerProfile on new { WId = wr.WorkerId, AId = r.AgencyId } equals new { WId = wp.WorkerId, AId = wp.AgencyId }
                join cp in _context.CompanyProfile on new { CId = r.CompanyId, AId = r.AgencyId } equals new { CId = cp.CompanyId, AId = cp.AgencyId }
                orderby wr.StartWorking descending
                select new WorkerRequestAgencyBoardModel
                {
                    Id = wr.Id,
                    StartWorking = wr.StartWorking,
                    WeekStartWorking = wr.WeekStartWorking,
                    WorkerRequestStatus = wr.WorkerRequestStatus.ToString(),
                    RejectComments = wr.RejectComments,
                    RejectedAt = wr.RejectedAt,
                    RequestId = r.Id,
                    NumberId = r.NumberId,
                    RequestStatus = r.Status.ToString(),
                    JobTitle = r.JobTitle,
                    WorkerRate = r.WorkerRate,
                    WorkerSalary = r.WorkerSalary,
                    DurationTerm = r.DurationTerm.ToString(),
                    DisplayRecruiters = r.DisplayRecruiters,
                    Location = $"{r.JobLocation.Address} {r.JobLocation.City.Value} {r.JobLocation.City.Province.Code} {r.JobLocation.PostalCode}",
                    Entrance = r.JobLocation.Entrance,
                    DisplayShift = r.Shift == null ? null : r.Shift.DisplayShift,
                    WorkerProfileId = wp.Id,
                    WorkerId = wp.WorkerId,
                    FirstName = wp.FirstName,
                    MiddleName = wp.MiddleName,
                    LastName = wp.LastName,
                    SecondLastName = wp.SecondLastName,
                    SocialInsurance = wp.SocialInsurance,
                    SocialInsuranceExpire = wp.SocialInsuranceExpire,
                    DueDate = wp.DueDate,
                    MobileNumber = wp.MobileNumber,
                    IsSubcontractor = wp.IsSubcontractor,
                    CompanyProfileId = cp.Id,
                    CompanyFullName = cp.FullName,
                    NotesCount = wr.Notes.Count(n => !n.Note.IsDeleted)
                }).ToPaginatedList(pagination);
    }

    public async Task<AgencyRequestDetailModel> GetRequestDetailForAgency(Guid id)
    {
        var requests = _context.Request
            .Include(r => r.JobLocation)
            .ThenInclude(jl => jl.City)
            .ThenInclude(c => c.Province)
            .ThenInclude(p => p.Country)
            .Where(c => c.Id == id);
        var query = from r in requests
                    join cpj in _context.CompanyProfileJobPositionRate on r.JobPositionRateId equals cpj.Id into tmp1
                    from cpj in tmp1.DefaultIfEmpty()
                    join jp in _context.JobPosition on cpj.JobPositionId equals jp.Id into tmp2
                    from jp in tmp2.DefaultIfEmpty()
                    join cp in _context.CompanyProfile on r.CompanyId equals cp.CompanyId
                    join cf in _context.CovenantFile on cp.LogoId equals cf.Id into tmp
                    from cfl in tmp.DefaultIfEmpty()
                    join rcd in _context.RequestCancellationDetail on r.Id equals rcd.RequestId
                    into tmpRcd
                    from rcd in tmpRcd.DefaultIfEmpty()
                    join rc in _context.RequestComissions on r.Id equals rc.RequestId into tmp3
                    from rc in tmp3.DefaultIfEmpty()
                    select new AgencyRequestDetailModel
                    {
                        Id = r.Id,
                        NumberId = r.NumberId,
                        JobTitle = r.JobTitle,
                        BillingTitle = string.IsNullOrWhiteSpace(r.BillingTitle) ? r.JobTitle : r.BillingTitle,
                        Status = r.Status.ToString(),
                        CancellationDetail = rcd == null ? null : rcd.OtherReasonCancellationRequest,
                        CompanyLogo = cfl == null ? null : $"{filesConfiguration.FilesPath}{cfl.FileName}",
                        CompanyProfileId = cp.Id,
                        FullName = cp.FullName,
                        Description = r.Description,
                        Requirements = r.Requirements,
                        Responsibilities = r.Responsibilities,
                        WorkersQuantity = r.WorkersQuantity,
                        WorkersQuantityWorking = r.WorkersQuantityWorking,
                        AgencyRate = r.AgencyRate,
                        WorkerRate = r.WorkerRate,
                        WorkerSalary = r.WorkerSalary,
                        JobPositionId = r.JobPositionRateId,
                        JobPosition = jp != null ? jp.Value : cpj != null ? cpj.OtherJobPosition : r.JobTitle,
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
                        DurationTerm = r.DurationTerm.ToString(),
                        EmploymentType = r.EmploymentType.ToString(),
                        DisplayRecruiters = r.DisplayRecruiters,
                        DisplayShift = r.Shift == null ? null : r.Shift.DisplayShift,
                        IsAsap = r.IsAsap,
                        VaccinationRequired = cp.VaccinationRequired,
                        PunchCardOptionEnabled = r.PunchCardOptionEnabled,
                        InternalRequirements = r.InternalRequirements,
                        SalesRepresentativeId = rc != null ? rc.AgencyPersonnelId : null,
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
        var requests = _context.Request.AsQueryable();
        if (filter.CompanyUserId.HasValue)
            requests = requests.Where(r => r.RequestCompanyUser.Any(rcu => rcu.CompanyUserId == filter.CompanyUserId));
        var query = from request in requests
                    join agency in _context.Agencies on request.AgencyId equals agency.Id
                    select new RequestListModel
                    {
                        CompanyId = request.CompanyId,
                        Id = request.Id,
                        NumberId = request.NumberId,
                        JobTitle = request.JobTitle,
                        CreatedAt = request.CreatedAt,
                        WorkersQuantity = request.WorkersQuantity,
                        WorkersQuantityWorking = request.WorkersQuantityWorking,
                        Location = request.JobLocation.Address + " " + request.JobLocation.City.Value + " " + request.JobLocation.City.Province.Code + " " + request.JobLocation.PostalCode,
                        Entrance = request.JobLocation.Entrance,
                        CompanyFullName = agency.FullName,
                        RequestStatus = request.Status,
                        Status = request.Status.ToString(),
                        IsAsap = request.IsAsap,
                        IsDirectHiring = request.WorkerSalary.HasValue,
                        DisplayShift = request.Shift == null ? null : request.Shift.DisplayShift
                    };
        var predicateNew = ApplyFilterForCompany(companyId, filter);
        query = query.Where(predicateNew);
        query = ApplySortForCompany(query, filter);
        return await query.ToPaginatedList(filter);
    }

    private Expression<Func<RequestListModel, bool>> ApplyFilterForCompany(Guid companyId, GetRequestForCompanyFilter filter)
    {
        var statusToVisualize = new RequestStatus[] { RequestStatus.Requested, RequestStatus.InProcess };
        Expression<Func<RequestListModel, bool>> predicate = r => r.CompanyId == companyId && statusToVisualize.Contains(r.RequestStatus);
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
        (from r in _context.Request.Where(c => c.Id == id)
         join cpj in _context.CompanyProfileJobPositionRate on r.JobPositionRateId equals cpj.Id into tmp1
         from cpj in tmp1.DefaultIfEmpty()
         join jp in _context.JobPosition on cpj.JobPositionId equals jp.Id into tmp2
         from jp in tmp2.DefaultIfEmpty()
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
                 Value = jp != null ? jp.Value : cpj != null ? cpj.OtherJobPosition : r.JobTitle
             },
             AgencyRate = r.AgencyRate,
             WorkerSalary = r.WorkerSalary,
             CreatedAt = r.CreatedAt,
             Status = r.Status.ToString(),
             DurationTerm = r.DurationTerm.ToString(),
             DisplayShift = r.Shift == null ? null : r.Shift.DisplayShift,
             StartAt = r.StartAt,
             FinishAt = r.FinishAt
         }).SingleOrDefaultAsync();

    public Task<Common.Entities.Request.Request> GetRequest(Expression<Func<Common.Entities.Request.Request, bool>> condition) =>
        _context.Request.Where(condition)
            .Include(r => r.JobPositionRate).ThenInclude(c => c.JobPosition)
            .Include(r => r.JobLocation).ThenInclude(l => l.City).ThenInclude(c => c.Province)
            .Include(r => r.Workers)
            .Include(r => r.Shift)
            .Include(i => i.Recruiters).ThenInclude(ti => ti.Recruiter).ThenInclude(u => u.User)
            .SingleOrDefaultAsync();

    public async Task<PaginatedList<AgencyWorkerRequestModel>> GetWorkersRequestByRequestId(Guid requestId, GetWorkersRequestFilter filter)
    {
        var query = from wr in _context.WorkerRequest
                    join wp in _context.WorkerProfile on wr.WorkerId equals wp.WorkerId
                    join cf in _context.CovenantFile on wp.ProfileImageId equals cf.Id into tmp
                    from cfl in tmp.DefaultIfEmpty()
                    select new AgencyWorkerRequestModel
                    {
                        Id = wr.Id,
                        RequestId = wr.RequestId,
                        NumberId = wp.NumberId,
                        WorkerId = wr.WorkerId,
                        WorkerProfileId = wp.Id,
                        Name =
                            wp.FirstName +
                            (string.IsNullOrWhiteSpace(wp.MiddleName) ? string.Empty : " " + wp.MiddleName) +
                            " " + wp.LastName +
                            (string.IsNullOrWhiteSpace(wp.SecondLastName) ? string.Empty : " " + wp.SecondLastName),
                        ProfileImage = cfl == null ? null : $"{filesConfiguration.FilesPath}{cfl.FileName}",
                        Status = wr.WorkerRequestStatus.ToString(),
                        WorkerRequestStatus = wr.WorkerRequestStatus,
                        RejectComments = wr.RejectComments,
                        RejectedAt = wr.RejectedAt,
                        RejectedBy = wr.RejectedBy,
                        ApprovedToWork = wp.ApprovedToWork,
                        IsSubcontractor = wp.IsSubcontractor,
                        SocialInsurance = wp.SocialInsurance,
                        DueDate = wp.DueDate,
                        SocialInsuranceExpire = wp.SocialInsuranceExpire,
                        MobileNumber = wp.MobileNumber,
                        CreatedBy = wr.CreatedBy,
                        CreatedAt = wr.CreatedAt,
                        StartWorking = wr.StartWorking,
                        NotesCount = wr.Notes.Count(n => !n.Note.IsDeleted),
                        TotalHoursApproved = wr.TimeSheets.Where(c => c.TimeOutApproved != null && c.TimeInApproved != null).Sum(s => (s.TimeOutApproved - s.TimeInApproved).Value.TotalHours),
                        TotalHoursWorker = wr.TimeSheets.Where(c => c.TimeOut != null).Sum(c => (c.TimeOut - c.TimeIn).Value.TotalHours)
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
                query = query.AddOrderBy(filter, wr => wr.WorkerRequestStatus);
                break;
            case GetWorkersRequestSortBy.StartWorking:
                query = query.AddOrderBy(filter, wr => wr.StartWorking);
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
        }
        return query;
    }

    public Task<AgencyWorkerRequestModel> GetWorkerRequestByAgencyId(Guid agencyId, Guid requestId, Guid workerRequestId)
    {
        IQueryable<AgencyWorkerRequestModel> query = from wr in _context.WorkerRequest.Where(c => c.Id == workerRequestId && c.RequestId == requestId)
                                                     join wp in _context.WorkerProfile.Where(c => c.AgencyId == agencyId) on wr.WorkerId equals wp.WorkerId
                                                     join cf in _context.CovenantFile on wp.ProfileImageId equals cf.Id into tmp
                                                     from cfl in tmp.DefaultIfEmpty()
                                                     where wr.WorkerId == wp.WorkerId
                                                     select new AgencyWorkerRequestModel
                                                     {
                                                         Id = wr.Id,
                                                         WorkerId = wr.WorkerId,
                                                         WorkerProfileId = wp.Id,
                                                         Name = $"{wp.FirstName} {wp.MiddleName} {wp.LastName} {wp.SecondLastName}",
                                                         Status = wr.WorkerRequestStatus.ToString(),
                                                         RejectComments = wr.RejectComments,
                                                         RejectedAt = wr.RejectedAt,
                                                         ProfileImage = cfl == null ? null : $"{filesConfiguration.FilesPath}{cfl.FileName}",
                                                         NumberId = wp.NumberId,
                                                         ApprovedToWork = wp.ApprovedToWork,
                                                         SocialInsurance = wp.SocialInsurance,
                                                         DueDate = wp.DueDate,
                                                         SocialInsuranceExpire = wp.SocialInsuranceExpire
                                                     };
        return query.AsNoTracking().SingleOrDefaultAsync();
    }

    public Task<PaginatedList<AgencyWorkerRequestModel>> GetAllWorkersThatCanApplyToRequest(Guid agencyId, Guid requestId, Pagination pagination, string filter)
    {
        IQueryable<WorkerProfile> workers = _context.WorkerProfile.Where(c => c.AgencyId == agencyId);
        if (!string.IsNullOrEmpty(filter))
        {
            string textSearch = filter.GetTextSearch();
            workers = workers.Where(p => EF.Functions.ToTsVector(p.TextSearch).Matches(EF.Functions.ToTsQuery(textSearch)));
        }

        IQueryable<AgencyWorkerRequestModel> query = from wp in workers
                                                     join cf in _context.CovenantFile on wp.ProfileImageId equals cf.Id
                                                         into tmp1
                                                     from cfl in tmp1.DefaultIfEmpty()
                                                     join l in _context.Location on wp.LocationId equals l.Id
                                                     join c in _context.City on l.CityId equals c.Id
                                                     join p in _context.Province on c.ProvinceId equals p.Id
                                                     join wr in _context.WorkerRequest.Where(c => c.RequestId == requestId) on wp.WorkerId equals wr.WorkerId
                                                         into tmp
                                                     from wr in tmp.DefaultIfEmpty()
                                                     orderby wp.FirstName
                                                     select new AgencyWorkerRequestModel
                                                     {
                                                         NumberId = wp.NumberId,
                                                         Id = wr == null ? Guid.Empty : wr.Id,
                                                         WorkerId = wp.WorkerId,
                                                         WorkerProfileId = wp.Id,
                                                         Name =
                                                            wp.FirstName +
                                                            (string.IsNullOrWhiteSpace(wp.MiddleName) ? string.Empty : " " + wp.MiddleName) +
                                                            " " + wp.LastName +
                                                            (string.IsNullOrWhiteSpace(wp.SecondLastName) ? string.Empty : " " + wp.SecondLastName),
                                                         Status = wr == null ? string.Empty : wr.WorkerRequestStatus.ToString(),
                                                         ProfileImage = cfl == null ? null : $"{filesConfiguration.FilesPath}{cfl.FileName}",
                                                         ApprovedToWork = wp.ApprovedToWork,
                                                         SocialInsurance = wp.SocialInsurance,
                                                         SocialInsuranceExpire = wp.SocialInsuranceExpire,
                                                         DueDate = wp.DueDate,
                                                         Address = $"{l.Address} {c.Value} {p.Code} {l.PostalCode}",
                                                         MobileNumber = wp.MobileNumber
                                                     };
        return query.AsNoTracking().ToPaginatedList(pagination);
    }

    public Task<PaginatedList<WorkerRequestListModel>> GetRequestsHistoryForWorker(Guid workerId, Pagination pagination) =>
        (from wp in _context.WorkerProfile.Where(c => c.WorkerId == workerId)
         join r in _context.Request.Where(r => r.Status == RequestStatus.Cancelled) on wp.AgencyId equals r.AgencyId
         join wr in _context.WorkerRequest.Where(c => c.WorkerId == workerId) on r.Id equals wr.RequestId
         join agency in _context.Agencies on r.AgencyId equals agency.Id
         join afl in _context.CovenantFile on agency.LogoId equals afl.Id into tmp3
         from afl in tmp3.DefaultIfEmpty()
         orderby r.NumberId descending
         select new WorkerRequestListModel
         {
             Id = r.Id,
             NumberId = r.NumberId,
             IsAsap = r.IsAsap,
             AgencyFullName = agency.FullName,
             AgencyLogo = afl == null ? null : $"{filesConfiguration.FilesPath}{afl.FileName}",
             CreatedAt = r.CreatedAt,
             JobTitle = r.JobTitle,
             WorkerRate = r.WorkerRate,
             WorkerSalary = r.WorkerSalary,
             WorkerApprovedToWork = wp.ApprovedToWork.ToString(),
             Location = $"{r.JobLocation.Address} {r.JobLocation.City.Value} {r.JobLocation.City.Province.Code} {r.JobLocation.PostalCode}",
             Entrance = r.JobLocation.Entrance,
             Status = wr.WorkerRequestStatus.ToString(),
             WorkersQuantity = r.WorkersQuantity,
             StartAt = r.StartAt,
             FinishAt = r.FinishAt,
             DurationTerm = r.DurationTerm.ToString()
         }).AsNoTracking().ToPaginatedList(pagination);

    public async Task<PaginatedList<WorkerRequestListModel>> GetRequestsForWorker(Guid workerId, Pagination pagination)
    {
        var workerProfile = await _context.WorkerProfile.FirstOrDefaultAsync(wp => wp.WorkerId == workerId);
        var openStatus = new RequestStatus[] { RequestStatus.Requested, RequestStatus.InProcess };
        var requests = Enumerable.Empty<WorkerRequestListModel>();
        var ownRequest = _context.Request.Include(wr => wr.Agency)
            .Join(_context.WorkerRequest.Where(wr => wr.WorkerId == workerId && wr.WorkerRequestStatus == WorkerRequestStatus.Booked), r => r.Id, wr => wr.RequestId, (r, wr) => new { r, wr })
            .GroupJoin(_context.CovenantFile, lj => lj.r.Agency.LogoId, cf => cf.Id, (lj, cf) => new { lj, cf })
            .SelectMany(lj => lj.cf.DefaultIfEmpty(), (lj, cf) => new WorkerRequestListModel
            {
                Id = lj.lj.r.Id,
                NumberId = lj.lj.r.NumberId,
                IsAsap = lj.lj.r.IsAsap,
                AgencyFullName = lj.lj.r.Agency.FullName,
                AgencyLogo = cf == null ? null : $"{filesConfiguration.FilesPath}{cf.FileName}",
                CreatedAt = lj.lj.r.CreatedAt,
                JobTitle = lj.lj.r.JobTitle,
                WorkerRate = lj.lj.r.WorkerRate.HasValue ? lj.lj.r.WorkerRate : lj.lj.r.WorkerSalary,
                WorkerSalary = lj.lj.r.WorkerSalary,
                Location = $"{lj.lj.r.JobLocation.City.Value} {lj.lj.r.JobLocation.City.Province.Code}",
                Entrance = lj.lj.r.JobLocation.Entrance,
                Status = lj.lj.wr.WorkerRequestStatus.ToString(),
                WorkersQuantity = lj.lj.r.WorkersQuantity,
                StartAt = lj.lj.r.StartAt,
                FinishAt = lj.lj.r.FinishAt,
                DurationTerm = lj.lj.r.DurationTerm.ToString()
            }).AsEnumerable();
        if (!ownRequest.Any())
        {
            var requestToExclude = _context.WorkerRequest.Where(wr => wr.WorkerId == workerId && wr.WorkerRequestStatus == WorkerRequestStatus.Rejected);
            var availableRequest = _context.Request.Include(r => r.Agency)
                .Where(r => r.IsOpen && openStatus.Contains(r.Status))
                .Where(r => r.AgencyId == workerProfile.AgencyId)
                .Where(r => !requestToExclude.Any(rte => rte.RequestId == r.Id))
                .Where(r => !ownRequest.Any(or => or.Id == r.Id))
                .GroupJoin(_context.CovenantFile, r => r.Agency.LogoId, cf => cf.Id, (r, cf) => new { r, cf })
                .SelectMany(lj => lj.cf.DefaultIfEmpty(), (lj, cf) => new WorkerRequestListModel
                {
                    Id = lj.r.Id,
                    NumberId = lj.r.NumberId,
                    IsAsap = lj.r.IsAsap,
                    AgencyFullName = lj.r.Agency.FullName,
                    AgencyLogo = cf == null ? null : $"{filesConfiguration.FilesPath}{cf.FileName}",
                    CreatedAt = lj.r.CreatedAt,
                    JobTitle = lj.r.JobTitle,
                    WorkerRate = lj.r.WorkerRate.HasValue ? lj.r.WorkerRate : lj.r.WorkerSalary,
                    WorkerSalary = lj.r.WorkerSalary,
                    Location = $"{lj.r.JobLocation.City.Value} {lj.r.JobLocation.City.Province.Code}",
                    Entrance = lj.r.JobLocation.Entrance,
                    Status = null,
                    WorkersQuantity = lj.r.WorkersQuantity,
                    StartAt = lj.r.StartAt,
                    FinishAt = lj.r.FinishAt,
                    DurationTerm = lj.r.DurationTerm.ToString()
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
        return (from r in _context.Request.Where(c => c.Id == requestId)
                join cpj in _context.CompanyProfileJobPositionRate on r.JobPositionRateId equals cpj.Id into tmp1
                from cpj in tmp1.DefaultIfEmpty()
                join jp in _context.JobPosition on cpj.JobPositionId equals jp.Id into tmp2
                from jp in tmp2.DefaultIfEmpty()
                join wp in _context.WorkerProfile.Where(c => c.WorkerId == workerId) on r.AgencyId equals wp.AgencyId
                join wr in _context.WorkerRequest.Where(c => c.WorkerId == workerId && c.RequestId == requestId) on r.Id equals wr.RequestId into tmp
                from wr in tmp.DefaultIfEmpty()
                join l in _context.Location on r.JobLocationId equals l.Id
                join agency in _context.Agencies on r.AgencyId equals agency.Id
                join afl in _context.CovenantFile on agency.LogoId equals afl.Id into tmp3
                from afl in tmp3.DefaultIfEmpty()
                join ra in _context.RequestApplicant on new { rId = r.Id, wpId = wp.Id } equals new { rId = ra.RequestId, wpId = ra.WorkerProfileId.Value }
                    into tmpRa
                from ra in tmpRa.DefaultIfEmpty()
                select new WorkerRequestDetailModel
                {
                    Id = r.Id,
                    JobTitle = r.JobTitle,
                    Status = wr == null ? "None" : wr.WorkerRequestStatus.ToString(),
                    DurationTerm = r.DurationTerm.ToString(),
                    RequestStatus = r.Status.ToString(),
                    AgencyFullName = agency.FullName,
                    AgencyLogo = afl == null ? null : filesConfiguration.FilesPath + afl.FileName,
                    Description = r.Description,
                    Requirements = r.Requirements,
                    WorkersQuantity = r.WorkersQuantity,
                    WorkerRate = r.WorkerRate,
                    WorkerSalary = r.WorkerSalary,
                    JobPosition = jp != null ? jp.Value : cpj != null ? cpj.OtherJobPosition : r.JobTitle,
                    HolidayIsPaid = r.HolidayIsPaid,
                    BreakIsPaid = r.BreakIsPaid,
                    CreatedAt = r.CreatedAt,
                    StartAt = r.StartAt,
                    FinishAt = r.FinishAt,
                    Incentive = r.Incentive,
                    IncentiveDescription = r.IncentiveDescription,
                    DurationBreak = r.DurationBreak,
                    Location = l.Address + " " + l.City.Value + " " + l.City.Province.Code + " " + l.PostalCode,
                    IsApplicant = ra != null,
                    PunchCardOptionEnabled = r.PunchCardOptionEnabled,
                    JobLocation = new LocationDetailModel
                    {
                        Id = l.Id,
                        Address = l.Address,
                        Latitude = l.Latitude,
                        Longitude = l.Longitude,
                        PostalCode = l.PostalCode,
                        Entrance = l.Entrance,
                        MainIntersection = l.MainIntersection,
                        City = new CityModel
                        {
                            Id = l.City.Id,
                            Value = l.City.Value,
                            Code = l.City.Code,
                            Province = new ProvinceModel
                            {
                                Id = l.City.Province.Id,
                                Value = l.City.Province.Value,
                                Code = l.City.Province.Code,
                                Country = new CountryModel
                                {
                                    Id = l.City.Province.Country.Id,
                                    Value = l.City.Province.Country.Value,
                                    Code = l.City.Province.Country.Code
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
        var query = from wp in _context.WorkerProfile.Where(filter)
                    join wr in _context.WorkerRequest on wp.WorkerId equals wr.WorkerId
                    join r in _context.Request on new { Ag = wp.AgencyId, Re = wr.RequestId } equals new { Ag = r.AgencyId, Re = r.Id }
                    join cp in _context.CompanyProfile on r.CompanyId equals cp.CompanyId
                    join cf in _context.CovenantFile on cp.LogoId equals cf.Id into tmp
                    from cfl in tmp.DefaultIfEmpty()
                    join agency in _context.Agencies on r.AgencyId equals agency.Id
                    join afl in _context.CovenantFile on agency.LogoId equals afl.Id into tmp3
                    from afl in tmp3.DefaultIfEmpty()
                    orderby r.NumberId descending
                    select new RequestListModel
                    {
                        Id = r.Id,
                        NumberId = r.NumberId,
                        IsAsap = r.IsAsap,
                        CompanyFullName = cp.FullName,
                        Logo = cfl == null ? null : $"{filesConfiguration.FilesPath}{cfl.FileName}",
                        AgencyFullName = agency.FullName,
                        AgencyLogo = afl == null ? null : $"{filesConfiguration.FilesPath}{afl.FileName}",
                        CreatedAt = r.CreatedAt,
                        JobTitle = r.JobTitle,
                        Location = $"{r.JobLocation.Address} {r.JobLocation.City.Value} {r.JobLocation.City.Province.Code} {r.JobLocation.PostalCode}",
                        Status = r.Status.ToString(),
                        WorkersQuantity = r.WorkersQuantity,
                        WorkersQuantityWorking = r.WorkersQuantityWorking,
                        StartWorking = (from wrS in _context.WorkerRequest.Where(wrW => wrW.RequestId == r.Id)
                                        join tS in _context.TimeSheet on wrS.Id equals tS.WorkerRequestId
                                        select tS.Date).DefaultIfEmpty().Min(),
                        FinishWorking = (from wrS in _context.WorkerRequest.Where(wrW => wrW.RequestId == r.Id)
                                         join tS in _context.TimeSheet on wrS.Id equals tS.WorkerRequestId
                                         select tS.Date).DefaultIfEmpty().Max()
                    };
        return query.ToPaginatedList(pagination);
    }

    public Task<AgencyWorkerRequestModel> GetRequestWorkerByCompanyId(Guid companyId, Guid requestId, Guid workerId)
    {
        var query = (from r in _context.Request.Where(c => c.Id == requestId && c.CompanyId == companyId)
                     join wp in _context.WorkerProfile on r.AgencyId equals wp.AgencyId
                     join cf in _context.CovenantFile on wp.ProfileImageId equals cf.Id into tmp
                     from cfl in tmp.DefaultIfEmpty()
                     join wr in _context.WorkerRequest.Where(c => c.RequestId == requestId && c.WorkerId == workerId) on wp.WorkerId equals wr.WorkerId
                     where wp.WorkerId == wr.WorkerId
                     select new AgencyWorkerRequestModel
                     {
                         Id = wr.Id,
                         WorkerId = wp.WorkerId,
                         WorkerProfileId = wp.Id,
                         Name = $"{wp.FirstName} {wp.MiddleName} {wp.LastName} {wp.SecondLastName}",
                         Status = wr.WorkerRequestStatus.ToString(),
                         ProfileImage = cfl == null ? null : $"{filesConfiguration.FilesPath}{cfl.FileName}",
                         NumberId = wp.NumberId,
                         DueDate = wp.DueDate,
                         SocialInsuranceExpire = wp.SocialInsuranceExpire,
                         ApprovedToWork = wp.ApprovedToWork
                     }).OrderByDescending(c => c.Name).AsNoTracking();
        return query.SingleOrDefaultAsync();
    }

    public Task<ShiftModel> GetRequestShift(Guid requestId) =>
        _context.Request.Where(c => c.Id == requestId)
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
        _context.RequestCancellationDetail.Where(s => s.RequestId == requestId)
            .Include(i => i.ReasonCancellationRequest)
            .SingleOrDefaultAsync();

    public Task<RequestFinalizationDetail> GetRequestFinalizationDetail(Guid requestId) => _context.RequestFinalizationDetail.SingleOrDefaultAsync(s => s.RequestId == requestId);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task PutRequestInProgress()
    {
        if (!_context.Database.IsNpgsql()) return;
        DateTime now = _timeService.GetCurrentDateTime();
        var requests = await _context.Request.Where(c => c.Status == RequestStatus.Requested && c.StartAt != null && now > c.StartAt).ToListAsync();
        foreach (var request in requests)
        {
            Result result = request.PutInProcess();
            if (!result) continue;
            _context.Request.Update(request);
            await _context.SaveChangesAsync();
        }
    }

    public Task<RequestContactPersonDetailModel> GetRequestedByDetail(Guid requestId, Guid contactPersonId) =>
        _context.RequestRequestedBy.Where(c => c.RequestId == requestId && c.ContactPersonId == contactPersonId)
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
        _context.RequestReportTo.Where(c => c.RequestId == requestId && c.ContactPersonId == contactPersonId)
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
        _context.RequestRequestedBy.Where(c => c.RequestId == requestId)
            .Select(s => new RequestContactPersonModel
            {
                Id = s.ContactPerson.Id,
                Title = s.ContactPerson.Title,
                FirstName = s.ContactPerson.FirstName,
                MiddleName = s.ContactPerson.MiddleName,
                LastName = s.ContactPerson.LastName,
            }).ToPaginatedList(pagination);

    public Task<RequestRequestedBy> GetRequestedBy(Guid requestId, Guid contactPersonId) => _context.RequestRequestedBy.SingleOrDefaultAsync(c => c.RequestId == requestId && c.ContactPersonId == contactPersonId);

    public Task<RequestReportTo> GetReportTo(Guid requestId, Guid contactPersonId) => _context.RequestReportTo.SingleOrDefaultAsync(c => c.RequestId == requestId && c.ContactPersonId == contactPersonId);

    public Task<PaginatedList<RequestContactPersonModel>> GetReportToList(Guid requestId, Pagination pagination) =>
        _context.RequestReportTo.Where(c => c.RequestId == requestId)
            .Select(s => new RequestContactPersonModel
            {
                Id = s.ContactPerson.Id,
                Title = s.ContactPerson.Title,
                FirstName = s.ContactPerson.FirstName,
                MiddleName = s.ContactPerson.MiddleName,
                LastName = s.ContactPerson.LastName,
            }).ToPaginatedList(pagination);

    public Task<PaginatedList<NoteModel>> GetNotes(Guid requestId, Pagination pagination) =>
        _context.RequestNotes.Where(w => w.RequestId == requestId && !w.Note.IsDeleted)
            .Select(RequestExtensionsMapping.SelectNote).OrderByDescending(c => c.CreatedAt).ToPaginatedList(pagination);

    public Task<NoteModel> GetNoteDetail(Guid requestId, Guid id) =>
        _context.RequestNotes.Where(w => w.RequestId == requestId && w.NoteId == id)
            .Select(RequestExtensionsMapping.SelectNote).SingleOrDefaultAsync();

    public Task<RequestNote> GetNote(Guid requestId, Guid id) =>
        _context.RequestNotes.Where(c => c.RequestId == requestId && c.NoteId == id)
            .Include(c => c.Note).SingleOrDefaultAsync();

    public Task<PaginatedList<RequestRecruiterDetailModel>> GetRecruiters(Guid requestId, Pagination pagination) =>
        _context.RequestRecruiter.Where(c => c.RequestId == requestId)
            .Select(s => new RequestRecruiterDetailModel { RecruiterId = s.RecruiterId, Email = s.Recruiter.User.Email })
            .ToPaginatedList(pagination);

    public Task<RequestSkill> GetSkill(Guid requestId, Guid id) => _context.RequestSkill.SingleOrDefaultAsync(c => c.RequestId == requestId && c.Id == id);

    public async Task<IEnumerable<SkillModel>> GetSkills(Guid requestId) =>
        await _context.RequestSkill.Where(c => c.RequestId == requestId)
            .Select(p => new SkillModel { Id = p.Id, Skill = p.Skill })
            .ToListAsync();

    public Task<RequestApplicant> GetRequestApplicant(Expression<Func<RequestApplicant, bool>> expression) => _context.RequestApplicant.SingleOrDefaultAsync(expression);

    public async Task<IEnumerable<RequestApplicant>> GetRequestApplicants(Expression<Func<RequestApplicant, bool>> expression)
    {
        var applicants = _context.RequestApplicant.Where(expression);
        return await applicants.ToListAsync();
    }

    public async Task<PaginatedList<RequestApplicantDetailModel>> GetRequestApplicants(Guid requestId, GetRequestApplicantFilter filter)
    {
        var query = from rc in _context.RequestApplicant.Where(c => c.RequestId == requestId)
                    join c in _context.Candidates on rc.CandidateId equals c.Id into tC
                    from c in tC.DefaultIfEmpty()
                    join wp in _context.WorkerProfile on rc.WorkerProfileId equals wp.Id into tWp
                    from wp in tWp.DefaultIfEmpty()
                    join u in _context.User on wp.WorkerId equals u.Id into tU
                    from u in tU.DefaultIfEmpty()
                    select new RequestApplicantDetailModel
                    {
                        Id = rc.Id,
                        Comments = rc.Comments,
                        CandidateId = rc.CandidateId,
                        WorkerProfileId = rc.WorkerProfileId,
                        WorkerId = c != null ? null : wp.WorkerId,
                        CreatedBy = rc.CreatedBy,
                        CreatedAt = rc.CreatedAt,
                        Name = c != null ? c.Name : wp.FirstName + " " + wp.MiddleName + " " + wp.LastName + " " + wp.SecondLastName,
                        PhoneNumber = c != null ? c.PhoneNumbers.FirstOrDefault().PhoneNumber : wp.MobileNumber != null ? wp.MobileNumber : wp.Phone,
                        Email = c != null ? c.Email : u.Email
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

    public async Task<RequestComission> GetRequestComission(Guid requestId) => await _context.RequestComissions.FirstOrDefaultAsync(rc => rc.RequestId == requestId);

    public async Task<IEnumerable<RequestCompanyUser>> GetRequestCompanyUsers(Guid requestId) => await _context.RequestCompanyUsers.Where(rcu => rcu.RequestId == requestId).ToListAsync();

    public async Task<IEnumerable<CompanyProfileListModel>> GetCompaniesWithRequests(IEnumerable<Guid> agencyIds)
    {
        var companyProfiles = _context.CompanyProfile.Where(cp => agencyIds.Contains(cp.AgencyId));
        var query = from r in _context.Request
                    join cp in companyProfiles on r.CompanyId equals cp.CompanyId
                    select new CompanyProfileListModel
                    {
                        Id = cp.Id,
                        CompanyId = cp.CompanyId,
                        FullName = cp.FullName,
                    };
        var result = await query.Distinct().OrderBy(q => q.FullName).ToListAsync();
        return result;
    }

    public async Task<IEnumerable<Common.Entities.Request.Request>> GetRequests(IEnumerable<Guid> ids)
    {
        var requests = _context.Request.Where(r => ids.Contains(r.Id));
        return await requests.ToListAsync();
    }

    public async Task<bool> ExistsRequestByNumber(int orderId)
    {
        var result = await _context.Request.AnyAsync(r => r.NumberId == orderId);
        return result;
    }
}