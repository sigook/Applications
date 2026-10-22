using Covenant.Common.Configuration;
using Covenant.Common.Entities.Candidates;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Companies.Agencies;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models.Locations;
using Covenant.Common.Models.Requests.Agencies;
using Covenant.Common.Models.Requests.Companies;
using Covenant.Common.Models.Requests.WeeklyBoard;
using Covenant.Common.Models.Requests.Workers;
using Covenant.Common.Models.Requests;
using Covenant.Common.Models.Website;
using Covenant.Common.Models.Workers;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Extensions;
using Covenant.Infrastructure.Mappers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Covenant.Infrastructure.Repositories.Requests;

public class RequestRepository(CovenantContext context, IOptions<FilesConfiguration> options) : BaseRepository<CovenantContext>(context), IRequestRepository
{
    private readonly FilesConfiguration filesConfiguration = options.Value;

    public IQueryable<AgencyRequestListModel> GetAllRequestsForAgency(Guid agencyId, GetRequestForAgencyFilter filter)
    {
        var requests = Context.Requests.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.Recruiter))
        {
            requests = from r in requests
                       join rr in Context.RequestRecruiters.Where(c => c.Recruiter.User.Email.ToLower() == filter.Recruiter.ToLower()) on r.Id equals rr.RequestId
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
        if (SearchExtensions.HasSearchTokens(filter.DisplayRecruiters))
        {
            var recruiterMatches = SearchExtensions.SearchPredicate<RequestRecruiter>(filter.DisplayRecruiters, rr => rr.Recruiter.Name);
            requests = requests.Where(r => Context.RequestRecruiters.Where(rr => rr.RequestId == r.Id).Any(recruiterMatches));
        }
        if (SearchExtensions.HasSearchTokens(filter.CompanyFullName))
        {
            var companyMatches = SearchExtensions.SearchPredicate<Request>(filter.CompanyFullName, r => r.CompanyProfile.FullName);
            var contactMatches = SearchExtensions.SearchPredicate<RequestReportTo>(filter.CompanyFullName,
                rt => rt.ContactPerson.FirstName, rt => rt.ContactPerson.LastName);
            requests = requests.Where(companyMatches.Or(r => Context.RequestReportTos.Where(rt => rt.RequestId == r.Id).Any(contactMatches)));
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
                        DisplayRecruiters = string.Join("|", Context.RequestRecruiters
                            .Where(rr => rr.RequestId == r.Id)
                            .Select(rr => rr.Recruiter.Name).Distinct()),
                        DisplayReportTo = string.Join(", ", Context.RequestReportTos
                            .Where(rt => rt.RequestId == r.Id)
                            .Select(rt => rt.ContactPerson.FirstName + " " + rt.ContactPerson.LastName)),
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
        return await Context.Set<RequestSource>()
            .Include(rs => rs.Source)
            .Where(rs => rs.RequestId == requestId)
            .ToListAsync();
    }

    public async Task ReplaceRequestSources(Guid requestId, IEnumerable<CreateRequestSourceModel> sources)
    {
        var set = Context.Set<RequestSource>();
        var existing = await set.Where(rs => rs.RequestId == requestId).ToListAsync();
        set.RemoveRange(existing);

        if (sources == null) return;
        foreach (var model in sources)
        {
            await set.AddAsync(new RequestSource(requestId, model.SourceId, model.PublishedAt, model.ExternalUrl));
        }
    }

    private static Expression<Func<AgencyRequestListModel, bool>> ApplyFilterForAgency(GetRequestForAgencyFilter filter)
    {
        Expression<Func<AgencyRequestListModel, bool>> predicate = PredicateBuilder.New<AgencyRequestListModel>(true);
        if (!filter.HasPermissionToSeeInternalRequests)
            predicate = predicate.And(p => p.HasPermissionToSeeInternalRequests == false);
        if (filter.NumberId.HasValue)
            predicate = predicate.And(r => r.NumberId == filter.NumberId.Value);
        predicate = predicate
            .AndSearch(filter.Location, r => r.Address, r => r.City, r => r.ProvinceName, r => r.PostalCode)
            .AndSearch(filter.JobTitle, r => r.JobTitle, r => r.BillingTitle)
            .AndSearch(filter.SalesRepresentative, r => r.SalesRepresentative);
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
        return predicate.AndSearch(filter.Filter, r => r.NumberId.ToString(), r => r.JobTitle, r => r.BillingTitle, r => r.CompanyFullName);
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
            default:
                query = query.OrderByDescending(o => o.NumberId);
                break;
        }
        return query;
    }

    public async Task<IEnumerable<JobViewModel>> GetAvailableRequest(IEnumerable<string> countries)
    {
        var openStatus = new RequestStatus[] { RequestStatus.Open };
        var requests = Context.Requests.Include(r => r.Shift)
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
                Type = r.EmploymentType.ToDisplayName(),
                CreatedAt = r.CreatedAt,
                Shift = r.Shift == null ? string.Empty : r.Shift.DisplayShift
            });
        return await requests.OrderBy(r => r.Title).ToListAsync();
    }

    public async Task<AgencyRequestDetailModel> GetRequestDetailForAgency(Guid id)
    {
        var requests = Context.Requests
            .Include(r => r.JobLocation)
            .ThenInclude(jl => jl.City)
            .ThenInclude(c => c.Province)
            .ThenInclude(p => p.Country)
            .Where(c => c.Id == id);
        var query = from r in requests
                    join rcd in Context.RequestCancellationDetails on r.Id equals rcd.RequestId
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
                        DisplayRecruiters = string.Join("|", Context.RequestRecruiters
                            .Where(rr => rr.RequestId == r.Id)
                            .Select(rr => rr.Recruiter.Name).Distinct()),
                        DisplayShift = r.Shift == null ? null : r.Shift.DisplayShift,
                        IsAsap = r.IsAsap,
                        VaccinationRequired = r.CompanyProfile.VaccinationRequired,
                        PunchCardOptionEnabled = r.PunchCardOptionEnabled,
                        InternalRequirements = r.InternalRequirements,
                        SalesRepresentativeId = r.RequestComission != null ? r.RequestComission.AgencyPersonnelId : null,
                        CompanyUserIds = r.RequestCompanyUser.Select(rcu => rcu.CompanyUserId),
                        ComplianceItems = r.ComplianceItems.Select(ci => new RequestComplianceItemModel
                        {
                            Id = ci.Id,
                            Name = ci.Name,
                            IsMandatory = ci.IsMandatory,
                            DocumentTarget = ci.DocumentTarget
                        }),
                        Skills = r.Skills.Select(s => new SkillModel { Id = s.Id, Skill = s.Skill }),
                        RequestedBy = r.RequestedBy.Select(rb => new RequestContactPersonModel
                        {
                            Id = rb.ContactPerson.Id,
                            Title = rb.ContactPerson.Title,
                            FirstName = rb.ContactPerson.FirstName,
                            MiddleName = rb.ContactPerson.MiddleName,
                            LastName = rb.ContactPerson.LastName
                        }),
                        ReportTo = r.ReportTo.Select(rt => new RequestContactPersonModel
                        {
                            Id = rt.ContactPerson.Id,
                            Title = rt.ContactPerson.Title,
                            FirstName = rt.ContactPerson.FirstName,
                            MiddleName = rt.ContactPerson.MiddleName,
                            LastName = rt.ContactPerson.LastName
                        }),
                        ApplicantsCount = Context.RequestApplicants.Count(ra => ra.RequestId == r.Id),
                        RunnersCount = Context.Runners.Count(ru => ru.RequestId == r.Id),
                        WorkersCount = Context.WorkerRequests.Count(wr => wr.RequestId == r.Id),
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
        var requests = Context.Requests.Where(r => r.CompanyProfile.CompanyId == companyId);
        if (filter.CompanyUserId.HasValue)
            requests = requests.Where(r => r.RequestCompanyUser.Any(rcu => rcu.CompanyUserId == filter.CompanyUserId));
        requests = requests.WhereSearch(filter.JobTitle, r => r.JobTitle, r => r.BillingTitle);
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
        return predicate.AndSearch(filter.Location, r => r.Location, r => r.Entrance);
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
            default:
                query = query.OrderByDescending(r => r.NumberId);
                break;
        }
        return query;
    }

    public Task<CompanyRequestDetailModel> GetRequestDetailForCompany(Guid id) =>
        (from r in Context.Requests.Where(c => c.Id == id)
         select new CompanyRequestDetailModel
         {
             Id = r.Id,
             NumberId = r.NumberId,
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

    public Task<Request> GetRequest(Expression<Func<Request, bool>> condition) =>
        Context.Requests.Where(condition)
            .Include(r => r.JobPositionRate)
            .Include(r => r.JobLocation).ThenInclude(l => l.City).ThenInclude(c => c.Province).ThenInclude(p => p.Country)
            .Include(r => r.Workers)
            .Include(r => r.Shift)
            .Include(i => i.Recruiters).ThenInclude(ti => ti.Recruiter).ThenInclude(u => u.User)
            .Include(i => i.CompanyProfile).ThenInclude(cp => cp.Agency)
            .SingleOrDefaultAsync();

    public async Task<PaginatedList<AgencyWorkerRequestModel>> GetWorkersRequestByRequestId(Guid requestId, GetWorkersRequestFilter filter)
    {
        var workerRequests = Context.WorkerRequests.WhereSearch(filter.Name,
            wr => wr.WorkerProfile.FirstName, wr => wr.WorkerProfile.MiddleName, wr => wr.WorkerProfile.LastName,
            wr => wr.WorkerProfile.SecondLastName, wr => wr.WorkerProfile.Worker.Email);
        var query = from wr in workerRequests
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
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(c => c.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && c.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
        if (filter.RejectedAtFrom.HasValue && filter.RejectedAtTo.HasValue)
            predicate = predicate.And(c => c.RejectedAt.Value.Date >= filter.RejectedAtFrom.Value.Date && c.RejectedAt.Value.Date <= filter.RejectedAtTo.Value.Date);
        return predicate
            .AndSearch(filter.CreatedBy, c => c.CreatedBy)
            .AndSearch(filter.RejectedBy, c => c.RejectedBy)
            .AndSearch(filter.ExternalId, wr => wr.ExternalId);
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
            default:
                query = query.OrderByDescending(wr => wr.CreatedAt).ThenBy(wr => wr.Name);
                break;
        }
        return query;
    }

    public Task<AgencyWorkerRequestModel> GetWorkerRequestByAgencyId(Guid agencyId, Guid requestId, Guid workerRequestId)
    {
        IQueryable<AgencyWorkerRequestModel> query = from wr in Context.WorkerRequests.Where(c => c.Id == workerRequestId && c.RequestId == requestId)
                                                     where wr.WorkerProfile.AgencyId == agencyId
                                                     select new AgencyWorkerRequestModel
                                                     {
                                                         Id = wr.Id,
                                                         WorkerId = wr.WorkerProfile.WorkerId,
                                                         WorkerProfileId = wr.WorkerProfileId,
                                                         Name = wr.WorkerProfile.FirstName + (string.IsNullOrWhiteSpace(wr.WorkerProfile.MiddleName) ? string.Empty : " " + wr.WorkerProfile.MiddleName) + " " + wr.WorkerProfile.LastName + (string.IsNullOrWhiteSpace(wr.WorkerProfile.SecondLastName) ? string.Empty : " " + wr.WorkerProfile.SecondLastName),
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
        (from wp in Context.WorkerProfiles.Where(c => c.WorkerId == workerId)
         join r in Context.Requests.Where(r => r.Status == RequestStatus.Cancelled) on wp.AgencyId equals r.CompanyProfile.AgencyId
         join wr in Context.WorkerRequests.Where(c => c.WorkerProfile.WorkerId == workerId) on r.Id equals wr.RequestId
         orderby r.NumberId descending
         select new WorkerRequestListModel
         {
             Id = r.Id,
             NumberId = r.NumberId,
             IsAsap = r.IsAsap,
             CreatedAt = r.CreatedAt,
             JobTitle = r.JobTitle,
             WorkerRate = r.WorkerRate,
             WorkerSalary = r.WorkerSalary,
             WorkerApprovedToWork = wp.ApprovedToWork.ToString(),
             Location = r.JobLocation.Address + " " + r.JobLocation.City.Value + " " + r.JobLocation.City.Province.Value + " " + r.JobLocation.PostalCode,
             Entrance = r.JobLocation.Entrance,
             Status = wr.WorkerRequestStatus.ToString(),
             WorkersQuantity = r.WorkersQuantity,
             StartAt = r.StartAt,
             FinishAt = r.FinishAt,
             DurationTerm = r.DurationTerm.ToString()
         }).AsNoTracking().ToPaginatedList(pagination);

    public async Task<PaginatedList<WorkerRequestListModel>> GetRequestsForWorker(Guid workerId, GetWorkerRequestsFilter filter)
    {
        var agencyId = await Context.WorkerProfiles
            .Where(wp => wp.WorkerId == workerId)
            .Select(wp => wp.AgencyId)
            .FirstOrDefaultAsync();
        var bookedRequests = Context.WorkerRequests
            .Where(wr => wr.WorkerProfile.WorkerId == workerId && wr.WorkerRequestStatus == WorkerRequestStatus.Booked);
        var rejectedRequests = Context.WorkerRequests
            .Where(wr => wr.WorkerProfile.WorkerId == workerId && wr.WorkerRequestStatus == WorkerRequestStatus.Rejected);
        var hasBooked = await bookedRequests.AnyAsync();

        var requests = Context.Requests.Where(r => r.CompanyProfile.AgencyId == agencyId);
        if (filter.NumberId.HasValue)
            requests = requests.Where(r => r.NumberId == filter.NumberId.Value);
        requests = requests.WhereSearch(filter.JobTitle, r => r.JobTitle);

        var projected = requests
            .Select(r => new
            {
                Request = r,
                IsBooked = bookedRequests.Any(wr => wr.RequestId == r.Id),
                IsRejected = rejectedRequests.Any(wr => wr.RequestId == r.Id)
            })
            .Where(x => x.IsBooked || (!hasBooked && x.Request.Status == RequestStatus.Open && !x.IsRejected))
            .Select(x => new WorkerRequestListModel
        {
            Id = x.Request.Id,
            NumberId = x.Request.NumberId,
            IsAsap = x.Request.IsAsap,
            CreatedAt = x.Request.CreatedAt,
            JobTitle = x.Request.JobTitle,
            WorkerRate = x.IsBooked ? (x.Request.WorkerRate ?? x.Request.WorkerSalary) : (x.Request.WorkerSalary ?? x.Request.WorkerRate),
            Location = x.IsBooked
                ? x.Request.JobLocation.Address + " " + x.Request.JobLocation.City.Value + " " + x.Request.JobLocation.City.Province.Value + " " + x.Request.JobLocation.PostalCode
                : x.Request.JobLocation.City.Value + " " + x.Request.JobLocation.City.Province.Value,
            Entrance = x.Request.JobLocation.Entrance,
            Status = x.IsBooked ? WorkerRequestStatus.Booked.ToString() : null,
            WorkersQuantity = x.Request.WorkersQuantity,
            StartAt = x.Request.StartAt,
            FinishAt = x.Request.FinishAt,
            DurationTerm = x.Request.DurationTerm.ToString()
        });

        var ordered = filter.SortBy switch
        {
            GetWorkerRequestsSortBy.Rate => projected.AddOrderBy(filter, r => r.WorkerRate).ThenByDescending(r => r.NumberId),
            _ => projected.AddOrderBy(filter, r => r.NumberId)
        };

        return await ordered.ThenBy(r => r.Id).AsNoTracking().ToPaginatedList(filter);
    }

    public Task<WorkerRequestDetailModel> GetRequestDetailForWorker(Guid workerId, Guid requestId)
    {
        return (from r in Context.Requests.Where(c => c.Id == requestId)
                join wp in Context.WorkerProfiles.Where(c => c.WorkerId == workerId) on r.CompanyProfile.AgencyId equals wp.AgencyId
                join wr in Context.WorkerRequests.Where(c => c.WorkerProfile.WorkerId == workerId && c.RequestId == requestId) on r.Id equals wr.RequestId into tmp
                from wr in tmp.DefaultIfEmpty()
                join ra in Context.RequestApplicants on new { rId = r.Id, wpId = wp.Id } equals new { rId = ra.RequestId, wpId = ra.WorkerProfileId.Value }
                    into tmpRa
                from ra in tmpRa.DefaultIfEmpty()
                select new WorkerRequestDetailModel
                {
                    Id = r.Id,
                    NumberId = r.NumberId,
                    JobTitle = r.JobTitle,
                    Status = wr == null ? "None" : wr.WorkerRequestStatus.ToString(),
                    DurationTerm = r.DurationTerm.ToString(),
                    RequestStatus = r.Status.ToString(),
                    Description = r.Description,
                    Requirements = r.Requirements,
                    Responsibilities = r.Responsibilities,
                    DisplayShift = r.Shift == null ? null : r.Shift.DisplayShift,
                    Skills = Context.RequestSkills.Where(rs => rs.RequestId == r.Id).Select(rs => rs.Skill),
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

    public Task<PaginatedList<WorkerRequestHistoryModel>> GetWorkerRequestHistory(Guid agencyId, Guid workerProfileId, GetWorkerRequestHistoryFilter filter)
    {
        var query = Context.WorkerRequests
            .Where(wr => wr.WorkerProfileId == workerProfileId
                         && wr.WorkerProfile.AgencyId == agencyId
                         && wr.Request.CompanyProfile.AgencyId == agencyId)
            .Select(wr => new WorkerRequestHistoryModel
            {
                Id = wr.RequestId,
                NumberId = wr.Request.NumberId,
                JobTitle = wr.Request.JobTitle,
                CompanyFullName = wr.Request.CompanyProfile.FullName,
                CompanyProfileId = wr.Request.CompanyProfileId,
                Address = wr.Request.JobLocation.Address,
                City = wr.Request.JobLocation.City.Value,
                ProvinceName = wr.Request.JobLocation.City.Province.Value,
                PostalCode = wr.Request.JobLocation.PostalCode,
                Entrance = wr.Request.JobLocation.Entrance,
                CreatedAt = wr.Request.CreatedAt,
                RequestStatus = wr.Request.Status,
                WorkerRequestStatus = wr.WorkerRequestStatus,
                IsAsap = wr.Request.IsAsap,
                StartWorking = wr.TimeSheets.Min(t => (DateTime?)t.Date) ?? wr.StartWorking,
                FinishWorking = wr.TimeSheets.Max(t => (DateTime?)t.Date)
            });
        query = query.Where(ApplyFilterWorkerRequestHistory(filter));
        query = ApplySortWorkerRequestHistory(query, filter);
        return query.ToPaginatedList(filter);
    }

    private static Expression<Func<WorkerRequestHistoryModel, bool>> ApplyFilterWorkerRequestHistory(GetWorkerRequestHistoryFilter filter)
    {
        Expression<Func<WorkerRequestHistoryModel, bool>> predicate = PredicateBuilder.New<WorkerRequestHistoryModel>(true);
        if (filter.NumberId.HasValue)
            predicate = predicate.And(r => r.NumberId == filter.NumberId.Value);
        predicate = predicate
            .AndSearch(filter.CompanyFullName, r => r.CompanyFullName)
            .AndSearch(filter.JobTitle, r => r.JobTitle)
            .AndSearch(filter.Location, r => r.Address, r => r.City, r => r.ProvinceName, r => r.PostalCode);
        if (filter.Statuses != null && filter.Statuses.Any())
            predicate = predicate.And(r => filter.Statuses.Contains(r.RequestStatus));
        if (filter.WorkerRequestStatuses != null && filter.WorkerRequestStatuses.Any())
            predicate = predicate.And(r => filter.WorkerRequestStatuses.Contains(r.WorkerRequestStatus));
        if (filter.StartWorkingFrom.HasValue && filter.StartWorkingTo.HasValue)
            predicate = predicate.And(r => r.StartWorking >= filter.StartWorkingFrom.Value.Date && r.StartWorking <= filter.StartWorkingTo.Value.Date);
        return predicate;
    }

    private static IQueryable<WorkerRequestHistoryModel> ApplySortWorkerRequestHistory(IQueryable<WorkerRequestHistoryModel> query, GetWorkerRequestHistoryFilter filter)
    {
        switch (filter.SortBy)
        {
            case WorkerRequestHistorySortBy.Client:
                return query.AddOrderBy(filter, r => r.CompanyFullName).ThenByDescending(r => r.NumberId).ThenBy(r => r.Id);
            case WorkerRequestHistorySortBy.JobTitle:
                return query.AddOrderBy(filter, r => r.JobTitle).ThenByDescending(r => r.NumberId).ThenBy(r => r.Id);
            case WorkerRequestHistorySortBy.StartWorking:
                return query.AddOrderBy(filter, r => r.StartWorking).ThenBy(r => r.Id);
            case WorkerRequestHistorySortBy.FinishWorking:
                return query.AddOrderBy(filter, r => r.FinishWorking).ThenBy(r => r.Id);
            case WorkerRequestHistorySortBy.CreatedAt:
                return query.AddOrderBy(filter, r => r.CreatedAt).ThenBy(r => r.Id);
            case WorkerRequestHistorySortBy.NumberId:
                return query.AddOrderBy(filter, r => r.NumberId).ThenBy(r => r.Id);
            default:
                return query.OrderByDescending(r => r.NumberId).ThenBy(r => r.Id);
        }
    }

    public Task<AgencyWorkerRequestModel> GetRequestWorkerByCompanyId(Guid companyId, Guid requestId, Guid workerProfileId)
    {
        var query = (from wr in Context.WorkerRequests.Where(c => c.RequestId == requestId && c.WorkerProfileId == workerProfileId)
                     where wr.Request.CompanyProfile.CompanyId == companyId
                     select new AgencyWorkerRequestModel
                     {
                         Id = wr.Id,
                         WorkerId = wr.WorkerProfile.WorkerId,
                         WorkerProfileId = wr.WorkerProfileId,
                         Name = wr.WorkerProfile.FirstName + (string.IsNullOrWhiteSpace(wr.WorkerProfile.MiddleName) ? string.Empty : " " + wr.WorkerProfile.MiddleName) + " " + wr.WorkerProfile.LastName + (string.IsNullOrWhiteSpace(wr.WorkerProfile.SecondLastName) ? string.Empty : " " + wr.WorkerProfile.SecondLastName),
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
        Context.Requests.Where(c => c.Id == requestId)
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
        Context.RequestCancellationDetails.Where(s => s.RequestId == requestId)
            .Include(i => i.ReasonCancellationRequest)
            .SingleOrDefaultAsync();

    public Task<RequestFinalizationDetail> GetRequestFinalizationDetail(Guid requestId) => Context.RequestFinalizationDetails.SingleOrDefaultAsync(s => s.RequestId == requestId);

    public Task<RequestContactPersonDetailModel> GetRequestedByDetail(Guid requestId, Guid contactPersonId) =>
        Context.RequestRequestedBys.Where(c => c.RequestId == requestId && c.ContactPersonId == contactPersonId)
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
        Context.RequestReportTos.Where(c => c.RequestId == requestId && c.ContactPersonId == contactPersonId)
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
        Context.RequestRequestedBys.Where(c => c.RequestId == requestId)
            .Select(s => new RequestContactPersonModel
            {
                Id = s.ContactPerson.Id,
                Title = s.ContactPerson.Title,
                FirstName = s.ContactPerson.FirstName,
                MiddleName = s.ContactPerson.MiddleName,
                LastName = s.ContactPerson.LastName,
            }).OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToPaginatedList(pagination);

    public Task<RequestRequestedBy> GetRequestedBy(Guid requestId, Guid contactPersonId) => Context.RequestRequestedBys.SingleOrDefaultAsync(c => c.RequestId == requestId && c.ContactPersonId == contactPersonId);

    public Task<RequestReportTo> GetReportTo(Guid requestId, Guid contactPersonId) => Context.RequestReportTos.SingleOrDefaultAsync(c => c.RequestId == requestId && c.ContactPersonId == contactPersonId);

    public Task<PaginatedList<RequestContactPersonModel>> GetReportToList(Guid requestId, Pagination pagination) =>
        Context.RequestReportTos.Where(c => c.RequestId == requestId)
            .Select(s => new RequestContactPersonModel
            {
                Id = s.ContactPerson.Id,
                Title = s.ContactPerson.Title,
                FirstName = s.ContactPerson.FirstName,
                MiddleName = s.ContactPerson.MiddleName,
                LastName = s.ContactPerson.LastName,
            }).OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToPaginatedList(pagination);

    public Task<PaginatedList<NoteModel>> GetNotes(Guid requestId, Pagination pagination) =>
        Context.RequestNotes.Where(w => w.RequestId == requestId && !w.Note.IsDeleted)
            .Select(RequestExtensionsMapping.SelectNote).OrderByDescending(c => c.CreatedAt).ToPaginatedList(pagination);

    public Task<NoteModel> GetNoteDetail(Guid requestId, Guid id) =>
        Context.RequestNotes.Where(w => w.RequestId == requestId && w.NoteId == id)
            .Select(RequestExtensionsMapping.SelectNote).SingleOrDefaultAsync();

    public Task<RequestNote> GetNote(Guid requestId, Guid id) =>
        Context.RequestNotes.Where(c => c.RequestId == requestId && c.NoteId == id)
            .Include(c => c.Note).SingleOrDefaultAsync();

    public async Task<IEnumerable<WeeklyBoardAssignmentModel>> GetWeeklyBoardAssignments(Guid agencyId, DateTime weekStart, DateTime weekEnd) =>
        await (from rr in Context.RequestRecruiters
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
        await (from r in Context.Runners
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
        await Context.WorkerProfiles
            .Where(w => workerProfileIds.Contains(w.Id))
            .ToDictionaryAsync(
                w => w.Id,
                w => w.FirstName +
                    (string.IsNullOrWhiteSpace(w.MiddleName) ? string.Empty : " " + w.MiddleName) +
                    " " + w.LastName +
                    (string.IsNullOrWhiteSpace(w.SecondLastName) ? string.Empty : " " + w.SecondLastName));

    public async Task<IEnumerable<WeeklyBoardAssignmentModel>> GetWeeklyBoardAssignmentsForRecruiter(Guid agencyId, Guid recruiterId, DateTime weekStart, DateTime weekEnd) =>
        await (from rr in Context.RequestRecruiters
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
        Context.RequestRecruiters
            .Include(rr => rr.Runners)
            .SingleOrDefaultAsync(rr => rr.RequestId == requestId
                && rr.RecruiterId == recruiterId
                && rr.WorkDate == workDate.Date
                && rr.Request.CompanyProfile.AgencyId == agencyId);

    public async Task BulkReplaceRecruiters(IEnumerable<Guid> requestIds, IEnumerable<Guid> recruiterIds, DateTime createdAt)
    {
        var requestIdList = requestIds.ToList();
        await Context.RequestRecruiters.Where(r => requestIdList.Contains(r.RequestId)).ExecuteDeleteAsync();
        var recruiterIdList = recruiterIds?.ToList() ?? [];
        if (recruiterIdList.Count == 0) return;
        var entities = requestIdList
            .SelectMany(rid => recruiterIdList.Select(recId => new RequestRecruiter(rid, recId, createdAt)))
            .ToList();
        await Context.RequestRecruiters.AddRangeAsync(entities);
    }

    public Task<RequestSkill> GetSkill(Guid requestId, Guid id) => Context.RequestSkills.SingleOrDefaultAsync(c => c.RequestId == requestId && c.Id == id);

    public async Task<IEnumerable<SkillModel>> GetSkills(Guid requestId) =>
        await Context.RequestSkills.Where(c => c.RequestId == requestId)
            .Select(p => new SkillModel { Id = p.Id, Skill = p.Skill })
            .ToListAsync();

    public Task<RequestApplicant> GetRequestApplicant(Expression<Func<RequestApplicant, bool>> expression) => Context.RequestApplicants.SingleOrDefaultAsync(expression);

    public async Task<IEnumerable<RequestApplicant>> GetRequestApplicants(Expression<Func<RequestApplicant, bool>> expression)
    {
        var applicants = Context.RequestApplicants.Where(expression);
        return await applicants.ToListAsync();
    }

    public async Task<PaginatedList<RequestApplicantDetailModel>> GetRequestApplicants(Guid requestId, GetRequestApplicantFilter filter)
    {
        var applicants = Context.RequestApplicants
            .Where(c => c.RequestId == requestId)
            .WhereSearch(filter.Name, ApplicantSearchFields);
        var query = from rc in applicants
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
                            : rc.WorkerProfile.FirstName +
                                (string.IsNullOrWhiteSpace(rc.WorkerProfile.MiddleName) ? string.Empty : " " + rc.WorkerProfile.MiddleName) +
                                " " + rc.WorkerProfile.LastName +
                                (string.IsNullOrWhiteSpace(rc.WorkerProfile.SecondLastName) ? string.Empty : " " + rc.WorkerProfile.SecondLastName),
                        PhoneNumber = rc.Candidate != null
                            ? rc.Candidate.PhoneNumbers.FirstOrDefault().PhoneNumber
                            : rc.WorkerProfile.MobileNumber != null ? rc.WorkerProfile.MobileNumber : rc.WorkerProfile.Phone,
                        Email = rc.Candidate != null ? rc.Candidate.Email : rc.WorkerProfile.Worker.Email,
                        Status = rc.Status,
                        ComplianceTotal = rc.Request.ComplianceItems.Count(),
                        ComplianceCompleted = rc.ComplianceItems.Count(),
                        MandatoryPending = rc.Request.ComplianceItems
                            .Count(ci => ci.IsMandatory && rc.ComplianceItems.All(c => c.RequestComplianceItemId != ci.Id))
                    };
        var predicateNew = ApplyFilterRequestApplicants(filter);
        query = query.Where(predicateNew);
        query = ApplySortRequestApplicants(query, filter);
        var result = await query.ToPaginatedList(filter);
        return result;
    }

    public IQueryable<AgencyApplicantListModel> GetAllAgencyApplicants(Guid agencyId, GetAgencyApplicantsFilter filter)
    {
        var applicants = ApplyRequestFilterAgencyApplicants(agencyId, filter);
        var query = from ra in applicants
                    select new AgencyApplicantListModel
                    {
                        Id = ra.Id,
                        CandidateId = ra.CandidateId,
                        WorkerProfileId = ra.WorkerProfileId,
                        WorkerId = ra.Candidate != null ? null : ra.WorkerProfile.WorkerId,
                        Name = ra.Candidate != null
                            ? ra.Candidate.Name
                            : ra.WorkerProfile.FirstName +
                                (string.IsNullOrWhiteSpace(ra.WorkerProfile.MiddleName) ? string.Empty : " " + ra.WorkerProfile.MiddleName) +
                                " " + ra.WorkerProfile.LastName +
                                (string.IsNullOrWhiteSpace(ra.WorkerProfile.SecondLastName) ? string.Empty : " " + ra.WorkerProfile.SecondLastName),
                        Email = ra.Candidate != null ? ra.Candidate.Email : ra.WorkerProfile.Worker.Email,
                        PhoneNumber = ra.Candidate != null
                            ? ra.Candidate.PhoneNumbers.FirstOrDefault().PhoneNumber
                            : ra.WorkerProfile.MobileNumber != null ? ra.WorkerProfile.MobileNumber : ra.WorkerProfile.Phone,
                        Comments = ra.Comments,
                        Status = ra.Status,
                        CreatedBy = ra.CreatedBy,
                        CreatedAt = ra.CreatedAt,
                        RequestId = ra.RequestId,
                        RequestNumberId = ra.Request.NumberId,
                        CompanyFullName = ra.Request.CompanyProfile.FullName,
                        JobTitle = ra.Request.JobTitle,
                        StartAt = ra.Request.StartAt,
                        IsAsap = ra.Request.IsAsap,
                        IsDirectHiring = ra.Request.WorkerSalary.HasValue,
                        ComplianceTotal = ra.Request.ComplianceItems.Count(),
                        ComplianceCompleted = ra.ComplianceItems.Count(),
                        MandatoryPending = ra.Request.ComplianceItems
                            .Count(ci => ci.IsMandatory && ra.ComplianceItems.All(c => c.RequestComplianceItemId != ci.Id))
                    };
        return query.Where(ApplyFilterAgencyApplicants(filter));
    }

    public async Task<AgencyApplicantsPagedResponse> GetAgencyApplicants(Guid agencyId, GetAgencyApplicantsFilter filter)
    {
        var query = GetAllAgencyApplicants(agencyId, filter);

        var requestsQuery = query
            .Select(ra => new { ra.RequestId, ra.RequestNumberId })
            .Distinct();
        requestsQuery = filter.IsDescending
            ? requestsQuery.OrderByDescending(r => r.RequestNumberId)
            : requestsQuery.OrderBy(r => r.RequestNumberId);
        var pageOfRequests = await requestsQuery.ToPaginatedList(filter);
        var requestIds = pageOfRequests.Items.Select(r => r.RequestId).ToList();

        var pagedApplicants = await ApplySortAgencyApplicants(query.Where(ra => requestIds.Contains(ra.RequestId)), filter).ToListAsync();
        var requests = await GetAgencyApplicantRequests(requestIds);

        return new AgencyApplicantsPagedResponse
        {
            PageIndex = pageOfRequests.PageIndex,
            TotalPages = pageOfRequests.TotalPages,
            TotalItems = pageOfRequests.TotalItems,
            TotalApplicants = await query.CountAsync(),
            Items = pageOfRequests.Items
                .Select(page => requests.Single(r => r.RequestId == page.RequestId))
                .Select(request =>
                {
                    request.Applicants = pagedApplicants.Where(a => a.RequestId == request.RequestId).ToList();
                    return request;
                })
                .ToList()
        };
    }

    private async Task<List<AgencyRequestApplicantsModel>> GetAgencyApplicantRequests(List<Guid> requestIds) =>
        await Context.Requests
            .Where(r => requestIds.Contains(r.Id))
            .Select(r => new AgencyRequestApplicantsModel
            {
                RequestId = r.Id,
                NumberId = r.NumberId,
                CompanyFullName = r.CompanyProfile.FullName,
                JobTitle = r.JobTitle,
                City = r.JobLocation.City.Value,
                ProvinceName = r.JobLocation.City.Province.Value,
                DisplayShift = r.Shift == null ? null : r.Shift.DisplayShift,
                StartAt = r.StartAt,
                IsAsap = r.IsAsap,
                IsDirectHiring = r.WorkerSalary.HasValue,
                WorkersQuantity = r.WorkersQuantity,
                ConfirmedApplicants = Context.RequestApplicants.Count(ra => ra.RequestId == r.Id && ra.Status == RequestApplicantStatus.Confirmed),
                TotalApplicants = Context.RequestApplicants.Count(ra => ra.RequestId == r.Id)
            })
            .ToListAsync();

    private IQueryable<RequestApplicant> ApplyRequestFilterAgencyApplicants(Guid agencyId, GetAgencyApplicantsFilter filter)
    {
        var applicants = Context.RequestApplicants.Where(ra => ra.Request.CompanyProfile.AgencyId == agencyId);
        applicants = filter.RequestStatuses is { Count: > 0 }
            ? applicants.Where(ra => filter.RequestStatuses.Contains(ra.Request.Status))
            : applicants.Where(ra => ra.Request.Status == RequestStatus.Open);
        if (filter.CompanyProfileId.HasValue)
            applicants = applicants.Where(ra => ra.Request.CompanyProfileId == filter.CompanyProfileId.Value);
        if (filter.NumberId.HasValue)
            applicants = applicants.Where(ra => ra.Request.NumberId == filter.NumberId.Value);
        applicants = applicants
            .WhereSearch(filter.JobTitle, ra => ra.Request.JobTitle, ra => ra.Request.BillingTitle)
            .WhereSearch(filter.Name, ApplicantSearchFields);
        if (!string.IsNullOrWhiteSpace(filter.Recruiter))
        {
            var recruiter = filter.Recruiter.ToLower();
            applicants = applicants.Where(ra => ra.Request.Recruiters.Any(rr => rr.Recruiter.User.Email.ToLower() == recruiter));
        }
        if (filter.StartAtFrom.HasValue)
            applicants = applicants.Where(ra => ra.Request.StartAt >= filter.StartAtFrom.Value.Date);
        if (filter.StartAtTo.HasValue)
            applicants = applicants.Where(ra => ra.Request.StartAt <= filter.StartAtTo.Value.Date);
        return applicants;
    }

    private Expression<Func<AgencyApplicantListModel, bool>> ApplyFilterAgencyApplicants(GetAgencyApplicantsFilter filter)
    {
        Expression<Func<AgencyApplicantListModel, bool>> predicate = PredicateBuilder.New<AgencyApplicantListModel>(true);
        predicate = predicate.AndSearch(filter.CreatedBy, ra => ra.CreatedBy);
        if (filter.Statuses is { Count: > 0 })
            predicate = predicate.And(ra => filter.Statuses.Contains(ra.Status));
        return predicate;
    }

    // Requests are already paginated and sorted by number, so this only decides
    // the order of the applicants inside each one.
    private static IQueryable<AgencyApplicantListModel> ApplySortAgencyApplicants(IQueryable<AgencyApplicantListModel> query, GetAgencyApplicantsFilter filter) =>
        filter.SortBy switch
        {
            GetAgencyApplicantSortBy.CreatedAt => query.OrderBy(ra => ra.CreatedAt),
            _ => query.OrderBy(ra => ra.Name)
        };

    private Expression<Func<RequestApplicantDetailModel, bool>> ApplyFilterRequestApplicants(GetRequestApplicantFilter filter)
    {
        Expression<Func<RequestApplicantDetailModel, bool>> predicate = PredicateBuilder.New<RequestApplicantDetailModel>(true);
        if (!string.IsNullOrWhiteSpace(filter.Phone))
        {
            var expression = new Regex(@"\s+");
            var phoneWithoutBlankSpaces = expression.Replace(filter.Phone, string.Empty);
            predicate = predicate.And(ra =>
                ra.PhoneNumber.Replace("(", string.Empty).Replace(")", string.Empty).Contains(filter.Phone) ||
                ra.PhoneNumber.Contains(phoneWithoutBlankSpaces));
        }
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(ra => ra.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && ra.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
        if (filter.Statuses is { Count: > 0 })
            predicate = predicate.And(ra => filter.Statuses.Contains(ra.Status));
        return predicate.AndSearch(filter.CreatedBy, ra => ra.CreatedBy);
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
            default:
                query = query.OrderByDescending(ra => ra.CreatedAt);
                break;
        }
        return query;
    }

    private const double SimilarityThreshold = 0.4;

    private static readonly Expression<Func<RequestApplicant, string>>[] ApplicantSearchFields =
    [
        ra => ra.Candidate.Name,
        ra => ra.Candidate.Email,
        ra => ra.WorkerProfile.FirstName,
        ra => ra.WorkerProfile.MiddleName,
        ra => ra.WorkerProfile.LastName,
        ra => ra.WorkerProfile.SecondLastName,
        ra => ra.WorkerProfile.Worker.Email
    ];

    private static Expression<Func<WorkerProfile, bool>> BuildWorkerSearchPredicate(
        Guid agencyId, string searchTerm, Expression<Func<WorkerProfile, bool>> exclusion)
    {
        var search = SearchExtensions.SearchPredicate<WorkerProfile>(searchTerm,
            wp => wp.FirstName, wp => wp.MiddleName, wp => wp.LastName, wp => wp.SecondLastName, wp => wp.Worker.Email);
        if (long.TryParse(searchTerm.Trim(), out var numberId))
            search = search.Or(wp => wp.NumberId == numberId);

        Expression<Func<WorkerProfile, bool>> predicate = wp => wp.AgencyId == agencyId;
        return predicate.And(exclusion).And(search);
    }

    private static Expression<Func<Candidate, bool>> BuildCandidateSearchPredicate(
        Guid agencyId, string searchTerm, Expression<Func<Candidate, bool>> exclusion)
    {
        var search = SearchExtensions.SearchPredicate<Candidate>(searchTerm, c => c.Name, c => c.Email);
        if (long.TryParse(searchTerm.Trim(), out var numberId))
            search = search.Or(c => c.NumberId == numberId);

        Expression<Func<Candidate, bool>> predicate = c => c.AgencyId == agencyId;
        return predicate.And(exclusion).And(search);
    }

    private static Expression<Func<WorkerProfile, bool>> BuildWorkerSimilarityPredicate(
        Guid agencyId, string searchTerm, Expression<Func<WorkerProfile, bool>> exclusion)
    {
        Expression<Func<WorkerProfile, bool>> predicate = wp => wp.AgencyId == agencyId;
        return predicate.And(exclusion)
            .And(SearchExtensions.SimilarityPredicate<WorkerProfile>(searchTerm, SimilarityThreshold, wp => wp.FirstName + " " + wp.LastName));
    }

    private static Expression<Func<Candidate, bool>> BuildCandidateSimilarityPredicate(
        Guid agencyId, string searchTerm, Expression<Func<Candidate, bool>> exclusion)
    {
        Expression<Func<Candidate, bool>> predicate = c => c.AgencyId == agencyId;
        return predicate.And(exclusion)
            .And(SearchExtensions.SimilarityPredicate<Candidate>(searchTerm, SimilarityThreshold, c => c.Name));
    }

    private IQueryable<ApplicantSearchResultModel> BuildWorkerSearchQuery(Expression<Func<WorkerProfile, bool>> workerPredicate) =>
        from wp in Context.WorkerProfiles.Where(workerPredicate)
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
        Expression<Func<Candidate, bool>> candidatePredicate)
    {
        var workers = BuildWorkerSearchQuery(workerPredicate);

        var candidates = from c in Context.Candidates.Where(candidatePredicate)
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

    public async Task<List<ApplicantSearchResultModel>> SearchApplicants(Guid agencyId, Guid requestId, string searchTerm)
    {
        if (!SearchExtensions.HasSearchTokens(searchTerm)) return [];

        var existingApplicants = Context.RequestApplicants.Where(ra => ra.RequestId == requestId);
        var bookedWorkers = Context.WorkerRequests.Where(wr => wr.RequestId == requestId && wr.WorkerRequestStatus == WorkerRequestStatus.Booked);
        Expression<Func<WorkerProfile, bool>> workerExclusion = wp =>
            !existingApplicants.Any(ra => ra.WorkerProfileId == wp.Id) && !bookedWorkers.Any(wr => wr.WorkerProfileId == wp.Id);
        Expression<Func<Candidate, bool>> candidateExclusion = c => !existingApplicants.Any(ra => ra.CandidateId == c.Id);

        var results = await SearchWorkersAndCandidates(
            BuildWorkerSearchPredicate(agencyId, searchTerm, workerExclusion),
            BuildCandidateSearchPredicate(agencyId, searchTerm, candidateExclusion));
        if (results.Count > 0) return results;

        return await SearchWorkersAndCandidates(
            BuildWorkerSimilarityPredicate(agencyId, searchTerm, workerExclusion),
            BuildCandidateSimilarityPredicate(agencyId, searchTerm, candidateExclusion));
    }

    public async Task<List<ApplicantSearchResultModel>> SearchRunnerProspects(Guid agencyId, Guid requestId, string searchTerm)
    {
        if (!SearchExtensions.HasSearchTokens(searchTerm)) return [];

        var existingRunners = Context.Runners.Where(r => r.RequestId == requestId);
        Expression<Func<WorkerProfile, bool>> workerExclusion = wp => !existingRunners.Any(r => r.WorkerProfileId == wp.Id);

        var results = await SearchWorkers(BuildWorkerSearchPredicate(agencyId, searchTerm, workerExclusion));
        if (results.Count > 0) return results;

        return await SearchWorkers(BuildWorkerSimilarityPredicate(agencyId, searchTerm, workerExclusion));
    }

    public async Task<RequestComission> GetRequestComission(Guid requestId) => await Context.RequestComissions.FirstOrDefaultAsync(rc => rc.RequestId == requestId);

    public async Task<IEnumerable<RequestCompanyUser>> GetRequestCompanyUsers(Guid requestId) => await Context.RequestCompanyUsers.Where(rcu => rcu.RequestId == requestId).ToListAsync();

    public async Task<IEnumerable<RequestComplianceItem>> GetComplianceItems(Guid requestId) => await Context.RequestComplianceItems.Where(ci => ci.RequestId == requestId).ToListAsync();

    public async Task<IEnumerable<RequestComplianceItem>> GetComplianceItems(IEnumerable<Guid> requestIds) =>
        await Context.RequestComplianceItems.Where(ci => requestIds.Contains(ci.RequestId)).ToListAsync();

    public async Task<RequestApplicantComplianceItem> GetApplicantComplianceItem(Guid applicantId, Guid complianceItemId) =>
        await Context.RequestApplicantComplianceItems
            .FirstOrDefaultAsync(ci => ci.RequestApplicantId == applicantId && ci.RequestComplianceItemId == complianceItemId);

    public async Task<IEnumerable<RequestApplicantComplianceItem>> GetApplicantComplianceItems(Guid applicantId) =>
        await Context.RequestApplicantComplianceItems
            .Where(ci => ci.RequestApplicantId == applicantId)
            .ToListAsync();

    public async Task<IEnumerable<RequestApplicantComplianceItem>> GetApplicantComplianceCompletions(IEnumerable<Guid> complianceItemIds) =>
        await Context.RequestApplicantComplianceItems
            .Where(ci => complianceItemIds.Contains(ci.RequestComplianceItemId))
            .ToListAsync();

    public async Task<IEnumerable<CompanyProfileListModel>> GetCompaniesWithRequests(IEnumerable<Guid> agencyIds)
    {
        var query = from r in Context.Requests
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

    public async Task<IEnumerable<Request>> GetRequests(IEnumerable<Guid> ids)
    {
        var requests = Context.Requests.Where(r => ids.Contains(r.Id));
        return await requests.ToListAsync();
    }

    public async Task<bool> ExistsRequestByNumber(int requestId)
    {
        var result = await Context.Requests.AnyAsync(r => r.NumberId == requestId);
        return result;
    }

    public async Task<bool> RequestBelongsToCompany(Guid requestId, Guid companyId)
    {
        return await Context.Requests.AnyAsync(r => r.Id == requestId && r.CompanyProfile.CompanyId == companyId);
    }
}