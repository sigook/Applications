using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;

namespace Covenant.Common.Entities.Request.Runners;

public class Runner
{
    private Runner()
    {
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public long NumberId { get; private set; }

    public Guid AgencyId { get; private set; }
    public Agency.Agency Agency { get; private set; }

    public Guid RequestId { get; private set; }
    public Request Request { get; private set; }

    public Guid WorkerProfileId { get; private set; }
    public WorkerProfile WorkerProfile { get; private set; }

    public Guid? RequestRecruiterId { get; private set; }
    public RequestRecruiter RequestRecruiter { get; private set; }

    public RunnerType Type { get; private set; }
    public RunnerStatus Status { get; private set; }
    public DateTime? StartDate { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private readonly List<RunnerStatusHistory> _statusHistory = new();
    public IEnumerable<RunnerStatusHistory> StatusHistory => _statusHistory;

    private readonly List<RunnerInterview> _interviews = new();
    public IEnumerable<RunnerInterview> Interviews => _interviews;

    public static Result<Runner> CreateFromWorker(Guid agencyId, Guid requestId, Guid workerProfileId, RunnerType type, Guid createdBy, Guid? requestRecruiterId = null)
    {
        var runner = new Runner
        {
            AgencyId = agencyId,
            RequestId = requestId,
            WorkerProfileId = workerProfileId,
            RequestRecruiterId = requestRecruiterId,
            Type = type,
            Status = RunnerStatus.SentToClient,
            CreatedBy = createdBy
        };
        runner._statusHistory.Add(RunnerStatusHistory.Create(runner.Id, null, RunnerStatus.SentToClient, createdBy, null));
        return Result.Ok(runner);
    }

    public static bool CanAddInterview(RunnerStatus status) =>
        status is RunnerStatus.InterviewScheduled or RunnerStatus.InterviewRescheduled;

    public Result ChangeStatus(RunnerStatus next, Guid changedBy, string comments = null, DateTime? startDate = null)
    {
        if (Status == RunnerStatus.Hired)
            return Result.Fail("A hired runner's status cannot be changed");
        if (next == RunnerStatus.Hired && startDate is null)
            return Result.Fail("A start date is required to hire a runner");
        var previous = Status;
        Status = next;
        UpdatedAt = DateTime.Now;
        UpdatedBy = changedBy;
        if (next == RunnerStatus.Hired) StartDate = startDate;
        _statusHistory.Add(RunnerStatusHistory.Create(Id, previous, next, changedBy, comments));
        return Result.Ok();
    }

    public Result<RunnerInterview> AddInterview(DateTime scheduledDate, InterviewType type, string interviewer, string notes, Guid createdBy)
    {
        if (!CanAddInterview(Status))
            return Result.Fail<RunnerInterview>("Interviews can only be added when the runner status is 'Interview scheduled' or 'Interview rescheduled'");
        var interview = RunnerInterview.Create(Id, scheduledDate, type, interviewer, notes, createdBy);
        _interviews.Add(interview);
        UpdatedAt = DateTime.Now;
        UpdatedBy = createdBy;
        return Result.Ok(interview);
    }

    public Result RescheduleInterview(Guid interviewId, DateTime newDate, Guid rescheduledBy)
    {
        if (!CanAddInterview(Status))
            return Result.Fail("Interviews can only be rescheduled when the runner status is 'Interview scheduled' or 'Interview rescheduled'");
        var interview = _interviews.FirstOrDefault(i => i.Id == interviewId);
        if (interview is null) return Result.Fail("Interview not found");
        var result = interview.Reschedule(newDate, rescheduledBy);
        if (!result) return result;
        UpdatedAt = DateTime.Now;
        UpdatedBy = rescheduledBy;
        if (Status != RunnerStatus.InterviewRescheduled)
            ChangeStatus(RunnerStatus.InterviewRescheduled, rescheduledBy);
        return Result.Ok();
    }
}
