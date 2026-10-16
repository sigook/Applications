using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Request.Runners;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Requests;

public class RunnerRepository(CovenantContext context) : BaseRepository<CovenantContext>(context), IRunnerRepository
{
    public Task<Runner> GetRunner(Expression<Func<Runner, bool>> expression) =>
        Context.Runners
            .Include(r => r.StatusHistory)
            .Include(r => r.Interviews)
            .FirstOrDefaultAsync(expression);

    public Task<List<Runner>> GetRunners(Expression<Func<Runner, bool>> expression) =>
        Context.Runners
            .Include(r => r.Request)
            .Include(r => r.StatusHistory)
            .Where(expression)
            .ToListAsync();

    public Task<bool> RunnerExists(Guid requestId, Guid workerProfileId) =>
        Context.Runners.AnyAsync(r => r.RequestId == requestId && r.WorkerProfileId == workerProfileId);

    public async Task<PaginatedList<RunnerListModel>> GetRunners(Guid agencyId, GetRunnersFilter filter)
    {
        var query = Context.Runners
            .AsNoTracking()
            .Select(r => new RunnerListModel
            {
                Id = r.Id,
                NumberId = r.NumberId,
                AgencyId = r.Request.CompanyProfile.AgencyId,
                RequestId = r.RequestId,
                WorkerProfileId = r.WorkerProfileId,
                Name = r.WorkerProfile.FirstName + " " + r.WorkerProfile.LastName,
                Email = r.WorkerProfile.Worker.Email,
                Type = r.Type,
                Status = r.Status,
                StartDate = r.StartDate,
                InterviewsCount = r.Interviews.Count(),
                CreatedAt = r.CreatedAt,
                CreatedBy = r.CreatedBy
            });
        query = query.Where(ApplyFilter(agencyId, filter));
        query = ApplySort(query, filter);
        return await query.ToPaginatedList(filter);
    }

    public IQueryable<AgencyRunnerListModel> GetAllAgencyRunners(Guid agencyId, GetAgencyRunnersFilter filter)
    {
        var runners = ApplyRequestFilterAgencyRunners(agencyId, filter);
        var query = runners.Select(r => new AgencyRunnerListModel
        {
            Id = r.Id,
            NumberId = r.NumberId,
            RequestId = r.RequestId,
            WorkerProfileId = r.WorkerProfileId,
            Name = r.WorkerProfile.FirstName + " " + r.WorkerProfile.LastName,
            Email = r.WorkerProfile.Worker.Email,
            Type = r.Type,
            Status = r.Status,
            StartDate = r.StartDate,
            InterviewsCount = r.Interviews.Count(),
            LastInterviewAt = r.Interviews.Max(i => (DateTime?)i.ScheduledDate),
            CreatedAt = r.CreatedAt,
            CreatedBy = r.CreatedBy,
            CreatedByEmail = r.CreatedByUser.Email,
            RequestNumberId = r.Request.NumberId,
            CompanyFullName = r.Request.CompanyProfile.FullName,
            JobTitle = r.Request.JobTitle,
            StartAt = r.Request.StartAt,
            IsAsap = r.Request.IsAsap,
            IsDirectHiring = r.Request.WorkerSalary.HasValue,
            UsesRunners = r.Request.UsesRunners
        });
        return query.Where(ApplyFilterAgencyRunners(filter));
    }

