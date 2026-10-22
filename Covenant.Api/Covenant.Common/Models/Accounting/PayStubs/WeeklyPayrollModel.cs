namespace Covenant.Common.Models.Accounting.PayStubs
{
    public class WeeklyPayrollModel
    {
        public decimal TotalNet { get; set; }
        public DateTime WeekEnding { get; set; }
        public int NumberOfPayStubs { get; set; }
        public string DisplayWeekEnding => WeekEnding.ToString("yyyy-MM-dd");
    }
}