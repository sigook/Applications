namespace Covenant.Common.Models.Requests.TimeSheets.Companies;

public class CompanyTimeSheetListModel
{
    public Guid Id { get; set; }
    public DateTime Day { get; set; }
    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public DateTime? ClockInRounded { get; set; }
    public DateTime? ClockOutRounded { get; set; }
    public DateTime TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public string Comment { get; set; }
    public DateTime? TimeInApproved { get; set; }
    public DateTime? TimeOutApproved { get; set; }
    public bool CanUpdate { get; set; }
    public bool WasApproved { get; set; }
    public TimeSpan MissingHours { get; set; }
    public TimeSpan MissingHoursOvertime { get; set; }
    public decimal DeductionsOthers { get; set; }
    public int Week { get; set; }
    public decimal BonusOrOthers { get; set; }
    public string DeductionsOthersDescription { get; set; }
    public string BonusOrOthersDescription { get; set; }
    public double TotalHours { get; set; }
    public double TotalHoursApproved { get; set; }
}
