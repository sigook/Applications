namespace Covenant.Common.Models.Agencies;

public class UpdateUserAttendanceModel
{
    public DateTime ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public int LunchMinutes { get; set; }
    public string Reason { get; set; }
}
