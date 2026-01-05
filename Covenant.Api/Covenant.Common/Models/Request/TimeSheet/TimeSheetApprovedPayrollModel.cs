namespace Covenant.Common.Models.Request.TimeSheet
{
    public class TimeSheetApprovedPayrollModel : ITimeSheetHoursCalculable
    {
        public TimeSheetApprovedPayrollModel(
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
            string reimbursementsDescription)
        {
            OvertimeStartsAfter = overtimeStartsAfter;
            RequestId = requestId;
            WorkerRate = workerRate;
            BreakIsPaid = breakIsPaid;
            DurationBreak = durationBreak;
            HolidayIsPaid = holidayIsPaid;
            WorkerId = workerId;
            WorkerProfileId = workerProfileId;
            TimeSheetId = timeSheetId;
            Week = week;
            Date = date;
            TimeInApproved = timeInApproved;
            TimeOutApproved = timeOutApproved;
            MissingHours = missingHours;
            MissingHoursOvertime = missingHoursOvertime;
            MissingRateWorker = missingRateWorker;
            IsHoliday = isHoliday;
            BonusOrOthers = bonusOrOthers;
            BonusOrOthersDescription = bonusOrOthersDescription;
            DeductionsOthers = deductionsOthers;
            DeductionsOthersDescription = deductionsOthersDescription;
            Reimbursements = reimbursements;
            ReimbursementsDescription = reimbursementsDescription;
        }

        public TimeSpan OvertimeStartsAfter { get; }
        public Guid RequestId { get; }
        public decimal WorkerRate { get; }
        public bool BreakIsPaid { get; }
        public TimeSpan DurationBreak { get; }
        public bool HolidayIsPaid { get; }
        public Guid WorkerId { get; }
        public Guid WorkerProfileId { get; }
        public Guid TimeSheetId { get; }
        public double Week { get; }
        public DateTime Date { get; }
        public DateTime? TimeInApproved { get; }
        public DateTime? TimeOutApproved { get; }
        public TimeSpan MissingHours { get; }
        public TimeSpan MissingHoursOvertime { get; }
        public decimal MissingRateWorker { get; }
        public bool IsHoliday { get; }
        public decimal BonusOrOthers { get; }
        public string BonusOrOthersDescription { get; }
        public decimal DeductionsOthers { get; }
        public string DeductionsOthersDescription { get; }
        public decimal Reimbursements { get; set; }
        public string ReimbursementsDescription { get; set; }
        public string TypeOfWork { get; set; }

        public static IEnumerable<IGrouping<double, TimeSheetApprovedPayrollModel>> GroupTimeSheetByWeek(IEnumerable<TimeSheetApprovedPayrollModel> timeSheet) =>
            timeSheet.GroupBy(m => m.Week).OrderBy(m => m.Key).ToList();
    }
}