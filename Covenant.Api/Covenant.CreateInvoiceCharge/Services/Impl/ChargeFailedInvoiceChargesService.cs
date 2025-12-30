using Covenant.Billing.Entities;
using Covenant.Billing.Models;
using Covenant.Billing.Repositories;
using Covenant.Common.Args;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Payment;

namespace Covenant.CreateInvoiceCharge.Services.Impl
{
    public class ChargeFailedInvoiceChargesService : IChargeFailedInvoiceChargesService
    {
        public event Func<object, InvoiceChargeSucceededEventArgs, Task> OnInvoiceChargeSucceeded;
        public event Func<object, InvoiceChargeFailedEventArgs, Task> OnInvoiceChargeFailed;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IBamboraService _chargeService;
        private readonly ITimeService _timeService;

        public ChargeFailedInvoiceChargesService(IInvoiceRepository invoiceRepository, IBamboraService chargeService, ITimeService timeService)
        {
            _invoiceRepository = invoiceRepository;
            _chargeService = chargeService;
            _timeService = timeService;
        }
        public async Task AttemptToChargeFailedInvoiceCharges()
        {
            DateTime now = _timeService.GetCurrentDateTime();
            const int maxAttempts = 5;
            var charges = await _invoiceRepository.GetFailedInvoiceCharges(now.Subtract(TimeSpan.FromHours(24)), maxAttempts);
            foreach (InvoiceChargeModel chargeModel in charges)
            {
                InvoiceCharge charge = await _invoiceRepository.GetInvoiceChargeById(chargeModel.Id);
                decimal amount = charge.Amount;
                var result = await _chargeService.CreateCharge(chargeModel.CompanyId, new ChargeOptions(amount, "Invoice Charge"));
                if (result) charge.ChangeChargeToSucceeded(result.Value.TransactionId, now);
                else charge.AddFailedAttempt(now, result.Errors.First().Message);

                await _invoiceRepository.UpdateInvoiceCharge(charge);
                await _invoiceRepository.SaveChangesAsync();

                if (result) OnInvoiceChargeSucceeded?.Invoke(this, new InvoiceChargeSucceededEventArgs(chargeModel.InvoiceNumberId, amount, chargeModel.CompanyId, chargeModel.CompanyEmail));
                else OnInvoiceChargeFailed?.Invoke(this, new InvoiceChargeFailedEventArgs(chargeModel.InvoiceNumberId, amount, chargeModel.CompanyId, chargeModel.CompanyEmail));
            }
        }
    }
}