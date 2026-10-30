using Covenant.Common.Enums;
using System.Text.Json.Serialization;

namespace Covenant.Common.Models.Requests.TimeSheets;

public class WorkerWeekTimeSheetModel
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public DateTime Date { get; set; }
    public string ClientName { get; set; }
    public string JobTitle { get; set; }
    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public DateTime TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public DateTime? TimeInApproved { get; set; }
    public DateTime? TimeOutApproved { get; set; }
    public TimeSpan DurationBreak { get; set; }
    public bool BreakIsPaid { get; set; }
    public double RegularHours { get; set; }
    public double OvertimeHours { get; set; }
    public double HolidayHours { get; set; }
    public double TotalHours { get; set; }
    public WorkerTimeSheetStatus Status { get; set; }
    public TimeSpan MissingHours { get; set; }
    public TimeSpan MissingHoursOvertime { get; set; }
    public decimal BonusOrOthers { get; set; }
    public string BonusOrOthersDescription { get; set; }
    public decimal DeductionsOthers { get; set; }
    public string DeductionsOthersDescription { get; set; }
    public decimal Reimbursements { get; set; }
    public string ReimbursementsDescription { get; set; }

    [JsonIgnore]
    public bool IsHoliday { get; set; }
    [JsonIgnore]
    public bool HolidayIsPaid { get; set; }
    [JsonIgnore]
    public bool IsPaid { get; set; }
    [JsonIgnore]
    public TimeSpan OvertimeStartsAfter { get; set; }
}
