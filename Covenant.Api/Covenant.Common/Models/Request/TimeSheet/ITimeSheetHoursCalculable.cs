namespace Covenant.Common.Models.Request.TimeSheet;

/// <summary>
/// Interface for timesheet models that can be used for hours calculation
/// </summary>
public interface ITimeSheetHoursCalculable
{
    DateTime? TimeInApproved { get; }
    DateTime? TimeOutApproved { get; }
    TimeSpan DurationBreak { get; }
    TimeSpan OvertimeStartsAfter { get; }
}
