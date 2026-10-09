using Covenant.Common.Enums;

namespace Covenant.Common.Models.Agency;

public class UserAttendanceTodayModel
{
    public Guid UserId { get; set; }
    public AttendanceStatus Status { get; set; }
    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public decimal? WorkedHours { get; set; }
}