    public async Task<AgencyRunnersPagedResponse> GetAgencyRunners(Guid agencyId, GetAgencyRunnersFilter filter)
    {
        var query = GetAllAgencyRunners(agencyId, filter);

        var requestsQuery = query
            .Select(r => new { r.RequestId, r.RequestNumberId })
            .Distinct();
        requestsQuery = filter.IsDescending
            ? requestsQuery.OrderByDescending(r => r.RequestNumberId)
            : requestsQuery.OrderBy(r => r.RequestNumberId);
        var pageOfRequests = await requestsQuery.ToPaginatedList(filter);
        var requestIds = pageOfRequests.Items.Select(r => r.RequestId).ToList();

        var pagedRunners = await ApplySortAgencyRunners(query.Where(r => requestIds.Contains(r.RequestId)), filter).ToListAsync();
        var requests = await GetAgencyRunnerRequests(requestIds);

        return new AgencyRunnersPagedResponse
        {
            PageIndex = pageOfRequests.PageIndex,
            TotalPages = pageOfRequests.TotalPages,
            TotalItems = pageOfRequests.TotalItems,
            TotalRunners = await query.CountAsync(),
            Items = pageOfRequests.Items
                .Select(page => requests.Single(r => r.RequestId == page.RequestId))
                .Select(request =>
                {
                    request.Runners = pagedRunners.Where(r => r.RequestId == request.RequestId).ToList();
                    return request;
                })
                .ToList()
        };
    }

