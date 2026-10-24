namespace Covenant.Common.Models.Requests.TimeSheets.Agencies;

public class TimeSheetHistoryModel
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public int NumberId { get; set; }
    public string BusinessName { get; set; }
    public string JobTitle { get; set; }
    public DateTime Date { get; set; }
    public bool IsHoliday { get; set; }
    public double RegularHours { get; set; }
    public double HolidayHours { get; set; }
    public double OvertimeHours { get; set; }
    public double MissingHours { get; set; }
    public double MissingHoursOvertime { get; set; }
    public double TotalHours => RegularHours + HolidayHours + OvertimeHours + MissingHours + MissingHoursOvertime;
    public double AccumulatedRegularHours { get; set; }
    public double AccumulatedHolidayHours { get; set; }
    public double AccumulatedOvertimeHours { get; set; }
    public double AccumulatedMissingHours { get; set; }
    public double AccumulatedMissingHoursOvertime { get; set; }
    public double AccumulatedTotalHours => AccumulatedRegularHours + AccumulatedHolidayHours + AccumulatedOvertimeHours + AccumulatedMissingHours + AccumulatedMissingHoursOvertime;
}
