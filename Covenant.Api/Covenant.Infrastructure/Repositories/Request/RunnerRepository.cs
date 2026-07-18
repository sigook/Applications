using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Request.Runners;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Request;

public class RunnerRepository(CovenantContext context) : IRunnerRepository
{
    public Task<Runner> GetRunner(Expression<Func<Runner, bool>> expression) =>
        context.Runners
            .Include(r => r.StatusHistory)
            .Include(r => r.Interviews)
            .FirstOrDefaultAsync(expression);

    public Task<List<Runner>> GetRunners(Expression<Func<Runner, bool>> expression) =>
        context.Runners.Where(expression).ToListAsync();

    public Task<bool> RunnerExists(Guid requestId, Guid? workerProfileId, Guid? candidateId) =>
        context.Runners.AnyAsync(r => r.RequestId == requestId &&
            ((workerProfileId != null && r.WorkerProfileId == workerProfileId) ||
             (candidateId != null && r.CandidateId == candidateId)));

    public async Task<PaginatedList<RunnerListModel>> GetRunners(Guid agencyId, GetRunnersFilter filter)
    {
        var query = context.Runners
            .AsNoTracking()
            .Select(r => new RunnerListModel
            {
                Id = r.Id,
                NumberId = r.NumberId,
                AgencyId = r.AgencyId,
                RequestId = r.RequestId,
                WorkerProfileId = r.WorkerProfileId,
                CandidateId = r.CandidateId,
                Name = r.WorkerProfileId != null ? r.WorkerProfile.FirstName + " " + r.WorkerProfile.LastName : r.Candidate.Name,
                Email = r.WorkerProfileId != null ? r.WorkerProfile.Worker.Email : r.Candidate.Email,
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

    public Task<RunnerDetailModel> GetRunnerDetail(Guid id) =>
        context.Runners
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new RunnerDetailModel
            {
                Id = r.Id,
                NumberId = r.NumberId,
                RequestId = r.RequestId,
                WorkerProfileId = r.WorkerProfileId,
                CandidateId = r.CandidateId,
                Name = r.WorkerProfileId != null ? r.WorkerProfile.FirstName + " " + r.WorkerProfile.LastName : r.Candidate.Name,
                Email = r.WorkerProfileId != null ? r.WorkerProfile.Worker.Email : r.Candidate.Email,
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

    public Task<List<RunnerStartingTodayModel>> GetRunnersStartingToday(Guid agencyId, Guid updatedBy, DateTime windowStart, DateTime windowEnd) =>
        (from r in context.Runners.AsNoTracking()
         join cp in context.CompanyProfile
            on new { r.Request.CompanyId, r.AgencyId } equals new { cp.CompanyId, cp.AgencyId }
         where r.AgencyId == agencyId
               && r.Status == RunnerStatus.Hired
               && r.UpdatedBy == updatedBy
               && r.WorkerProfileId != null
               && (r.Request.WorkerSalary == null || r.Request.WorkerSalary == 0)
               && r.StartDate != null
               && r.StartDate.Value.Date >= windowStart.Date
               && r.StartDate.Value.Date <= windowEnd.Date
         select new RunnerStartingTodayModel
         {
             RunnerId = r.Id,
             RequestId = r.RequestId,
             RequestNumberId = r.Request.NumberId,
             JobTitle = r.Request.JobTitle,
             CompanyName = cp.BusinessName,
             WorkerProfileId = r.WorkerProfileId.Value,
             WorkerName = r.WorkerProfile.FirstName + " " + r.WorkerProfile.LastName,
             StartDate = r.StartDate.Value
         }).Distinct().ToListAsync();

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

    public Task Update<T>(T entity) where T : class => Task.FromResult(context.Set<T>().Update(entity));

    public async Task Create<T>(T entity) where T : class => await context.Set<T>().AddAsync(entity);

    public void Delete<T>(T entity) where T : class => context.Set<T>().Remove(entity);

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
