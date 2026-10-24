using Covenant.Common.Enums;

namespace Covenant.Common.Models.Requests.Runners;

public class RunnerInterviewCreateModel
{
    public DateTime ScheduledDate { get; set; }
    public InterviewType Type { get; set; }
    public string Interviewer { get; set; }
    public string Notes { get; set; }
}
