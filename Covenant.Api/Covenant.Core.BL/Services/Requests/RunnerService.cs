using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Request.Runners;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Requests;
using FluentValidation;

namespace Covenant.Core.BL.Services.Requests;

public class RunnerService(
    IRunnerRepository runnerRepository,
    IRequestRepository requestRepository,
    IWorkerRepository workerRepository,
    ICurrentUserService currentUserService,
    IValidator<ChangeRunnersStatusModel> changeRunnersStatusValidator) : IRunnerService
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

    public Task<AgencyRunnersPagedResponse> GetAgencyRunners(GetAgencyRunnersFilter filter) =>
        runnerRepository.GetAgencyRunners(currentUserService.GetAgencyId(), filter);

    public async Task<Result<ChangeRunnersStatusResultModel>> ChangeRunnersStatus(ChangeRunnersStatusModel model)
    {
        var validationResult = await changeRunnersStatusValidator.ValidateAsync(model);
        if (!validationResult.IsValid) return validationResult.ToResultFailure<ChangeRunnersStatusResultModel>();
        var agencyId = currentUserService.GetAgencyId();
        var changedBy = currentUserService.GetUserId();
        var runners = await runnerRepository.GetRunners(r => model.RunnerIds.Contains(r.Id) && r.Request.CompanyProfile.AgencyId == agencyId);
        var response = new ChangeRunnersStatusResultModel();
        foreach (var runner in runners)
        {
            var result = runner.Request.UsesRunners
                ? runner.ChangeStatus(model.Status, changedBy, model.Comments, model.StartDate)
                : Result.Fail(RunnersNotAllowed);
            if (result) response.Updated++;
            else response.Skipped.Add(new SkippedRunnerModel { RunnerId = runner.Id, Reason = result.StringErrors });
        }
        response.Skipped.AddRange(model.RunnerIds
            .Where(id => runners.TrueForAll(r => r.Id != id))
            .Select(id => new SkippedRunnerModel { RunnerId = id, Reason = "Runner not found" }));
        if (response.Updated > 0) await runnerRepository.SaveChangesAsync();
        return Result.Ok(response);
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
