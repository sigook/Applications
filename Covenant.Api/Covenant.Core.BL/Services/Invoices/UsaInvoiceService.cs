using Covenant.Billing.Utils;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;

namespace Covenant.Core.BL.Services.Invoices;

public class UsaInvoiceService : BaseInvoiceService
{
    public UsaInvoiceService(
        ITimeSheetRepository timeSheetRepository,
        IInvoiceRepository invoiceRepository,
        IAgencyRepository agencyRepository,
        ICompanyRepository companyRepository,
        ILocationRepository locationRepository,
        ICatalogRepository catalogRepository,
        ITimeService timeService,
        Rates rates,
        ISubcontractorRepository subcontractorRepository,
        TimeLimits timeLimits)
        : base(timeSheetRepository, invoiceRepository, agencyRepository, companyRepository, locationRepository, catalogRepository, timeService, rates, subcontractorRepository, timeLimits)
    {
    }

    public override async Task<Result<InvoicePreviewModel>> PreviewAsync(IEnumerable<Guid> agencyIds, CreateInvoiceModel model)
    {
        var result = await CreateInvoiceInternal(agencyIds, model);
        if (!result) return Result.Fail<InvoicePreviewModel>(result.Errors);

        var invoice = result.Value;

        var preview = new InvoicePreviewModel
        {
            SubTotal = invoice.SubTotal,
            Hst = invoice.Tax,
            Total = invoice.TotalNet,
            Items = ConsolidatePreviewItems(invoice.Items),
            Discounts = ConsolidatePreviewDiscounts(invoice.Discounts)
        };

        return Result.Ok(preview);
    }

    public override async Task<Result<Guid>> CreateAsync(IEnumerable<Guid> agencyIds, CreateInvoiceModel model)
    {
        var result = await CreateInvoiceInternal(agencyIds, model);
        if (!result) return Result.Fail<Guid>(result.Errors);

        var invoice = result.Value;

        await invoiceRepository.Create(invoice);
        await invoiceRepository.SaveChangesAsync();

        // Create subcontractor reports for USA invoices
        await CreateSubcontractorReportsAsync(agencyIds, model.CompanyId);

        return Result.Ok(invoice.Id);
    }

    private async Task<Result<InvoiceUSA>> CreateInvoiceInternal(IEnumerable<Guid> agencyIds, CreateInvoiceModel model)
    {
        // 1. Get timesheets
        var timesheets = await timeSheetRepository.GetTimeSheetForCreatingInvoice(agencyIds, model);
        if (!timesheets.Any() && !model.DirectHiring)
        {
            return Result.Fail<InvoiceUSA>("No approved timesheets found for the selected period");
        }

        // 2. Get holidays for the period
        var from = timesheets.Min(ts => ts.Date);
        var to = timesheets.Max(ts => ts.Date);
        var holidays = await GetHolidaysForPeriod(from, to);

        // 3. Process timesheets and build invoice items with TimeSheetTotal entities
        // EF Core will cascade insert TimeSheetTotal entities when the invoice is saved
        var usaItems = ProcessTimesheets<InvoiceUSAItem>(timesheets, holidays);

        // 4. Get sales tax
        var salesTax = model.ProvinceId.HasValue
            ? await locationRepository.GetProvinceSalesTax(model.ProvinceId.Value)
            : null;

        // 5. Add additional items and discounts
        var additionalItems = model.AdditionalItems.ToInvoiceUSAAdditionalItems();
        var discounts = model.Discounts.ToInvoiceUSADiscounts();

        // 6. Get next invoice number
        var nextNumber = await invoiceRepository.GetNextInvoiceUSANumber();

        // 7. Create invoice
        var invoiceDate = model.InvoiceDate ?? timeService.GetCurrentDateTime();
        var invoiceResult = InvoiceUSA.Create(
            nextNumber.NextNumber,
            invoiceDate,
            model.CompanyProfileId,
            usaItems.Concat(additionalItems),
            discounts,
            salesTax);

        if (!invoiceResult) return invoiceResult;

        var invoice = invoiceResult.Value;

        // 8. Set week ending date
        invoice.WeekEnding = timesheets.Any() ? timesheets.Max(t => t.Date).GetWeekEndingCurrentWeek() : null;
        invoice.BillToEmail = model.Email;

        // 9. Set billing addresses
        var agencyId = agencyIds.First();
        var agency = await agencyRepository.GetAgency(agencyId);
        if (agency != null)
        {
            invoice.BillFromAddress = agency.BillingAddress?.FormattedAddress;
            invoice.BillFromPhone = agency.FormattedPhone;
        }

        var company = await companyRepository.GetCompanyProfile(cp => cp.Id == model.CompanyProfileId);
        if (company != null)
        {
            if (string.IsNullOrEmpty(invoice.BillToEmail))
                invoice.BillToEmail = company.Company?.Email;
            invoice.BillToAddress = company.Locations.FirstOrDefault(f => f.IsBilling)?.Location?.FormattedAddress;
            invoice.BillToPhone = company.FormattedPhone;
            invoice.BillToFax = company.FormattedFax;
        }

        return Result.Ok(invoice);
    }

    private List<InvoiceSummaryItemModel> ConsolidatePreviewDiscounts(IEnumerable<InvoiceUSADiscount> discounts)
    {
        return discounts
            .GroupBy(d => d.Description)
            .Select(group =>
            {
                var totalQty = group.Sum(d => d.Quantity);
                var totalAmount = group.Sum(d => d.Total);
                var unitPrice = totalQty > 0 ? totalAmount / (decimal)totalQty : 0;

                return new InvoiceSummaryItemModel
                {
                    Description = group.Key,
                    Quantity = totalQty,
                    UnitPrice = unitPrice,
                    Total = totalAmount
                };
            })
            .ToList();
    }
}
