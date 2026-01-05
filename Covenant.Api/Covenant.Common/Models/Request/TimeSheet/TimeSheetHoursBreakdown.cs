namespace Covenant.Common.Models.Request.TimeSheet;

/// <summary>
/// Helper class to store hours breakdown for a timesheet item
/// </summary>
public class TimeSheetHoursBreakdown
{
    public TimeSpan RegularHours { get; set; }
    public TimeSpan OtherRegularHours { get; set; }
    public TimeSpan OvertimeHours { get; set; }
    public TimeSpan HolidayHours { get; set; }
}
