namespace Covenant.Common.Models.Requests.TimeSheets;

/// <summary>
/// Interface for timesheet models that can be used for hours calculation
/// </summary>
public interface ITimeSheetHoursCalculable
{
    DateTime? TimeInApproved { get; }
    DateTime? TimeOutApproved { get; }
    TimeSpan DurationBreak { get; }
    bool BreakIsPaid { get; }
    TimeSpan OvertimeStartsAfter { get; }
}
