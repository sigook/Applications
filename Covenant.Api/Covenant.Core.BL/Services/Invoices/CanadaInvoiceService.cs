using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services.Invoices;

public class CanadaInvoiceService(
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
    ITimesheetCalculatorService calculatorService) : BaseInvoiceService(timeSheetRepository, invoiceRepository, agencyRepository, companyRepository, locationRepository, catalogRepository, timeService, rates, subcontractorRepository, timeLimits, calculatorService)
{
    public override async Task<Result<InvoicePreviewModel>> PreviewAsync(IEnumerable<Guid> agencyIds, CreateInvoiceModel model)
    {
        var result = await CreateInvoiceInternal(agencyIds, model);
        if (!result) return Result.Fail<InvoicePreviewModel>(result.Errors);

        var invoice = result.Value;

        // Combine timesheet items, additional items, and holidays for preview
        var timesheetItems = ConsolidatePreviewItems(invoice.InvoiceTotals);
        var additionalItems = ConsolidatePreviewAdditionalItems(invoice.AdditionalItems);
        var holidayItems = ConsolidatePreviewHolidays(invoice.Holidays);
        var allItems = timesheetItems.Concat(additionalItems).Concat(holidayItems).ToList();

        var preview = new InvoicePreviewModel
        {
            SubTotal = invoice.SubTotal,
            Hst = invoice.Hst,
            Total = invoice.TotalNet,
            Items = allItems,
            Discounts = ConsolidatePreviewDiscounts(invoice.Discounts)
        };

        return Result.Ok(preview);
    }

    public override async Task<Result<Guid>> CreateAsync(IEnumerable<Guid> agencyIds, CreateInvoiceModel model)
    {
        var result = await CreateInvoiceInternal(agencyIds, model);
        if (!result) return Result.Fail<Guid>(result.Errors);

        var invoice = result.Value;

        // Save invoice
        await invoiceRepository.Create(invoice);
        await invoiceRepository.SaveChangesAsync();

        // Create subcontractor reports for Canadian invoices
        await CreateSubcontractorReportsAsync(agencyIds, model.CompanyId);

        return Result.Ok(invoice.Id);
    }

    private async Task<Result<Invoice>> CreateInvoiceInternal(IEnumerable<Guid> agencyIds, CreateInvoiceModel model)
    {
        // 1. Get timesheets
        var timesheets = await timeSheetRepository.GetTimeSheetForCreatingInvoice(agencyIds, model);
        if (timesheets.Count == 0 && !model.DirectHiring)
        {
            return Result.Fail<Invoice>("No approved timesheets found for the selected period");
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


        // 3. Process timesheets and build invoice totals with TimeSheetTotal entities
        // EF Core will cascade insert TimeSheetTotal entities when the invoice is saved
        var invoiceTotals = new List<InvoiceTotal>();
        if (!model.DirectHiring)
        {
            invoiceTotals.AddRange(ProcessTimesheets<InvoiceTotal>(timesheets, holidays));
        }

        // 3.1. Get paid holidays (workers who don't work on holidays but get paid)

        var invoiceHolidays = new List<InvoiceHoliday>();
        if (!model.DirectHiring)
        {
            await GetInvoiceHolidaysAsync(timesheets, holidays, model.CompanyProfileId);
        }

        // 4. Prepare additional items
        var additionalItems = new List<InvoiceAdditionalItem>();
        if (model.AdditionalItems != null && model.AdditionalItems.Any())
        {
            additionalItems = model.AdditionalItems
                .Select(a => new InvoiceAdditionalItem(
                    quantity: a.Quantity,
                    unitPrice: a.UnitPrice,
                    description: a.Description))
                .ToList();
        }

        // 4.1. Prepare discounts
        var discounts = new List<InvoiceDiscount>();
        if (model.Discounts != null && model.Discounts.Any())
        {
            discounts = model.Discounts
                .Select(d => new InvoiceDiscount(
                    quantity: d.Quantity,
                    unitPrice: d.UnitPrice,
                    description: d.Description))
                .ToList();
        }

        // 5. Calculate subtotal including additional items, holidays, and discounts
        var timesheetsSubtotal = invoiceTotals.Sum(t => t.TotalGross);
        var additionalItemsSubtotal = additionalItems.Sum(a => a.Total);
        var holidaysSubtotal = invoiceHolidays.Sum(h => h.Amount);
        var discountsSubtotal = discounts.Sum(d => d.Amount);
        var subtotal = timesheetsSubtotal + additionalItemsSubtotal + holidaysSubtotal - discountsSubtotal;

        // 6. Calculate HST (Canadian tax) on full subtotal
        var hst = subtotal * rates.Hst;
        var totalNet = subtotal + hst;

        // 7. Get next invoice number
        var nextInvoiceNumber = await invoiceRepository.GetNextInvoiceNumber();

        // 8. Create invoice
        var invoice = new Invoice
        {
            CompanyId = model.CompanyProfileId,
            InvoiceNumber = nextInvoiceNumber.NextNumber,
            NightShiftRate = 0, // Not using night shift
            HolidayRate = rates.Holiday,
            OverTimeRate = rates.OverTime,
            VacationsRate = rates.Vacations,
            HstRate = rates.Hst,
            BonusRate = rates.Bonus,
            SubTotal = subtotal,
            Hst = hst,
            TotalNet = totalNet,
            Email = model.Email,
            CreatedAt = model.InvoiceDate ?? timeService.GetCurrentDateTime()
        };
        if (model.DirectHiring)
        {
            invoice.WeekEnding = null;
        }
        else
        {
            invoice.WeekEnding = timesheets.Count != 0 ? timesheets.Max(t => t.Date).GetWeekEndingCurrentWeek() : null;
        }

        // 9. Add invoice totals
        invoice.AddInvoiceTotals(invoiceTotals);

        // 10. Add paid holidays
        if (invoiceHolidays.Count != 0)
        {
            invoice.AddHolidays(invoiceHolidays);
        }

        // 11. Add additional items
        if (additionalItems.Count != 0)
        {
            invoice.AddAdditionalItems(additionalItems);
        }

        // 12. Add discounts
        if (discounts.Count != 0)
        {
            invoice.AddDiscounts(discounts);
        }

        // 13. Add additional detail (client's site address)
        if (!string.IsNullOrWhiteSpace(model.ClientSiteAddress))
        {
            invoice.AdditionalDetail = new InvoiceAdditionalDetail
            {
                ClientSiteAddress = model.ClientSiteAddress,
                CanadaInvoiceId = invoice.Id
            };
        }

        return Result.Ok(invoice);
    }

    /// <summary>
    /// Gets paid holidays for workers who are entitled to holiday pay
    /// This is for workers who DON'T work on holidays but still get paid (labor benefit)
    /// </summary>
    private async Task<List<InvoiceHoliday>> GetInvoiceHolidaysAsync(
        IEnumerable<TimeSheetApprovedBillingModel> timesheets,
        List<DateTime> holidays,
        Guid companyProfileId)
    {
        var invoiceHolidays = new List<InvoiceHoliday>();

        // Check if paid holidays are enabled
        var paidHolidays = timesheets.FirstOrDefault()?.PaidHolidays ?? false;
        if (!paidHolidays || !holidays.Any())
        {
            return invoiceHolidays;
        }

        // For each holiday in the period, get workers entitled to holiday pay
        foreach (var holiday in holidays)
        {
            // 4-week look-back window before the holiday's work week (ESA public holiday pay rule).
            var holidayWeekEnd = holiday.GetEnd();          // last Saturday of the week before the holiday's week
            var lookbackStart = holidayWeekEnd.GetStart();  // four work weeks earlier
            var qualifyingDays = holiday.GetRangeOfDaysWorkerMustWorkToReceiveHolidayPay();
            var workersCharges = await invoiceRepository.GetCompanyRegularCharges(companyProfileId, lookbackStart, holidayWeekEnd, qualifyingDays);

            foreach (var workerCharge in workersCharges)
            {
                var result = InvoiceHoliday.Create(
                    holiday,
                    workerCharge.HoursToPay,
                    workerCharge.AmountToPay,
                    workerCharge.WorkerProfileId);

                if (result)
                {
                    invoiceHolidays.Add(result.Value);
                }
            }
        }

        return invoiceHolidays;
    }

    private List<InvoiceSummaryItemModel> ConsolidatePreviewDiscounts(IEnumerable<InvoiceDiscount> discounts)
    {
        return discounts
            .GroupBy(d => d.Description)
            .Select(group =>
            {
                var totalQty = group.Sum(d => d.Quantity);
                var totalAmount = group.Sum(d => d.Amount);
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

    private List<InvoiceSummaryItemModel> ConsolidatePreviewHolidays(IEnumerable<InvoiceHoliday> holidays)
    {
        return holidays
            .GroupBy(h => h.Holiday.Date)
            .Select(group =>
            {
                var totalHours = group.Sum(h => h.Hours);
                var totalAmount = group.Sum(h => h.Amount);
                var unitPrice = totalHours > 0 ? totalAmount / (decimal)totalHours : 0;

                return new InvoiceSummaryItemModel
                {
                    Description = $"Charge for Holiday {group.Key:D}",
                    Quantity = totalHours,
                    UnitPrice = unitPrice,
                    Total = totalAmount
                };
            })
            .ToList();
    }
}
