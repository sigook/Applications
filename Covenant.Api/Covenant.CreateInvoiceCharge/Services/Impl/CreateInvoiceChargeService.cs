using Covenant.Billing.Entities;
using Covenant.Billing.Models;
using Covenant.Billing.Repositories;
using Covenant.Common.Args;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Payment;

namespace Covenant.CreateInvoiceCharge.Services.Impl
{
    public class CreateInvoiceChargeService : ICreateInvoiceChargeService
    {
        public event Func<object, InvoiceChargeSucceededEventArgs, Task> OnInvoiceChargeSucceeded;
        public event Func<object, InvoiceChargeFailedEventArgs, Task> OnInvoiceChargeFailed;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IBamboraService _chargeService;
        private readonly ITimeService _timeService;

        public CreateInvoiceChargeService(IInvoiceRepository invoiceRepository, IBamboraService chargeService, ITimeService timeService)
        {
            _invoiceRepository = invoiceRepository;
            _chargeService = chargeService;
            _timeService = timeService;
        }

        public async Task CreatePendingInvoiceCharge()
        {
            DateTime now = _timeService.GetCurrentDateTime();
            List<InvoiceModel> invoices = await _invoiceRepository.GetInvoicesReadyForCharge(now.Subtract(TimeSpan.FromDays(5)));
            foreach (InvoiceModel invoice in invoices)
            {
                decimal amount = invoice.TotalNet;
                var result = await _chargeService.CreateCharge(invoice.CompanyId, new ChargeOptions(amount, "Invoice charge"));
                InvoiceCharge invoiceCharge = result
                    ? InvoiceCharge.CreateSuccessInvoiceCharge(invoice.Id, amount, now, result.Value.TransactionId)
                    : InvoiceCharge.CreateFailInvoiceCharge(invoice.Id, amount, now, result.Errors.First().Message);

                await _invoiceRepository.Create(invoiceCharge);
                await _invoiceRepository.SaveChangesAsync();

                if (result) OnInvoiceChargeSucceeded?.Invoke(this, new InvoiceChargeSucceededEventArgs(invoice.NumberId, amount, invoice.CompanyId, invoice.CompanyEmail));
                else OnInvoiceChargeFailed?.Invoke(this, new InvoiceChargeFailedEventArgs(invoice.NumberId, amount, invoice.CompanyId, invoice.CompanyEmail));
            }
        }
    }
}