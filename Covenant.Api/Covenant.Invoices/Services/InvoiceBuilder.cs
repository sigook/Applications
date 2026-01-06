using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Invoices.Services
{
    public class InvoiceBuilder :
        IInvoiceNumberHolder,
        ICompanyProfileIdHolder,
        IInvoiceWeekEnding,
        IInvoiceTotalsHolder,
        IInvoiceHolidaysHolder,
        IInvoiceAdditionalItemsHolder,
        IInvoiceDiscountsHolder,
        IInvoiceBuilder,
        IInvoiceEmailHolder,
        IInvoiceDate
    {
        private long _invoiceNumber;
        private Guid _companyProfileId;
        private IReadOnlyCollection<InvoiceTotal> _invoiceTotals = new List<InvoiceTotal>();
        private IReadOnlyCollection<InvoiceSummaryItemModel> _invoiceSummaryItemModels = new List<InvoiceSummaryItemModel>();
        private IReadOnlyCollection<InvoiceHoliday> _invoiceHolidays = new List<InvoiceHoliday>();
        private List<InvoiceAdditionalItem> _additionalItems = new List<InvoiceAdditionalItem>();
        private List<InvoiceDiscount> _discounts = new List<InvoiceDiscount>();
        private readonly Rates _rates;
        private string _email;
        private DateTime _invoiceDate;
        private DateTime? _weekEnding;

        private InvoiceBuilder(Rates rates) => _rates = rates;
        public static IInvoiceNumberHolder Invoice(Rates rates) => new InvoiceBuilder(rates);

        public IInvoiceDate WithInvoiceNumber(long invoiceNumber)
        {
            _invoiceNumber = invoiceNumber;
            return this;
        }

        public IInvoiceWeekEnding WithInvoiceDate(DateTime? invoiceDate)
        {
            _invoiceDate = invoiceDate + DateTime.Now.TimeOfDay ?? DateTime.Now;
            return this;
        }

        public ICompanyProfileIdHolder WithWeekEnding(DateTime? weekEnding)
        {
            _weekEnding = weekEnding;
            return this;
        }

        public IInvoiceEmailHolder WithCompanyProfileId(Guid companyProfileId)
        {
            _companyProfileId = companyProfileId;
            return this;
        }

        public IInvoiceTotalsHolder WithEmail(string email)
        {
            _email = string.IsNullOrEmpty(email) ? null : email;
            return this;
        }

        public IInvoiceHolidaysHolder WithInvoiceTotals(IReadOnlyCollection<InvoiceTotal> invoiceTotals, IReadOnlyCollection<InvoiceSummaryItemModel> invoiceSummaryItemModels)
        {
            _invoiceTotals = invoiceTotals;
            _invoiceSummaryItemModels = invoiceSummaryItemModels;
            return this;
        }

        public IInvoiceHolidaysHolder WithoutInvoiceTotals() => this;

        public IInvoiceAdditionalItemsHolder WithInvoiceHolidays(IReadOnlyCollection<InvoiceHoliday> invoiceHolidays)
        {
            _invoiceHolidays = invoiceHolidays;
            return this;
        }

        public IInvoiceAdditionalItemsHolder WithoutInvoiceHolidays() => this;

        public IInvoiceDiscountsHolder WithAdditionalItems(List<InvoiceAdditionalItem> additionalItems)
        {
            _additionalItems = additionalItems;
            return this;
        }

        public IInvoiceDiscountsHolder WithoutAdditionalItems() => this;

        public IInvoiceBuilder WithDiscounts(List<InvoiceDiscount> discounts)
        {
            _discounts = discounts;
            return this;
        }

        public IInvoiceBuilder WithoutDiscounts() => this;

        public Result<Invoice> Build()
        {
            decimal totalDiscounts = _discounts.Sum(d => d.Amount);
            decimal totalAdditionalItems = _additionalItems.Sum(i => i.Total);
            decimal totalInvoiceHolidays = _invoiceHolidays.Sum(h => h.Amount);
            decimal invoiceTotalNet = _invoiceSummaryItemModels.Sum(i => i.Total) + totalAdditionalItems + totalInvoiceHolidays;
            if (totalDiscounts > invoiceTotalNet) return Result.Fail<Invoice>($"The discounts({totalDiscounts:C}) must be less than the total Invoice({invoiceTotalNet:C})");
            decimal subTotal = decimal.Subtract(invoiceTotalNet, totalDiscounts);
            decimal hst = decimal.Multiply(subTotal, _rates.Hst);
            decimal totalNet = subTotal.Add(hst);

            var invoice = new Invoice(
                _companyProfileId, 
                _invoiceNumber,
                _rates.NightShift,
                _rates.Holiday,
                _rates.OverTime,
                _rates.Vacations,
                _rates.Hst,
                _rates.Bonus,
                subTotal.DefaultMoneyRound(), 
                hst.DefaultMoneyRound(), 
                totalNet.DefaultMoneyRound())
            {
                Email = _email,
                WeekEnding = _weekEnding,
                CreatedAt = _invoiceDate
            };

            invoice.AddInvoiceTotals(_invoiceTotals);
            invoice.AddAdditionalItems(_additionalItems);
            invoice.AddHolidays(_invoiceHolidays);
            invoice.AddDiscounts(_discounts);
            return Result.Ok(invoice);
        }
    }


    public interface IInvoiceNumberHolder
    {
        IInvoiceDate WithInvoiceNumber(long invoiceNumber);
    }

    public interface IInvoiceDate
    {
        IInvoiceWeekEnding WithInvoiceDate(DateTime? modelInvoiceDate);
    }

    public interface IInvoiceWeekEnding
    {
        ICompanyProfileIdHolder WithWeekEnding(DateTime? weekEnding);
    }

    public interface ICompanyProfileIdHolder
    {
        IInvoiceEmailHolder WithCompanyProfileId(Guid companyProfileId);
    }

    public interface IInvoiceEmailHolder
    {
        IInvoiceTotalsHolder WithEmail(string email);
    }

    public interface IInvoiceTotalsHolder
    {
        IInvoiceHolidaysHolder WithInvoiceTotals(IReadOnlyCollection<InvoiceTotal> invoiceTotals, IReadOnlyCollection<InvoiceSummaryItemModel> invoiceSummaryItemModels);
        IInvoiceHolidaysHolder WithoutInvoiceTotals();
    }

    public interface IInvoiceHolidaysHolder
    {
        IInvoiceAdditionalItemsHolder WithInvoiceHolidays(IReadOnlyCollection<InvoiceHoliday> invoiceHolidays);
        IInvoiceAdditionalItemsHolder WithoutInvoiceHolidays();
    }

    public interface IInvoiceAdditionalItemsHolder
    {
        IInvoiceDiscountsHolder WithAdditionalItems(List<InvoiceAdditionalItem> additionalItems);
        IInvoiceDiscountsHolder WithoutAdditionalItems();
    }

    public interface IInvoiceDiscountsHolder
    {
        IInvoiceBuilder WithDiscounts(List<InvoiceDiscount> discounts);
        IInvoiceBuilder WithoutDiscounts();
    }

    public interface IInvoiceBuilder
    {
        Result<Invoice> Build();
    }
}