using Covenant.Common.Enums;

namespace Covenant.Common.Models.Requests;

public class FinishAssignmentModel
{
    public DateTime LastDay { get; set; }
    public WorkerRequestFinishReason Reason { get; set; }
    public string Comments { get; set; }
}
