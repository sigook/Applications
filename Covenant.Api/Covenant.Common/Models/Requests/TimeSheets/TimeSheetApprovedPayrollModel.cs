namespace Covenant.Common.Models.Requests.TimeSheets;

public class TimeSheetApprovedPayrollModel(
    TimeSpan overtimeStartsAfter,
    Guid requestId,
    decimal workerRate,
    bool breakIsPaid,
    TimeSpan durationBreak,
    bool holidayIsPaid,
    Guid workerId,
    Guid workerProfileId,
    Guid timeSheetId,
    int week,
    DateTime date,
    DateTime? timeInApproved,
    DateTime? timeOutApproved,
    TimeSpan missingHours,
    TimeSpan missingHoursOvertime,
    decimal missingRateWorker,
    bool isHoliday,
    decimal bonusOrOthers,
    string bonusOrOthersDescription,
    decimal deductionsOthers,
    string deductionsOthersDescription,
    decimal reimbursements,
    string reimbursementsDescription) : ITimeSheetHoursCalculable
{
    public TimeSpan OvertimeStartsAfter { get; } = overtimeStartsAfter;
    public Guid RequestId { get; } = requestId;
    public decimal WorkerRate { get; } = workerRate;
    public bool BreakIsPaid { get; } = breakIsPaid;
    public TimeSpan DurationBreak { get; } = durationBreak;
    public bool HolidayIsPaid { get; } = holidayIsPaid;
    public Guid WorkerId { get; } = workerId;
    public Guid WorkerProfileId { get; } = workerProfileId;
    public Guid TimeSheetId { get; } = timeSheetId;
    public double Week { get; } = week;
    public DateTime Date { get; } = date;
    public DateTime? TimeInApproved { get; } = timeInApproved;
    public DateTime? TimeOutApproved { get; } = timeOutApproved;
    public TimeSpan MissingHours { get; } = missingHours;
    public TimeSpan MissingHoursOvertime { get; } = missingHoursOvertime;
    public decimal MissingRateWorker { get; } = missingRateWorker;
    public bool IsHoliday { get; } = isHoliday;
    public decimal BonusOrOthers { get; } = bonusOrOthers;
    public string BonusOrOthersDescription { get; } = bonusOrOthersDescription;
    public decimal DeductionsOthers { get; } = deductionsOthers;
    public string DeductionsOthersDescription { get; } = deductionsOthersDescription;
    public decimal Reimbursements { get; set; } = reimbursements;
    public string ReimbursementsDescription { get; set; } = reimbursementsDescription;
    public string TypeOfWork { get; set; }
    public string CountryCode { get; set; }

    public static IEnumerable<IGrouping<double, TimeSheetApprovedPayrollModel>> GroupTimeSheetByWeek(IEnumerable<TimeSheetApprovedPayrollModel> timeSheet) =>
        timeSheet.GroupBy(m => m.Week).OrderBy(m => m.Key).ToList();
}