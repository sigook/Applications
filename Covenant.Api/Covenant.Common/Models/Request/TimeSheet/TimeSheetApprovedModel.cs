namespace Covenant.Common.Models.Request.TimeSheet
{
    public class TimeSheetApprovedModel
    {
        public Guid AgencyId { get; }
        public Guid CompanyId { get; }
        public string FullName { get; }
        public Guid CompanyProfileId { get; }
        public bool PaidHolidays { get; }
        public TimeSpan OvertimeStartsAfter { get; }
        public Guid RequestId { get; }
        public decimal AgencyRate { get; }
        public decimal WorkerRate { get; }
        public bool PayNightShift { get; }
        public TimeSpan? TimeStartNightShift { get; }
        public TimeSpan? TimeEndNightShift { get; }
        public bool BreakIsPaid { get; }
        public TimeSpan DurationBreak { get; }
        public bool HolidayIsPaid { get; }
        public Guid WorkerId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public Guid WorkerProfileId { get; }
        public bool IsSubcontractor { get; }
        public Guid TimeSheetId { get; }
        public double Week { get; }
        public double Year { get; }
        public DateTime Date { get; }
        public DateTime? TimeInApproved { get; }
        public DateTime? TimeOutApproved { get; }
        public TimeSpan MissingHours { get; }
        public TimeSpan MissingHoursOvertime { get; }
        public decimal MissingRateWorker { get; }
        public decimal MissingRateAgency { get; }
        public bool IsHoliday { get; }
        public decimal BonusOrOthers { get; }
        public string BonusOrOthersDescription { get; }
        public decimal DeductionsOthers { get; }
        public string DeductionsOthersDescription { get; }
        public string TypeOfWork { get; set; }
        public TimeSheetApprovedModel(Guid agencyId,
            Guid companyId,
            string fullName,
            Guid companyProfileId,
            bool paidHolidays,
            TimeSpan overtimeStartsAfter,
            Guid requestId,
            decimal agencyRate,
            decimal workerRate,
            bool payNightShift,
            TimeSpan? timeStartNightShift,
            TimeSpan? timeEndNightShift,
            bool breakIsPaid,
            TimeSpan durationBreak,
            bool holidayIsPaid,
            Guid workerId,
            string firstName,
            string lastName,
            Guid workerProfileId,
            bool isSubcontractor,
            Guid timeSheetId,
            int week,
            DateTime date,
            DateTime? timeInApproved,
            DateTime? timeOutApproved,
            TimeSpan missingHours,
            TimeSpan missingHoursOvertime,
            decimal missingRateWorker,
            decimal missingRateAgency,
            bool isHoliday,
            decimal bonusOrOthers,
            string bonusOrOthersDescription,
            decimal deductionsOthers,
            string deductionsOthersDescription)
        {
            AgencyId = agencyId;
            CompanyId = companyId;
            FullName = fullName;
            CompanyProfileId = companyProfileId;
            PaidHolidays = paidHolidays;
            OvertimeStartsAfter = overtimeStartsAfter;
            RequestId = requestId;
            AgencyRate = agencyRate;
            WorkerRate = workerRate;
            PayNightShift = payNightShift;
            TimeStartNightShift = timeStartNightShift;
            TimeEndNightShift = timeEndNightShift;
            BreakIsPaid = breakIsPaid;
            DurationBreak = durationBreak;
            HolidayIsPaid = holidayIsPaid;
            WorkerId = workerId;
            FirstName = firstName;
            LastName = lastName;
            WorkerProfileId = workerProfileId;
            IsSubcontractor = isSubcontractor;
            TimeSheetId = timeSheetId;
            Week = week;
            Year = Date.Year;
            Date = date;
            TimeInApproved = timeInApproved;
            TimeOutApproved = timeOutApproved;
            MissingHours = missingHours;
            MissingHoursOvertime = missingHoursOvertime;
            MissingRateWorker = missingRateWorker;
            MissingRateAgency = missingRateAgency;
            IsHoliday = isHoliday;
            BonusOrOthers = bonusOrOthers;
            BonusOrOthersDescription = bonusOrOthersDescription;
            DeductionsOthers = deductionsOthers;
            DeductionsOthersDescription = deductionsOthersDescription;
        }
    }
}