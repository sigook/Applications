using Covenant.Common.Enums;

namespace Covenant.Common.Models.Requests.TimeSheets;

public class PunchStatusModel
{
    public ClockType ClockType { get; set; }
    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public double DayHours { get; set; }
    public double WeekHours { get; set; }
}
