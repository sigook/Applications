using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.Runners;

public class RunnerListModel
{
    public Guid Id { get; set; }
    public long NumberId { get; set; }
    public Guid AgencyId { get; set; }
    public Guid RequestId { get; set; }
    public Guid? WorkerProfileId { get; set; }
    public Guid? CandidateId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public RunnerType Type { get; set; }
    public RunnerStatus Status { get; set; }
    public DateTime? StartDate { get; set; }
    public int InterviewsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
