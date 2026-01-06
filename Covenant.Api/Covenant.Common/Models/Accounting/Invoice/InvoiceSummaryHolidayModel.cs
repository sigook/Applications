using Covenant.Common.Utils.Extensions;

namespace Covenant.Common.Models.Accounting.Invoice;

public class InvoiceSummaryHolidayModel
{
    public InvoiceSummaryHolidayModel()
    {
    }

    public InvoiceSummaryHolidayModel(decimal amount, double hours, string description)
    {
        Amount = amount;
        Hours = hours;
        Description = description;
    }

    public decimal Amount { get; set; }
    public double Hours { get; set; }
    public string Description { get; set; }
    public decimal UnitPrice => Hours <= 0 ? decimal.Zero : (Amount / (decimal)Hours).DefaultMoneyRound();
}