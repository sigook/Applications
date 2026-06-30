using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.Runners;

public class RunnerInterviewModel
{
    public Guid Id { get; set; }
    public DateTime ScheduledDate { get; set; }
    public InterviewType Type { get; set; }
    public string Interviewer { get; set; }
    public InterviewStatus Status { get; set; }
    public string Feedback { get; set; }
    public string Notes { get; set; }
    public int RescheduleCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? RescheduledAt { get; set; }
}