    private async Task<List<AgencyRequestRunnersModel>> GetAgencyRunnerRequests(List<Guid> requestIds) =>
        await Context.Requests
            .Where(r => requestIds.Contains(r.Id))
            .Select(r => new AgencyRequestRunnersModel
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
                UsesRunners = r.UsesRunners,
                WorkersQuantity = r.WorkersQuantity,
                HiredRunners = Context.Runners.Count(x => x.RequestId == r.Id && x.Status == RunnerStatus.Hired),
                TotalRunners = Context.Runners.Count(x => x.RequestId == r.Id)
            })
            .ToListAsync();

    private IQueryable<Runner> ApplyRequestFilterAgencyRunners(Guid agencyId, GetAgencyRunnersFilter filter)
    {
        var runners = Context.Runners.AsNoTracking().Where(r => r.Request.CompanyProfile.AgencyId == agencyId);
        runners = filter.RequestStatuses is { Count: > 0 }
            ? runners.Where(r => filter.RequestStatuses.Contains(r.Request.Status))
            : runners.Where(r => r.Request.Status == RequestStatus.Open);
        if (filter.CompanyProfileId.HasValue)
            runners = runners.Where(r => r.Request.CompanyProfileId == filter.CompanyProfileId.Value);
        if (filter.NumberId.HasValue)
            runners = runners.Where(r => r.Request.NumberId == filter.NumberId.Value);
        if (!string.IsNullOrWhiteSpace(filter.JobTitle))
        {
            var jobTitle = filter.JobTitle.ToLower();
            runners = runners.Where(r => r.Request.JobTitle.ToLower().Contains(jobTitle)
                || (r.Request.BillingTitle != null && r.Request.BillingTitle.ToLower().Contains(jobTitle)));
        }
        if (!string.IsNullOrWhiteSpace(filter.Recruiter))
        {
            var recruiter = filter.Recruiter.ToLower();
            runners = runners.Where(r => r.Request.Recruiters.Any(rr => rr.Recruiter.User.Email.ToLower() == recruiter));
        }
        if (filter.StartAtFrom.HasValue)
            runners = runners.Where(r => r.Request.StartAt >= filter.StartAtFrom.Value.Date);
        if (filter.StartAtTo.HasValue)
            runners = runners.Where(r => r.Request.StartAt <= filter.StartAtTo.Value.Date);
        return runners;
    }

    private static Expression<Func<AgencyRunnerListModel, bool>> ApplyFilterAgencyRunners(GetAgencyRunnersFilter filter)
    {
        var predicate = PredicateBuilder.New<AgencyRunnerListModel>(true);
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var name = filter.Name.ToLower();
            predicate = predicate.And(r => r.Name.ToLower().Contains(name) || r.Email.ToLower().Contains(name));
        }
        if (filter.Type.HasValue)
            predicate = predicate.And(r => r.Type == filter.Type.Value);
        if (filter.Statuses is { Count: > 0 })
            predicate = predicate.And(r => filter.Statuses.Contains(r.Status));
        return predicate;
    }

    private static IQueryable<AgencyRunnerListModel> ApplySortAgencyRunners(IQueryable<AgencyRunnerListModel> query, GetAgencyRunnersFilter filter) =>
        filter.SortBy switch
        {
            GetAgencyRunnerSortBy.CreatedAt => query.OrderBy(r => r.CreatedAt).ThenBy(r => r.Id),
            GetAgencyRunnerSortBy.Status => query.OrderBy(r => r.Status).ThenBy(r => r.Name).ThenBy(r => r.Id),
            _ => query.OrderBy(r => r.Name).ThenBy(r => r.Id)
        };

    public Task<RunnerDetailModel> GetRunnerDetail(Guid id) =>
        Context.Runners
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new RunnerDetailModel
            {
                Id = r.Id,
                NumberId = r.NumberId,
                RequestId = r.RequestId,
                WorkerProfileId = r.WorkerProfileId,
                Name = r.WorkerProfile.FirstName + " " + r.WorkerProfile.LastName,
                Email = r.WorkerProfile.Worker.Email,
                Type = r.Type,
                Status = r.Status,
                StartDate = r.StartDate,
                CreatedAt = r.CreatedAt,
                CreatedBy = r.CreatedBy,
                UpdatedAt = r.UpdatedAt,
                StatusHistory = r.StatusHistory
                    .OrderByDescending(h => h.ChangedAt)
                    .Select(h => new RunnerStatusHistoryModel
                    {
                        Id = h.Id,
                        PreviousStatus = h.PreviousStatus,
                        NewStatus = h.NewStatus,
                        ChangedBy = h.ChangedBy,
                        ChangedByEmail = h.ChangedByUser.Email,
                        ChangedAt = h.ChangedAt,
                        Comments = h.Comments
                    }),
                Interviews = r.Interviews
                    .OrderByDescending(i => i.ScheduledDate)
                    .Select(i => new RunnerInterviewModel
                    {
                        Id = i.Id,
                        ScheduledDate = i.ScheduledDate,
                        Type = i.Type,
                        Interviewer = i.Interviewer,
                        Status = i.Status,
                        Feedback = i.Feedback,
                        Notes = i.Notes,
                        RescheduleCount = i.RescheduleCount,
                        CreatedAt = i.CreatedAt,
                        CreatedBy = i.CreatedBy,
                        RescheduledAt = i.RescheduledAt
                    })
            })
            .SingleOrDefaultAsync();

    private static Expression<Func<RunnerListModel, bool>> ApplyFilter(Guid agencyId, GetRunnersFilter filter)
    {
        Expression<Func<RunnerListModel, bool>> predicate = r => r.AgencyId == agencyId;
        if (filter.RequestId.HasValue)
            predicate = predicate.And(r => r.RequestId == filter.RequestId.Value);
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var name = filter.Name.ToLower();
            predicate = predicate.And(r => r.Name.ToLower().Contains(name) || r.Email.ToLower().Contains(name));
        }
        if (filter.Type.HasValue)
            predicate = predicate.And(r => r.Type == filter.Type.Value);
        if (filter.Statuses != null && filter.Statuses.Any())
            predicate = predicate.And(r => filter.Statuses.Contains(r.Status));
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(r => r.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && r.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
        return predicate;
    }

    private static IQueryable<RunnerListModel> ApplySort(IQueryable<RunnerListModel> query, GetRunnersFilter filter) =>
        filter.SortBy switch
        {
            GetRunnersSortBy.Name => query.AddOrderBy(filter, r => r.Name),
            GetRunnersSortBy.Status => query.AddOrderBy(filter, r => r.Status),
            GetRunnersSortBy.Type => query.AddOrderBy(filter, r => r.Type),
            GetRunnersSortBy.CreatedAt => query.AddOrderBy(filter, r => r.CreatedAt),
            _ => query.AddOrderBy(filter, r => r.CreatedAt)
        };
}
