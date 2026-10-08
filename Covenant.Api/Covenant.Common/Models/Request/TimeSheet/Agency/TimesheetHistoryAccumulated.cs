namespace Covenant.Common.Models.Request.TimeSheet.Agency;

public class TimesheetHistoryAccumulated
{
    public double RegularHours { get; set; }
    public double HolidayHours { get; set; }
    public double OvertimeHours { get; set; }
    public double MissingHours { get; set; }
    public double MissingHoursOvertime { get; set; }
    public double TotalHours => RegularHours + HolidayHours + OvertimeHours + MissingHours + MissingHoursOvertime;
}
