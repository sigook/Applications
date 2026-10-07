namespace Covenant.Common.Entities.Agency;

public class UserAttendance
{
    public const int DefaultLunchMinutes = 60;

    private UserAttendance() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public DateTime Date { get; private set; }
    public DateTime ClockIn { get; private set; }
    public DateTime? ClockOut { get; private set; }
    public int LunchMinutes { get; private set; }
    public string EditedBy { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public string EditReason { get; private set; }

    public bool IsOpen => !ClockOut.HasValue;

    public static UserAttendance Start(Guid userId, DateTime now) =>
        new()
        {
            UserId = userId,
            Date = now.Date,
            ClockIn = now,
            LunchMinutes = now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? 0 : DefaultLunchMinutes
        };

    public void Stop(DateTime now) => ClockOut = now;

    public void Edit(DateTime clockIn, DateTime? clockOut, int lunchMinutes, string reason, string editedBy, DateTime now)
    {
        ClockIn = clockIn;
        ClockOut = clockOut;
        LunchMinutes = lunchMinutes;
        EditReason = reason;
        EditedBy = editedBy;
        EditedAt = now;
    }
}
