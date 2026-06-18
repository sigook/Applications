namespace Covenant.Common.Models.Request.WeeklyBoard;

public class DispatchWorkersModel
{
    public Guid RequestId { get; set; }
    public DateTime WorkDate { get; set; }
    public IEnumerable<Guid> WorkerProfileIds { get; set; }
}
