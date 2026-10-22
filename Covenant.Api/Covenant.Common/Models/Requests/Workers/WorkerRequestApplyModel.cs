using Covenant.Common.Models.Worker;
namespace Covenant.Common.Models.Request.Worker;

public class WorkerRequestApplyModel
{
    public int? NumberId { get; set; }
    public string Email { get; set; }
    public string Comments { get; set; }
}
