namespace Covenant.Common.Models.Accounting.PayStubs;

public class PayStubHistoryModel
{
    public Guid Id { get; set; }
    public long NumberId { get; set; }
    public string PayStubNumber { get; set; }
    public DateTime WeekEnding { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal Vacations { get; set; }
    public decimal TotalPaid { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public double AccumulatedQuantity { get; set; }
    public decimal AccumulatedTotal { get; set; }
    public decimal AccumulatedVacations { get; set; }
    public decimal AccumulatedTotalEarnings { get; set; }
    public decimal AccumulatedTotalPaid { get; set; }
    public IEnumerable<PayStubItemHistoryModel> Items { get; set; } = new List<PayStubItemHistoryModel>();
    public IEnumerable<string> Companies { get; set; } = new List<string>();
}
