namespace Covenant.Common.Entities.Request;

public class TimesheetHistory
{
    public int RowNumber { get; set; }
    public int NumberId { get; set; }
    public string CompanyName { get; set; }
    public string JobTitle { get; set; }
    public bool IsHoliday { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? RegularHours { get; private set; }
    public TimeSpan? HolidayHours { get; private set; }
    public TimeSpan? OvertimeHours { get; private set; }
    public TimeSpan? MissingHours { get; set; }
    public TimeSpan? MissingHoursOvertime { get; set; }
    public Guid WorkerProfileId { get; set; }
}
