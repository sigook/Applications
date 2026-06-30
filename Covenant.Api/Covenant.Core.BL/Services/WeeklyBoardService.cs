using Covenant.Common.Entities.Agency;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Request.WeeklyBoard;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services;

public class WeeklyBoardService(IRequestRepository requestRepository, IAgencyRepository agencyRepository, IIdentityServerService identityServerService, ITimeService timeService) : IWeeklyBoardService
{
    public async Task<WeeklyBoardModel> GetWeeklyBoard(WeeklyBoardFilter filter)
    {
        var agencyId = identityServerService.GetAgencyId();
        var assignments = (await requestRepository.GetWeeklyBoardAssignments(agencyId, filter.From, filter.To)).ToList();

        var recruiters = assignments
            .GroupBy(a => new { a.RecruiterId, a.RecruiterName })
            .Select(g => new WeeklyBoardRecruiterRowModel
            {
                RecruiterId = g.Key.RecruiterId,
                RecruiterName = g.Key.RecruiterName,
                OrdersCount = g.Select(a => a.RequestId).Distinct().Count(),
                WorkersSent = g.Sum(a => a.WorkersSent),
                Assignments = g.OrderBy(a => a.WorkDate).ThenBy(a => a.NumberId).ToList()
            })
            .OrderBy(r => r.RecruiterName)
            .ToList();

        return new WeeklyBoardModel
        {
            WeekStart = filter.From.Date,
            WeekEnd = filter.To.Date,
            TotalAssignments = assignments.Count,
            TotalWorkersSent = assignments.Sum(a => a.WorkersSent),
            Recruiters = recruiters
        };
    }

    public async Task<RecruiterWeeklyBoardModel> GetRecruiterWeeklyBoard(WeeklyBoardFilter filter)
    {
        var agencyId = identityServerService.GetAgencyId();
        var recruiter = await GetCurrentRecruiter(agencyId);
        var model = new RecruiterWeeklyBoardModel { WeekStart = filter.From.Date, WeekEnd = filter.To.Date };
        if (recruiter is null) return model;

        model.RecruiterName = recruiter.Name;

        var assignments = (await requestRepository.GetWeeklyBoardAssignmentsForRecruiter(agencyId, recruiter.Id, filter.From, filter.To)).ToList();

        model.Assignments = assignments.OrderBy(a => a.WorkDate).ThenBy(a => a.NumberId).ToList();
        model.OrdersCount = assignments.Select(a => a.RequestId).Distinct().Count();
        model.WorkersSent = assignments.Sum(a => a.WorkersSent);
        return model;
    }

    public async Task<Result> AssignRecruiters(AssignRecruitersModel model)
    {
        var agencyId = identityServerService.GetAgencyId();
        var request = await requestRepository.GetRequest(r => r.Id == model.RequestId && r.AgencyId == agencyId);
        if (request is null) return Result.Fail("Order not found");

        var recruiterIds = model.RecruiterIds.Distinct().ToList();
        var personnel = await agencyRepository.GetPersonnels(recruiterIds);
        if (personnel.Count != recruiterIds.Count) return Result.Fail("Recruiter not found");

        var now = timeService.GetCurrentDateTime();
        foreach (var recruiter in personnel)
            foreach (var workDate in model.WorkDates.Select(d => d.Date).Distinct())
            {
                Result result = request.AddRecruiter(recruiter, now, workDate);
                if (!result) return result;
            }

        await requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UnassignRecruiter(Guid requestId, Guid recruiterId, DateTime workDate)
    {
        var agencyId = identityServerService.GetAgencyId();
        var request = await requestRepository.GetRequest(r => r.Id == requestId && r.AgencyId == agencyId);
        if (request is null) return Result.Fail("Order not found");

        Result result = request.RemoveRecruiter(recruiterId, timeService.GetCurrentDateTime(), workDate);
        if (!result) return result;

        await requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> MoveAssignment(MoveAssignmentModel model)
    {
        var agencyId = identityServerService.GetAgencyId();
        var request = await requestRepository.GetRequest(r => r.Id == model.RequestId && r.AgencyId == agencyId);
        if (request is null) return Result.Fail("Order not found");

        if (model.FromRecruiterId != model.ToRecruiterId)
        {
            var personnel = await agencyRepository.GetPersonnels([model.ToRecruiterId]);
            if (personnel.Count == 0 || personnel[0].AgencyId != agencyId) return Result.Fail("Recruiter not found");
        }

        Result result = request.MoveRecruiterAssignment(model.FromRecruiterId, model.FromWorkDate, model.ToRecruiterId, model.ToWorkDate, timeService.GetCurrentDateTime());
        if (!result) return result;

        await requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> AddWorkers(DispatchWorkersModel model)
    {
        var agencyId = identityServerService.GetAgencyId();
        var recruiter = await GetCurrentRecruiter(agencyId);
        if (recruiter is null) return Result.Fail("Recruiter not found");

        var assignment = await requestRepository.GetRequestRecruiter(agencyId, model.RequestId, recruiter.Id, model.WorkDate);
        if (assignment is null) return Result.Fail("Assignment not found");

        var now = timeService.GetCurrentDateTime();
        foreach (var workerProfileId in model.WorkerProfileIds.Distinct())
            assignment.AddDispatch(workerProfileId, now, recruiter.UserId);

        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> RemoveWorker(Guid requestId, DateTime workDate, Guid workerProfileId)
    {
        var agencyId = identityServerService.GetAgencyId();
        var recruiter = await GetCurrentRecruiter(agencyId);
        if (recruiter is null) return Result.Fail("Recruiter not found");

        var assignment = await requestRepository.GetRequestRecruiter(agencyId, requestId, recruiter.Id, workDate);
        if (assignment is null) return Result.Fail("Assignment not found");

        assignment.RemoveDispatch(workerProfileId);

        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    private async Task<AgencyPersonnel> GetCurrentRecruiter(Guid agencyId) =>
        (await agencyRepository.GetPersonnelByUserId(identityServerService.GetUserId()))
            .FirstOrDefault(p => p.AgencyId == agencyId);
}
