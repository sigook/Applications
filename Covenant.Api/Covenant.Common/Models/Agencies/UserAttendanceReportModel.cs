namespace Covenant.Common.Models.Agencies;

public class UserAttendanceReportModel
{
    public List<UserAttendanceListModel> Items { get; set; } = [];
    public UserAttendanceHoursModel Totals { get; set; } = new();
}
