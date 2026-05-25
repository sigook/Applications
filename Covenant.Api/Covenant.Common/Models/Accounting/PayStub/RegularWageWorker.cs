namespace Covenant.Common.Models.Accounting.PayStub;

/// <summary>
/// Inputs for public holiday pay: the four-week earnings base plus the entitlement flags.
/// The amount is resolved by ITimesheetCalculatorService.ResolveHolidayPay.
/// </summary>
public class RegularWageWorker
{
    public decimal RegularWage { get; set; }
    public bool HolidayWasPaid { get; set; }
    public decimal CustomPublicHolidayValue { get; set; }
    public bool IsEntitledToReceiveHolidayPay { get; set; }
}