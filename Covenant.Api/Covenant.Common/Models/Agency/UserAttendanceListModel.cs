namespace Covenant.Common.Models.Agency;

public class UserAttendanceListModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public DateTime ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public int LunchMinutes { get; set; }
    public decimal WorkedHours { get; set; }
    public decimal LunchHours { get; set; }
    public decimal RegularHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public bool IsWeekend { get; set; }
    public bool IsMissingClockOut { get; set; }
    public bool IsEdited { get; set; }
    public string EditedBy { get; set; }
    public DateTime? EditedAt { get; set; }
    public string EditReason { get; set; }
}
