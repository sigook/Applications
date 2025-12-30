using Covenant.Billing.Utils;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Covenant.Invoices.Services;

namespace Covenant.Billing.Services.Impl
{
    public class CreateInvoiceWithoutTimeSheet : ICreateInvoiceWithoutTimeSheet
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly Rates _rates;

        public CreateInvoiceWithoutTimeSheet(IInvoiceRepository invoiceRepository, Rates rates)
        {
            _invoiceRepository = invoiceRepository;
            _rates = rates;
        }

        public Task<Result<Invoice>> Preview(Guid companyProfileId, CreateInvoiceModel model) =>
            PrivateCreate(companyProfileId, model, true);

        public Task<Result<Invoice>> Create(Guid companyProfileId, CreateInvoiceModel model) =>
            PrivateCreate(companyProfileId, model);

        private async Task<Result<Invoice>> PrivateCreate(Guid companyProfileId, CreateInvoiceModel model, bool preview = false)
        {
            var weekEnding = model.DirectHiring ? null : (model.InvoiceDate ?? (DateTime?)DateTime.Now);
            if (weekEnding.HasValue)
            {
                weekEnding = weekEnding.Value.GetWeekEndingWeekBefore();
            }
            Result<Invoice> result = InvoiceBuilder
                .Invoice(_rates)
                .WithInvoiceNumber((await _invoiceRepository.GetNextInvoiceNumber()).NextNumber)
                .WithInvoiceDate(model.InvoiceDate)
                .WithWeekEnding(weekEnding)
                .WithCompanyProfileId(companyProfileId)
                .WithEmail(model.Email)
                .WithoutInvoiceTotals()
                .WithoutInvoiceHolidays()
                .WithAdditionalItems(model.AdditionalItems.ToInvoiceAdditionalItems())
                .WithDiscounts(model.Discounts.ToInvoiceDiscounts())
                .Build();
            if (!result || preview) return result;
            await _invoiceRepository.Create(result.Value);
            await _invoiceRepository.SaveChangesAsync();
            return result;
        }
    }
}