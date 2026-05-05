using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using TimeSheetTotalEntity = Covenant.Common.Entities.Request.TimeSheetTotal;

namespace Covenant.Core.BL.Services.Invoices;

public abstract class BaseInvoiceService(
    ITimeSheetRepository timeSheetRepository,
    IInvoiceRepository invoiceRepository,
    IAgencyRepository agencyRepository,
    ICompanyRepository companyRepository,
    ILocationRepository locationRepository,
    ICatalogRepository catalogRepository,
    ITimeService timeService,
    Rates rates,
    ISubcontractorRepository subcontractorRepository,
    TimeLimits timeLimits,
    ITimesheetCalculatorService calculatorService)
{
    protected readonly ITimeSheetRepository timeSheetRepository = timeSheetRepository;
    protected readonly IInvoiceRepository invoiceRepository = invoiceRepository;
    protected readonly IAgencyRepository agencyRepository = agencyRepository;
    protected readonly ICompanyRepository companyRepository = companyRepository;
    protected readonly ILocationRepository locationRepository = locationRepository;
    protected readonly ICatalogRepository catalogRepository = catalogRepository;
    protected readonly ITimeService timeService = timeService;
    protected readonly Rates rates = rates;
    protected readonly ISubcontractorRepository subcontractorRepository = subcontractorRepository;
    protected readonly TimeLimits timeLimits = timeLimits;
    protected readonly ITimesheetCalculatorService calculatorService = calculatorService;

    // Abstract methods to be implemented by country-specific services
    public abstract Task<Result<InvoicePreviewModel>> PreviewAsync(IEnumerable<Guid> agencyIds, CreateInvoiceModel model);
    public abstract Task<Result<Guid>> CreateAsync(IEnumerable<Guid> agencyIds, CreateInvoiceModel model);

    #region Helper Methods

    /// <summary>
    /// Checks if a date is a public holiday
    /// </summary>
    protected bool IsPublicHoliday(DateTime date, List<DateTime> holidays)
    {
        return holidays.Any(h => h.Date == date.Date);
    }

    /// <summary>
    /// Calculates hours breakdown for a single timesheet item.
    /// Delegates to ITimesheetCalculatorService with breakIsPaid=false, holidayIsPaid=true
    /// (invoices always subtract break and always pay holidays).
    /// </summary>
    protected TimeSheetHoursBreakdown CalculateHoursForItem(
        ITimeSheetHoursCalculable item,
        ref TimeSpan accumulatedRegularHours,
        bool isHoliday)
    {
        if (!item.TimeOutApproved.HasValue || !item.TimeInApproved.HasValue)
        {
            return new TimeSheetHoursBreakdown();
        }

        return calculatorService.CalculateHoursBreakdown(
            item.TimeInApproved.Value,
            item.TimeOutApproved.Value,
            item.DurationBreak,
            breakIsPaid: false,
            isHoliday: isHoliday,
            holidayIsPaid: true,
            ref accumulatedRegularHours,
            item.OvertimeStartsAfter,
            timeLimits.MaxHoursWeek);
    }

    /// <summary>
    /// Gets holidays for a given period
    /// </summary>
    protected async Task<List<DateTime>> GetHolidaysForPeriod(DateTime? from, DateTime? to, string countryCode)
    {
        if (!from.HasValue || !to.HasValue)
            return new List<DateTime>();

        var holidays = new List<DateTime>();
        var currentDate = from.Value;

        while (currentDate <= to.Value)
        {
            var holidaysInWeek = await catalogRepository.GetHolidaysInWeek(currentDate, countryCode);
            holidays.AddRange(holidaysInWeek);
            currentDate = currentDate.AddDays(7);
        }

        return [.. holidays.Distinct()];
    }

    /// <summary>
    /// Processes timesheets and returns invoice line items with TimeSheetTotal entities
    /// Creates TimeSheetTotal entities and assigns them to navigation properties
    /// EF Core will cascade insert TimeSheetTotal entities when the invoice is saved
    /// </summary>
    protected List<T> ProcessTimesheets<T>(
        List<TimeSheetApprovedBillingModel> timesheets,
        List<DateTime> holidays) where T : IInvoiceLineItem, new()
    {
        var items = new List<T>();

        // Group timesheets by week and worker (composite key)
        var workerWeekGroups = timesheets
            .GroupBy(t => new { Week = t.Date.GetWeekEndingCurrentWeek(), t.WorkerId });

        foreach (var group in workerWeekGroups)
        {
            TimeSpan accumulatedRegularHours = TimeSpan.Zero;

            // Process each timesheet individually, ordered by date
            foreach (var timesheet in group.OrderBy(t => t.Date))
            {
                bool isHoliday = IsPublicHoliday(timesheet.Date, holidays);
                var hoursBreakdown = CalculateHoursForItem(timesheet, ref accumulatedRegularHours, isHoliday);

                // Create TimeSheetTotal entity for this timesheet
                var timeSheetTotal = calculatorService.CreateTimeSheetTotalEntity(
                    timesheet.TimeSheetId,
                    hoursBreakdown,
                    accumulatedRegularHours);

                var jobTitle = timesheet.JobTitle;
                var agencyRate = timesheet.AgencyRate;

                // Create items for this timesheet with TimeSheetTotal reference
                var regularHours = hoursBreakdown.RegularHours.TotalHours + hoursBreakdown.OtherRegularHours.TotalHours;
                if (regularHours > 0)
                {
                    var regularAmount = calculatorService.CalculateRegularAmount(agencyRate, regularHours);
                    var item = new T
                    {
                        Quantity = regularHours,
                        UnitPrice = agencyRate,
                        Description = $"Charge for {jobTitle} / Regular",
                        AgencyRate = agencyRate,
                        Regular = regularAmount,
                        Total = regularAmount,
                        TotalGross = regularAmount,
                        TotalNet = regularAmount
                    };
                    AssignTimeSheetTotal(item, timeSheetTotal);
                    items.Add(item);
                }

                var overtimeHours = hoursBreakdown.OvertimeHours.TotalHours;
                if (overtimeHours > 0)
                {
                    var overtimeRate = agencyRate * rates.OverTime;
                    var overtimeAmount = calculatorService.CalculateOvertimeAmount(agencyRate, rates.OverTime, overtimeHours);
                    var item = new T
                    {
                        Quantity = overtimeHours,
                        UnitPrice = overtimeRate,
                        Description = $"Charge for {jobTitle} / Overtime",
                        AgencyRate = agencyRate,
                        Overtime = overtimeAmount,
                        Total = overtimeAmount,
                        TotalGross = overtimeAmount,
                        TotalNet = overtimeAmount
                    };
                    AssignTimeSheetTotal(item, timeSheetTotal);
                    items.Add(item);
                }

                var holidayHours = hoursBreakdown.HolidayHours.TotalHours;
                if (holidayHours > 0)
                {
                    var holidayRate = agencyRate * rates.Holiday;
                    var holidayAmount = calculatorService.CalculateHolidayAmount(agencyRate, rates.Holiday, holidayHours);
                    var item = new T
                    {
                        Quantity = holidayHours,
                        UnitPrice = holidayRate,
                        Description = $"Charge for {jobTitle} / Holiday",
                        AgencyRate = agencyRate,
                        Holiday = holidayAmount,
                        Total = holidayAmount,
                        TotalGross = holidayAmount,
                        TotalNet = holidayAmount
                    };
                    AssignTimeSheetTotal(item, timeSheetTotal);
                    items.Add(item);
                }

                var missingHours = timesheet.MissingHours.TotalHours;
                if (missingHours > 0)
                {
                    var missingAmount = calculatorService.CalculateMissingAmount(agencyRate, missingHours);
                    var item = new T
                    {
                        Quantity = missingHours,
                        UnitPrice = agencyRate,
                        Description = $"Charge for {jobTitle} / Missing Hours",
                        AgencyRate = agencyRate,
                        Missing = missingAmount,
                        Total = missingAmount,
                        TotalGross = missingAmount,
                        TotalNet = missingAmount
                    };
                    AssignTimeSheetTotal(item, timeSheetTotal);
                    items.Add(item);
                }

                var missingOvertimeHours = timesheet.MissingHoursOvertime.TotalHours;
                if (missingOvertimeHours > 0)
                {
                    var missingOvertimeRate = agencyRate * rates.OverTime;
                    var missingOvertimeAmount = calculatorService.CalculateOvertimeAmount(agencyRate, rates.OverTime, missingOvertimeHours);
                    var item = new T
                    {
                        Quantity = missingOvertimeHours,
                        UnitPrice = missingOvertimeRate,
                        Description = $"Charge for {jobTitle} / Missing Overtime Hours",
                        AgencyRate = agencyRate,
                        MissingOvertime = missingOvertimeAmount,
                        Total = missingOvertimeAmount,
                        TotalGross = missingOvertimeAmount,
                        TotalNet = missingOvertimeAmount
                    };
                    AssignTimeSheetTotal(item, timeSheetTotal);
                    items.Add(item);
                }
            }
        }

        return items;
    }

    /// <summary>
    /// Assigns TimeSheetTotal entity to invoice line item
    /// Works for both InvoiceTotal and InvoiceUSAItem
    /// </summary>
    private void AssignTimeSheetTotal<T>(T item, TimeSheetTotalEntity timeSheetTotal) where T : IInvoiceLineItem
    {
        // Use pattern matching to assign TimeSheetTotal navigation property
        if (item is InvoiceTotal invoiceTotal)
        {
            invoiceTotal.TimeSheetTotalId = timeSheetTotal.Id;
            invoiceTotal.TimeSheetTotal = timeSheetTotal;
        }
        else if (item is InvoiceUSAItem invoiceUSAItem)
        {
            invoiceUSAItem.TimeSheetTotalId = timeSheetTotal.Id;
            invoiceUSAItem.TimeSheetTotal = timeSheetTotal;
        }
    }

    /// <summary>
    /// Consolidates invoice items for preview display
    /// Groups items by description and sums quantities and totals
    /// </summary>
    protected List<InvoiceSummaryItemModel> ConsolidatePreviewItems<T>(IEnumerable<T> items) where T : IInvoiceLineItem
    {
        return items
            .GroupBy(i => i.Description)
            .Select(group =>
            {
                var totalQty = group.Sum(i => i.Quantity);
                var totalAmount = group.Sum(i => i.Total);
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

    /// <summary>
    /// Converts additional items to preview format
    /// </summary>
    protected List<InvoiceSummaryItemModel> ConsolidatePreviewAdditionalItems(IEnumerable<InvoiceAdditionalItem> additionalItems)
    {
        return additionalItems
            .Select(item => new InvoiceSummaryItemModel
            {
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Total = item.Total
            })
            .ToList();
    }

    #endregion

    #region Subcontractor Reports

    /// <summary>
    /// Creates subcontractor reports for the given agency and company
    /// </summary>
    protected async Task CreateSubcontractorReportsAsync(IEnumerable<Guid> agencyIds, Guid companyId)
    {
        var timesheets = await timeSheetRepository.GetTimeSheetForCreatingReportsSubcontractor(agencyIds, companyId);
        if (timesheets.Count == 0) return;

        // Group by worker and week
        var workerWeekGroups = timesheets
            .GroupBy(t => new { t.WorkerId, Week = t.Date.GetWeekEndingCurrentWeek() });

        foreach (var group in workerWeekGroups)
        {
            var first = group.First();
            var workerProfileId = first.WorkerProfileId;

            TimeSpan accumulatedRegularHours = TimeSpan.Zero;
            var from = group.Min(t => t.Date);
            var to = group.Max(t => t.Date);
            var countryCode = first.CountryCode;
            var holidays = await GetHolidaysForPeriod(from, to, countryCode);

            // Create a WageDetail for each timesheet (not just one per week)
            var wageDetails = new List<ReportSubcontractorWageDetail>();

            foreach (var timesheet in group.OrderBy(t => t.Date))
            {
                bool isHoliday = IsPublicHoliday(timesheet.Date, holidays);
                var hoursBreakdown = CalculateHoursForItem(timesheet, ref accumulatedRegularHours, isHoliday);

                var workerRate = timesheet.WorkerRate;
                var regularHours = hoursBreakdown.RegularHours.TotalHours + hoursBreakdown.OtherRegularHours.TotalHours;
                var overtimeHours = hoursBreakdown.OvertimeHours.TotalHours;
                var holidayHours = hoursBreakdown.HolidayHours.TotalHours;

                var regularAmount = calculatorService.CalculateRegularAmount(workerRate, regularHours);
                var overtimeAmount = calculatorService.CalculateOvertimeAmount(workerRate, rates.OverTime, overtimeHours);
                var holidayAmount = calculatorService.CalculateHolidayAmount(workerRate, rates.Holiday, holidayHours);

                // Create TimeSheetTotalPayroll for this timesheet
                var timeSheetTotal = calculatorService.CreateTimeSheetTotalPayrollEntity(
                    timesheet.TimeSheetId,
                    hoursBreakdown,
                    accumulatedRegularHours);

                // Create WageDetail linked to the TimeSheetTotalPayroll
                var wageDetail = new ReportSubcontractorWageDetail(
                    workerRate: workerRate,
                    regular: regularAmount,
                    otherRegular: 0,
                    missing: 0,
                    missingOvertime: 0,
                    nightShift: 0,
                    holiday: holidayAmount,
                    overtime: overtimeAmount
                )
                {
                    TimeSheetTotal = timeSheetTotal
                };

                wageDetails.Add(wageDetail);
            }

            var totalRegular = wageDetails.Sum(w => w.Regular);
            var totalOvertime = wageDetails.Sum(w => w.Overtime);
            var totalHoliday = wageDetails.Sum(w => w.Holiday);
            var gross = totalRegular + totalOvertime + totalHoliday;

            // Create report
            var report = new ReportSubcontractor
            {
                WorkerProfileId = workerProfileId,
                RegularWage = totalRegular,
                Gross = gross,
                PublicHolidayPay = 0,
                Earnings = gross,
                TotalNet = gross,
                DateWorkBegins = group.Min(t => t.Date),
                DateWorkEnd = group.Max(t => t.Date),
                WeekEnding = group.Key.Week,
            };

            report.AddWageDetail(wageDetails);

            // Handle other deductions
            var otherDeductions = group
                .Where(t => t.DeductionsOthers > 0)
                .Select(t => ReportSubContractorOtherDeduction.CreateDefaultDeduction(t.DeductionsOthers, t.DeductionsOthersDescription));

            report.AddOtherDeductionsDetail(otherDeductions);

            await subcontractorRepository.Create(report);
        }

        await subcontractorRepository.SaveChangesAsync();
    }

    #endregion
}
