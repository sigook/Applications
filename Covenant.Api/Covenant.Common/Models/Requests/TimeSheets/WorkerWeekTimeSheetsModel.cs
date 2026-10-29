namespace Covenant.Common.Models.Requests.TimeSheets;

public class WorkerWeekTimeSheetsModel
{
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public WorkerWeekHoursModel Hours { get; set; } = new();
    public List<WorkerWeekTimeSheetModel> Items { get; set; } = [];
}

public class WorkerWeekHoursModel
{
    public double Total { get; set; }
    public double Regular { get; set; }
    public double Overtime { get; set; }
    public double Holiday { get; set; }
    public double PendingApproval { get; set; }
}
