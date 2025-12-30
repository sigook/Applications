namespace Covenant.Common.Models.Worker
{
    public class WorkerWageHistoryModel
    {
        public long Id { get; set; }
        public double? Year { get; set; }
        public double? Month { get; set; }
        public DateTime? Week { get; set; }
        public string Holidays { get; set; }
        public string Companies { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public decimal? TotalEarnings { get; set; }
        public decimal? TotalPaid { get; set; }
        public decimal? Vacations { get; set; }
        public decimal? PublicHolidays { get; set; }
        public double? RegularHours { get; set; }
        public double? OvertimeHours { get; set; }
        public double? MissingHours { get; set; }
        public double? MissingOvertimeHours { get; set; }
        public double? HolidayPremiumPayHours { get; set; }

        public double TotalHours =>
            RegularHours.GetValueOrDefault() +
            OvertimeHours.GetValueOrDefault() +
            MissingHours.GetValueOrDefault() +
            MissingOvertimeHours.GetValueOrDefault() +
            HolidayPremiumPayHours.GetValueOrDefault();
    }
}