namespace Covenant.Common.Models.Agency;

public class GetUserAttendanceReportFilter
{
    public Guid? UserId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}
