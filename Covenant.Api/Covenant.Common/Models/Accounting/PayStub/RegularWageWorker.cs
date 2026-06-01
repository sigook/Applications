using Covenant.Common.Utils.Extensions;

namespace Covenant.Common.Models.Accounting.PayStub;

public class RegularWageWorker
{
    public decimal RegularWage { get; set; }
    public bool HolidayWasPaid { get; set; }
    public decimal CustomPublicHolidayValue { get; set; }
    public bool IsEntitledToReceiveHolidayPay { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }

    public void CalculateAmount()
    {
        if (HolidayWasPaid)
        {
            Amount = decimal.Zero;
            Description = "The holiday was already paid";
        }
        else if (CustomPublicHolidayValue > decimal.Zero)
        {
            Amount = CustomPublicHolidayValue;
            Description = $"The amount to pay is a custom value ({CustomPublicHolidayValue})";
        }
        else if (!IsEntitledToReceiveHolidayPay)
        {
            Amount = decimal.Zero;
            Description = "The worker is not entitled to receive the public holiday because he did not work the required days";
        }
        else
        {
            Amount = (RegularWage / 20).DefaultMoneyRound();
            Description = $"The amount was calculated using the regular formula: (gross earnings plus vacation pay ({RegularWage}) of the last four weeks / 20)";
        }
    }
}