using Covenant.Common.Entities.Worker;
using Covenant.Common.Functionals;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Common.Entities.Accounting.Invoice;

public class InvoiceHoliday
{
    private InvoiceHoliday()
    {
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Holiday { get; private set; }
    public double Hours { get; private set; }
    public decimal Amount { get; private set; }
    public Guid? WorkerProfileId { get; private set; }
    public WorkerProfile WorkerProfile { get; private set; }
    public Guid InvoiceId { get; private set; }
    public Invoice Invoice { get; private set; }

    public decimal UnitPrice => Hours <= 0 ? decimal.Zero : (Amount / (decimal)Hours).DefaultMoneyRound();

    public static Result<InvoiceHoliday> Create(DateTime holiday, double hours, decimal amount, Guid? workerProfileId = null)
    {
        if (hours <= 0) return Result.Fail<InvoiceHoliday>("Hours must be greater than 0");
        if (amount <= 0) return Result.Fail<InvoiceHoliday>("Amount must be greater than 0");
        var invoiceHoliday = new InvoiceHoliday
        {
            Holiday = holiday,
            Hours = Math.Round(hours, 2),
            WorkerProfileId = workerProfileId
        };
        var unitPrice = invoiceHoliday.Hours <= 0 ? decimal.Zero : (amount / (decimal)hours).DefaultMoneyRound();
        invoiceHoliday.Amount = Math.Round(unitPrice * (decimal)invoiceHoliday.Hours, 2);
        return Result.Ok(invoiceHoliday);
    }

    public void AssignTo(Invoice invoice)
    {
        Invoice = invoice ?? throw new ArgumentNullException();
        InvoiceId = invoice.Id;
    }
}