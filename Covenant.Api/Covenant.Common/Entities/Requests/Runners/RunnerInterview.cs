using Covenant.Common.Enums;
using Covenant.Common.Functionals;

namespace Covenant.Common.Entities.Request.Runners;

public class RunnerInterview
{
    private RunnerInterview()
    {
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid RunnerId { get; private set; }
    public Runner Runner { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public InterviewType Type { get; private set; }
    public string Interviewer { get; private set; }
    public InterviewStatus Status { get; private set; }
    public string Feedback { get; private set; }
    public string Notes { get; private set; }
    public int RescheduleCount { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public Guid CreatedBy { get; private set; }
    public User CreatedByUser { get; private set; }
    public DateTime? RescheduledAt { get; private set; }
    public Guid? RescheduledBy { get; private set; }
    public User RescheduledByUser { get; private set; }

    public static RunnerInterview Create(Guid runnerId, DateTime scheduledDate, InterviewType type, string interviewer, string notes, Guid createdBy) =>
        new()
        {
            RunnerId = runnerId,
            ScheduledDate = scheduledDate,
            Type = type,
            Interviewer = interviewer,
            Notes = notes,
            Status = InterviewStatus.Scheduled,
            CreatedBy = createdBy
        };

    public Result Reschedule(DateTime newDate, Guid rescheduledBy)
    {
        ScheduledDate = newDate;
        Status = InterviewStatus.Rescheduled;
        RescheduleCount++;
        RescheduledAt = DateTime.Now;
        RescheduledBy = rescheduledBy;
        return Result.Ok();
    }
}
