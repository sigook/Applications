namespace Covenant.Common.Models.Requests.TimeSheets;

public class RequestTimeSheetModel
{
    public string CompanyFullName { get; set; }
    public string AgencyFullName { get; set; }
    public string WorkerFullName { get; set; }
    public DateTime Date { get; set; }
    public DateTime? TimeInApproved { get; set; }
    public DateTime? TimeOutApproved { get; set; }
    public TimeSpan DurationBreak { get; set; }
    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public string Comment { get; set; }
    public string RequestTitle { get; set; }
    public double TotalHours
    {
        get
        {
            if (TimeOutApproved is null) return 0;
            return (TimeOutApproved - TimeInApproved).GetValueOrDefault().TotalHours;
        }
    }
}