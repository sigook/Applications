using Covenant.Common.Enums;

namespace Covenant.Common.Models.Request.Runners;

public class RunnerDetailModel
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
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IEnumerable<RunnerStatusHistoryModel> StatusHistory { get; set; } = new List<RunnerStatusHistoryModel>();
    public IEnumerable<RunnerInterviewModel> Interviews { get; set; } = new List<RunnerInterviewModel>();
}
