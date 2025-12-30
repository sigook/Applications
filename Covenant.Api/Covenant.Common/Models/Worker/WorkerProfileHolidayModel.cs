namespace Covenant.Common.Models.Worker;

public class WorkerProfileHolidayModel
{
    public Guid WorkerProfileId { get; set; }
    public decimal StatPaidWorker { get; set; }
    public Guid HolidayId { get; set; }
    public DateTime Date { get; set; }
}