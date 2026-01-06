namespace Covenant.Common.Models.Request.TimeSheet
{
    public class TimeSheetApprovedBillingModel : ITimeSheetHoursCalculable
    {
        public bool PaidHolidays { get; set; }
        public TimeSpan OvertimeStartsAfter { get; set; }
        public Guid RequestId { get; set; }
        public string JobTitle { get; set; }
        public decimal AgencyRate { get; set; }
        public bool BreakIsPaid { get; set; }
        public TimeSpan DurationBreak { get; set; }
        public bool HolidayIsPaid { get; set; }
        public Guid WorkerId { get; set; }
        public Guid TimeSheetId { get; set; }
        public double Week { get; set; }
        public DateTime Date { get; set; }
        public DateTime? TimeInApproved { get; set; }
        public DateTime? TimeOutApproved { get; set; }
        public TimeSpan MissingHours { get; set; }
        public TimeSpan MissingHoursOvertime { get; set; }
        public decimal MissingRateAgency { get; set; }
        public bool IsHoliday { get; set; }
        public TimeSpan MaxHoursWeek { get; set; }
        public decimal OverTime { get; set; }
        public decimal NightShift { get; set; }
        public decimal Holiday { get; set; }
    }
}