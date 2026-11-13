using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Requests.Runners;
using Covenant.Common.Models.Requests.WeeklyBoard;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Requests;
using Covenant.Core.BL.Interfaces.Requests;

namespace Covenant.Core.BL.Services.Requests;

public class WeeklyBoardService(
    IRequestRepository requestRepository,
    IRunnerService runnerService,
    IUserAccountService userAccountService,
    IUserRepository userRepository,
    ICurrentUserService currentUserService,
    TimeProvider timeProvider) : IWeeklyBoardService
{
    public async Task<WeeklyBoardModel> GetWeeklyBoard(WeeklyBoardFilter filter)
    {
        var assignments = (await requestRepository.GetWeeklyBoardAssignments(currentUserService.GetAgencyIds(), filter.From, filter.To)).ToList();

        var recruiters = assignments
            .GroupBy(a => new { a.RecruiterId, a.RecruiterName })
            .Select(g => new WeeklyBoardRecruiterRowModel
            {
                RecruiterId = g.Key.RecruiterId,
                RecruiterName = g.Key.RecruiterName,
                OrdersCount = g.Select(a => a.RequestId).Distinct().Count(),
                WorkersSent = g.Sum(a => a.RunnersSent),
                Assignments = g.OrderBy(a => a.WorkDate).ThenBy(a => a.NumberId).ToList()
            })
            .OrderBy(r => r.RecruiterName)
            .ToList();

        return new WeeklyBoardModel
        {
            WeekStart = filter.From.Date,
            WeekEnd = filter.To.Date,
            TotalAssignments = assignments.Count,
            TotalWorkersSent = assignments.Sum(a => a.RunnersSent),
            Recruiters = recruiters
        };
    }

    public async Task<RecruiterWeeklyBoardModel> GetRecruiterWeeklyBoard(WeeklyBoardFilter filter)
    {
        var recruiter = await GetCurrentRecruiter();
        var model = new RecruiterWeeklyBoardModel { WeekStart = filter.From.Date, WeekEnd = filter.To.Date };
        if (recruiter is null) return model;

        model.RecruiterName = recruiter.Name ?? recruiter.Email;

        var assignments = (await requestRepository.GetWeeklyBoardAssignmentsForRecruiter(currentUserService.GetAgencyIds(), recruiter.Id, filter.From, filter.To)).ToList();

        model.Assignments = [.. assignments.OrderBy(a => a.WorkDate).ThenBy(a => a.NumberId)];
        model.OrdersCount = assignments.Select(a => a.RequestId).Distinct().Count();
        model.WorkersSent = assignments.Sum(a => a.RunnersSent);
        return model;
    }

    public Task<IEnumerable<WeeklyBoardRunnerModel>> GetOrderRunners(Guid requestId) =>
        requestRepository.GetOrderRunners(requestId);

    public async Task<Result> AssignRecruiters(AssignRecruitersModel model)
    {
        var request = await requestRepository.GetRequest(r => r.Id == model.RequestId);
        if (request is null) return Result.Fail("Order not found");

        var recruiterIds = model.RecruiterIds.Distinct().ToList();
        var personnel = await GetAgencyStaffUserIds(recruiterIds, request.CompanyProfile.AgencyId);
        if (personnel.Count != recruiterIds.Count) return Result.Fail("Recruiter not found in the order's agency");

        var now = timeProvider.GetLocalNow().DateTime;
        foreach (var recruiter in recruiterIds)
            foreach (var workDate in model.WorkDates.Select(d => d.Date).Distinct())
            {
                Result result = request.AddRecruiter(recruiter, now, workDate);
                if (!result) return result;
            }

        requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UnassignRecruiter(Guid requestId, Guid recruiterId, DateTime workDate)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null) return Result.Fail("Order not found");

        Result result = request.RemoveRecruiter(recruiterId, timeProvider.GetLocalNow().DateTime, workDate);
        if (!result) return result;

        requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> MoveAssignment(MoveAssignmentModel model)
    {
        var request = await requestRepository.GetRequest(r => r.Id == model.RequestId);
        if (request is null) return Result.Fail("Order not found");

        if (model.FromRecruiterId != model.ToRecruiterId)
        {
            var personnel = await GetAgencyStaffUserIds([model.ToRecruiterId], request.CompanyProfile.AgencyId);
            if (personnel.Count == 0) return Result.Fail("Recruiter not found in the order's agency");
        }

        Result result = request.MoveRecruiterAssignment(model.FromRecruiterId, model.FromWorkDate, model.ToRecruiterId, model.ToWorkDate, timeProvider.GetLocalNow().DateTime);
        if (!result) return result;

        requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> AddRunner(AddRunnerModel model)
    {
        var recruiter = await GetCurrentRecruiter();
        if (recruiter is null) return Result.Fail("Recruiter not found");

        var assignment = await requestRepository.GetRequestRecruiter(model.RequestId, recruiter.Id, model.WorkDate);
        if (assignment is null) return Result.Fail("Assignment not found");

        var created = await runnerService.CreateRunner(
            model.RequestId,
            new RunnerCreateModel { WorkerProfileId = model.WorkerProfileId, Type = model.Type },
            assignment.Id);
        if (!created) return Result.Fail(created.Errors);

        var names = await requestRepository.GetWorkerProfileNames([model.WorkerProfileId]);
        if (names.TryGetValue(model.WorkerProfileId, out var name) && !string.IsNullOrWhiteSpace(name))
        {
            var note = CovenantNote.Create($"{name} was sent", CovenantNote.RedColor, recruiter.Name ?? recruiter.Email).Value;
            await requestRepository.CreateRange([new RequestNote(model.RequestId, note)]);
            await requestRepository.SaveChangesAsync();
        }

        return Result.Ok();
    }

    private Task<User> GetCurrentRecruiter() => userRepository.GetUserById(currentUserService.GetUserId());

    private async Task<List<Guid>> GetAgencyStaffUserIds(IEnumerable<Guid> userIds, Guid agencyId) =>
        [.. (await userAccountService.GetUsersAgencies(userIds)).Where(m => m.AgencyId == agencyId).Select(m => m.UserId).Distinct()];
}
