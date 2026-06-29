using Covenant.Common.Entities.Request.Runners;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Request.Runners;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services;

public class RunnerService(
    IRunnerRepository runnerRepository,
    IRequestRepository requestRepository,
    IWorkerRepository workerRepository,
    ICandidateRepository candidateRepository) : IRunnerService
{
    public async Task<Result<Guid>> CreateRunner(Guid agencyId, Guid requestId, RunnerCreateModel model, string createdBy)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId && r.AgencyId == agencyId);
        if (request is null) return Result.Fail<Guid>("Request not found");

        Result<Runner> runnerResult;
        if (model.WorkerProfileId.HasValue)
        {
            var worker = await workerRepository.GetProfile(w => w.Id == model.WorkerProfileId.Value && w.AgencyId == agencyId);
            if (worker is null) return Result.Fail<Guid>("Worker profile not found");
            if (await runnerRepository.RunnerExists(requestId, model.WorkerProfileId, null))
                return Result.Fail<Guid>("This worker is already a runner on this request");
            runnerResult = Runner.CreateFromWorker(agencyId, requestId, model.WorkerProfileId.Value, model.Type, createdBy);
        }
        else if (model.CandidateId.HasValue)
        {
            var candidate = await candidateRepository.GetCandidate(c => c.Id == model.CandidateId.Value && c.AgencyId == agencyId);
            if (candidate is null) return Result.Fail<Guid>("Candidate not found");
            if (await runnerRepository.RunnerExists(requestId, null, model.CandidateId))
                return Result.Fail<Guid>("This candidate is already a runner on this request");
            runnerResult = Runner.CreateFromCandidate(agencyId, requestId, model.CandidateId.Value, model.Type, createdBy);
        }
        else
        {
            return Result.Fail<Guid>("A worker or a candidate must be provided");
        }

        if (!runnerResult) return Result.Fail<Guid>(runnerResult.Errors);
        await runnerRepository.Create(runnerResult.Value);
        await runnerRepository.SaveChangesAsync();
        return Result.Ok(runnerResult.Value.Id);
    }

    public async Task<Result> ChangeStatus(Guid runnerId, ChangeRunnerStatusModel model, string changedBy)
    {
        var runner = await runnerRepository.GetRunner(r => r.Id == runnerId);
        if (runner is null) return Result.Fail("Runner not found");
        var result = runner.ChangeStatus(model.Status, changedBy, model.Comments, model.StartDate);
        if (!result) return result;
        await runnerRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<Guid>> AddInterview(Guid runnerId, RunnerInterviewCreateModel model, string createdBy)
    {
        var runner = await runnerRepository.GetRunner(r => r.Id == runnerId);
        if (runner is null) return Result.Fail<Guid>("Runner not found");
        var result = runner.AddInterview(model.ScheduledDate, model.Type, model.Interviewer, model.Notes, createdBy);
        if (!result) return Result.Fail<Guid>(result.Errors);
        await runnerRepository.SaveChangesAsync();
        return Result.Ok(result.Value.Id);
    }

    public async Task<Result> RescheduleInterview(Guid runnerId, Guid interviewId, RunnerInterviewRescheduleModel model, string rescheduledBy)
    {
        var runner = await runnerRepository.GetRunner(r => r.Id == runnerId);
        if (runner is null) return Result.Fail("Runner not found");
        var result = runner.RescheduleInterview(interviewId, model.NewDate, rescheduledBy);
        if (!result) return result;
        await runnerRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
