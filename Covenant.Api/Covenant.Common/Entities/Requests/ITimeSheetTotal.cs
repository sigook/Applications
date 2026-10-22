namespace Covenant.Common.Entities.Request
{
    public interface ITimeSheetTotal
    {
        Guid Id { get; }
        TimeSpan TotalHours { get; }
        TimeSpan RegularHours { get; }
        TimeSpan OtherRegularHours { get; }
        TimeSpan NightShiftHours { get; }
        TimeSpan HolidayHours { get; }
        TimeSpan OvertimeHours { get; }
        TimeSpan AccumulateWeekHours { get; }
        Guid TimeSheetId { get; }
        TimeSheet TimeSheet { get; }
    }
}