using Covenant.Common.Enums;

namespace Covenant.Common.Models.Requests.Runners;

public class AgencyRunnerListModel
{
    public Guid Id { get; set; }
    public long NumberId { get; set; }
    public Guid RequestId { get; set; }
    public Guid WorkerProfileId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public RunnerType Type { get; set; }
    public RunnerStatus Status { get; set; }
    public DateTime? StartDate { get; set; }
    public int InterviewsCount { get; set; }
    public DateTime? LastInterviewAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedByEmail { get; set; }
    public int RequestNumberId { get; set; }
    public string CompanyFullName { get; set; }
    public string JobTitle { get; set; }
    public DateTime? StartAt { get; set; }
    public bool IsAsap { get; set; }
    public bool IsDirectHiring { get; set; }
    public bool UsesRunners { get; set; }
}
