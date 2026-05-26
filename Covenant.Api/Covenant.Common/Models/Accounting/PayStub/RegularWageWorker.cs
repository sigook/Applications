namespace Covenant.Common.Models.Accounting.PayStub;

public class RegularWageWorker
{
    public decimal RegularWage { get; set; }
    public bool HolidayWasPaid { get; set; }
    public decimal CustomPublicHolidayValue { get; set; }
    public bool IsEntitledToReceiveHolidayPay { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}