using Covenant.Common.Models.Workers;
namespace Covenant.Common.Models.Requests.Workers;

public class WorkerRequestApplyModel
{
    public int? NumberId { get; set; }
    public string Email { get; set; }
    public string Comments { get; set; }
}
