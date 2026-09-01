using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace Covenant.Core.BL.Services.Accounting.Invoices;

public class UsaInvoiceService(
    ITimesheetRepository timeSheetRepository,
    IInvoiceRepository invoiceRepository,
    IAgencyRepository agencyRepository,
    ICompanyRepository companyRepository,
    ILocationRepository locationRepository,
    ICatalogRepository catalogRepository,
    ITimeService timeService,
    Rates rates,
    ISubcontractorRepository subcontractorRepository,
    TimeLimits timeLimits,
    ITimesheetCalculatorService calculatorService,
    IIdentityServerService identityServerService,
    IInvoicesContainer invoicesContainer,
    IRazorViewToStringRenderer renderer,
    IPdfGeneratorService pdfGenerator,
    IEmailService emailService,
    IMediator mediator,
    IPayStubsContainer payStubsContainer,
    ITeamsService teamsService,
    IOptions<TeamsWebhookConfiguration> teamsOptions,
    IInvoiceDocumentAdapter invoiceDocumentAdapter) : InvoiceService(timeSheetRepository, invoiceRepository, agencyRepository, companyRepository, locationRepository, catalogRepository, timeService, rates, subcontractorRepository, timeLimits, calculatorService, identityServerService, invoicesContainer, renderer, pdfGenerator, emailService, mediator, payStubsContainer, teamsService, teamsOptions, invoiceDocumentAdapter)
{
    protected override Task<InvoiceListModelWithTotals> FetchInvoices(IEnumerable<Guid> agencyIds, GetInvoicesFilter filter)
        => invoiceRepository.GetInvoicesUSAForAgency(agencyIds, filter);

    protected override Task<List<InvoiceListModel>> FetchInvoicesForExport(IEnumerable<Guid> agencyIds, GetInvoicesFilter filter)
        => invoiceRepository.GetAllInvoicesUSAForAgency(agencyIds, filter);

    protected override Task<InvoiceSummaryModel> FetchInvoiceSummary(Guid invoiceId)
        => invoiceRepository.GetInvoiceUSASummaryById(invoiceId);

    protected override async Task<(Guid InvoiceId, string InvoiceNumber, IReadOnlyList<string> PayStubsDeleted)> DeleteInvoiceData(Guid invoiceId, DeleteInvoiceModel model)
    {
        var (deletedId, invoiceNumber) = await invoiceRepository.DeleteInvoiceUSA(invoiceId);
        return (deletedId, invoiceNumber, []);
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
        await CreateSubcontractorReportsAsync(agencyIds, model.CompanyProfileId);

        return Result.Ok(invoice.Id);
    }

    private async Task<Result<InvoiceUSA>> CreateInvoiceInternal(IEnumerable<Guid> agencyIds, CreateInvoiceModel model)
    {
        // 1. Get timesheets
        var timesheets = await timeSheetRepository.GetTimeSheetForCreatingInvoice(agencyIds, model);
        if (timesheets.Count == 0 && !model.DirectHiring)
        {
            return Result.Fail<InvoiceUSA>("No approved timesheets found for the selected period");
        }

        // 2. Get holidays for the period
        var holidays = new List<DateTime>();
        if (timesheets.Count != 0)
        {
            var from = timesheets.Min(ts => ts.Date);
            var to = timesheets.Max(ts => ts.Date);
            var countryCode = timesheets.FirstOrDefault().CountryCode;
            var holidaysData = await GetHolidaysForPeriod(from, to, countryCode);
            holidays.AddRange(holidaysData);
        }

        // 3. Process timesheets and build invoice items with TimeSheetTotal entities
        var usaItems = new List<InvoiceUSAItem>();
        var timesheetsTax = 0m;
        if (!model.DirectHiring)
        {
            usaItems.AddRange(ProcessTimesheets<InvoiceUSAItem>(timesheets, holidays, out timesheetsTax));
        }

        // 4. Add additional items and discounts
        var additionalItems = model.AdditionalItems.Select(s => new InvoiceUSAItem(s.Quantity, s.UnitPrice, s.Description));
        var discounts = model.Discounts.Select(s => new InvoiceUSADiscount(s.Quantity, s.UnitPrice, s.Description)).ToList();
        var allItems = usaItems.Concat(additionalItems).ToList();

        // 5. Get next invoice number
        var nextNumber = await invoiceRepository.GetNextInvoiceUSANumber();

        // 6. Compute totals
        var invoiceDate = model.InvoiceDate ?? timeService.GetCurrentDateTime();
        var subTotal = allItems.Sum(s => s.Total) - discounts.Sum(s => s.Total);

        // 7. Compute tax
        decimal tax;
        if (model.DirectHiring)
        {
            var directHiringRate = (model.TaxPercentage ?? 0m) / 100m;
            tax = subTotal * directHiringRate;
        }
        else
        {
            tax = timesheetsTax;
        }

        var totalNet = subTotal + tax;
        var invoiceNumber = $"{InvoiceUSA.PrefixInvoiceNumber}-{nextNumber.NextNumber:0000}-{invoiceDate:yy}";

        // 8. Create invoice
        var invoice = new InvoiceUSA
        {
            InvoiceNumberId = nextNumber.NextNumber,
            CreatedAt = invoiceDate,
            CompanyProfileId = model.CompanyProfileId,
            Items = allItems,
            Discounts = discounts,
            SubTotal = subTotal,
            Tax = tax,
            TotalNet = totalNet,
            InvoiceNumber = invoiceNumber
        };

        // 9. Set week ending date
        if (model.DirectHiring)
        {
            invoice.WeekEnding = null;
        }
        else
        {
            invoice.WeekEnding = timesheets.Count != 0 ? timesheets.Max(t => t.Date).GetWeekEndingCurrentWeek() : null;
        }
        invoice.BillToEmail = model.Email;

        // 10. Set billing addresses
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

        // 11. Add additional detail (client's site address)
        if (!string.IsNullOrWhiteSpace(model.ClientSiteAddress))
        {
            invoice.AdditionalDetail = new InvoiceAdditionalDetail
            {
                ClientSiteAddress = model.ClientSiteAddress,
                UsaInvoiceId = invoice.Id
            };
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
