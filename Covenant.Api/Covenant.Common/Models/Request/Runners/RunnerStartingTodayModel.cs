namespace Covenant.Common.Models.Request.Runners;

public class RunnerStartingTodayModel
{
    public Guid RunnerId { get; set; }
    public Guid RequestId { get; set; }
    public int RequestNumberId { get; set; }
    public string JobTitle { get; set; }
    public string CompanyName { get; set; }
    public Guid WorkerProfileId { get; set; }
    public string WorkerName { get; set; }
    public DateTime StartDate { get; set; }
    public int DayNumber { get; set; }
}
