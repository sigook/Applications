namespace Covenant.Common.Models.Agency;

public class UserAttendanceReportModel
{
    public List<UserAttendanceListModel> Items { get; set; } = [];
    public UserAttendanceHoursModel Totals { get; set; } = new();
}
