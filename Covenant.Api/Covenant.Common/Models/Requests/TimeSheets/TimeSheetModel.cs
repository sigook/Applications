namespace Covenant.Common.Models.Request.TimeSheet
{
    public class TimeSheetModel
    {
        public TimeSpan Hours { get; set; }
        public DateTime TimeIn { get; set; }
        public TimeSpan MissingHours { get; set; }
        public TimeSpan MissingHoursOvertime { get; set; }
        public decimal DeductionsOthers { get; set; }
        public decimal BonusOrOthers { get; set; }
        public string DeductionsOthersDescription { get; set; }
        public string BonusOrOthersDescription { get; set; }
        public string Comments { get; set; }
        public decimal MissingRateWorker { get; set; }
        public decimal MissingRateAgency { get; set; }
        public decimal Reimbursements { get; set; }
        public string ReimbursementsDescription { get; set; }
    }
}