namespace Covenant.Common.Models.Worker
{
    public class PayStubHistoryModel
    {
        public int RowNumber { get; set; }
        public Guid Id { get; set; }
        public string PayStubNumber { get; set; }
        public DateTime WeekEnding { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal Vacations { get; set; }
        public decimal PublicHolidays { get; set; }
        public decimal TotalPaid { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public IEnumerable<PayStubItemHistoryModel> Items { get; set; } = new List<PayStubItemHistoryModel>();
        public IEnumerable<string> Companies { get; set; } = new List<string>();
    }
}