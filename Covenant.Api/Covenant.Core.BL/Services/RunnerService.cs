using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Request.Runners;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services;

public class RunnerService(
    IRunnerRepository runnerRepository,
    IRequestRepository requestRepository,
    IWorkerRepository workerRepository,
    ICurrentUserService currentUserService) : IRunnerService
{
    private const string RunnersNotAllowed = "This order does not accept runners";

    public async Task<Result<Guid>> CreateRunner(Guid requestId, RunnerCreateModel model, Guid? requestRecruiterId = null)
    {
        var agencyId = currentUserService.GetAgencyId();
        var createdBy = currentUserService.GetUserId();
        var request = await requestRepository.GetRequest(r => r.Id == requestId && r.CompanyProfile.AgencyId == agencyId);
        if (request is null) return Result.Fail<Guid>("Request not found");
        if (!request.UsesRunners) return Result.Fail<Guid>(RunnersNotAllowed);

        var worker = await workerRepository.GetProfile(w => w.Id == model.WorkerProfileId && w.AgencyId == agencyId);
        if (worker is null) return Result.Fail<Guid>("Worker profile not found");
        if (await runnerRepository.RunnerExists(requestId, model.WorkerProfileId))
            return Result.Fail<Guid>("This worker is already a runner on this request");

        var runnerResult = Runner.CreateFromWorker(requestId, model.WorkerProfileId, model.Type, createdBy, requestRecruiterId);
        if (!runnerResult) return Result.Fail<Guid>(runnerResult.Errors);
        await runnerRepository.Create(runnerResult.Value);
        await runnerRepository.SaveChangesAsync();
        return Result.Ok(runnerResult.Value.Id);
    }

    public async Task<Result> DeleteRunner(Guid runnerId)
    {
        var agencyId = currentUserService.GetAgencyId();
        var runner = await runnerRepository.GetRunner(r => r.Id == runnerId && r.Request.CompanyProfile.AgencyId == agencyId);
        if (runner is null) return Result.Fail("Runner not found");
        runnerRepository.Delete(runner);
        await runnerRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> ChangeStatus(Guid runnerId, ChangeRunnerStatusModel model)
    {
        var changedBy = currentUserService.GetUserId();
        var runner = await runnerRepository.GetRunner(r => r.Id == runnerId);
        if (runner is null) return Result.Fail("Runner not found");
        if (!await RunnersAllowed(runner.RequestId)) return Result.Fail(RunnersNotAllowed);
        var result = runner.ChangeStatus(model.Status, changedBy, model.Comments, model.StartDate);
        if (!result) return result;
        await runnerRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<Guid>> AddInterview(Guid runnerId, RunnerInterviewCreateModel model)
    {
        var createdBy = currentUserService.GetUserId();
        var runner = await runnerRepository.GetRunner(r => r.Id == runnerId);
        if (runner is null) return Result.Fail<Guid>("Runner not found");
        if (!await RunnersAllowed(runner.RequestId)) return Result.Fail<Guid>(RunnersNotAllowed);
        var result = runner.AddInterview(model.ScheduledDate, model.Type, model.Interviewer, model.Notes, createdBy);
        if (!result) return Result.Fail<Guid>(result.Errors);
        await runnerRepository.SaveChangesAsync();
        return Result.Ok(result.Value.Id);
    }

    public async Task<Result> RescheduleInterview(Guid runnerId, Guid interviewId, RunnerInterviewRescheduleModel model)
    {
        var rescheduledBy = currentUserService.GetUserId();
        var runner = await runnerRepository.GetRunner(r => r.Id == runnerId);
        if (runner is null) return Result.Fail("Runner not found");
        if (!await RunnersAllowed(runner.RequestId)) return Result.Fail(RunnersNotAllowed);
        var result = runner.RescheduleInterview(interviewId, model.NewDate, rescheduledBy);
        if (!result) return result;
        await runnerRepository.SaveChangesAsync();
        return Result.Ok();
    }

    private async Task<bool> RunnersAllowed(Guid requestId)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        return request is not null && request.UsesRunners;
    }
}
