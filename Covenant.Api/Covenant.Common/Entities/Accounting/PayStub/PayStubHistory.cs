namespace Covenant.Common.Entities.Accounting.PayStub;

public class PayStubHistory
{
    public int RowNumber { get; set; }
    public Guid Id { get; set; }
    public Guid WorkerProfileId { get; set; }
    public int NumberId { get; set; }
    public string PayStubNumber { get; set; }
    public DateTime WeekEnding { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal Vacations { get; set; }
    public decimal PublicHolidayPay { get; set; }
    public decimal TotalPaid { get; set; }
    public DateTime DateWorkBegins { get; set; }
    public DateTime DateWorkEnd { get; set; }
}
