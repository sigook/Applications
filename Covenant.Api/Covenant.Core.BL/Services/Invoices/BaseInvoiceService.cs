using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Entities.Request;
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
using TimeSheetTotalEntity = Covenant.Common.Entities.Request.TimeSheetTotal;

namespace Covenant.Core.BL.Services.Invoices;

public abstract class BaseInvoiceService
{
    protected readonly ITimeSheetRepository timeSheetRepository;
    protected readonly IInvoiceRepository invoiceRepository;
    protected readonly IAgencyRepository agencyRepository;
    protected readonly ICompanyRepository companyRepository;
    protected readonly ILocationRepository locationRepository;
    protected readonly ICatalogRepository catalogRepository;
    protected readonly ITimeService timeService;
    protected readonly Rates rates;
    protected readonly ISubcontractorRepository subcontractorRepository;
    protected readonly TimeLimits timeLimits;

    protected BaseInvoiceService(
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
    {
        this.timeSheetRepository = timeSheetRepository;
        this.invoiceRepository = invoiceRepository;
        this.agencyRepository = agencyRepository;
        this.companyRepository = companyRepository;
        this.locationRepository = locationRepository;
        this.catalogRepository = catalogRepository;
        this.timeService = timeService;
        this.rates = rates;
        this.subcontractorRepository = subcontractorRepository;
        this.timeLimits = timeLimits;
    }

    // Abstract methods to be implemented by country-specific services
    public abstract Task<Result<InvoicePreviewModel>> PreviewAsync(IEnumerable<Guid> agencyIds, CreateInvoiceModel model);
    public abstract Task<Result<Guid>> CreateAsync(IEnumerable<Guid> agencyIds, CreateInvoiceModel model);

    #region Hour Calculations (Replaces TimeSheetTotal logic)

    /// <summary>
    /// Calculates hours breakdown for a single timesheet item
    /// This replaces TimeSheetTotalCalculator.Calculate() logic
    /// </summary>
    protected TimeSheetHoursBreakdown CalculateHoursForItem(
        ITimeSheetHoursCalculable item,
        ref TimeSpan accumulatedRegularHours,
        bool isHoliday)
    {
        var breakdown = new TimeSheetHoursBreakdown();

        // Calculate total hours: TimeOut - TimeIn - Break
        TimeSpan itemTotalHours = TimeSpan.Zero;
        if (item.TimeOutApproved.HasValue && item.TimeInApproved.HasValue)
        {
            itemTotalHours = item.TimeOutApproved.Value - item.TimeInApproved.Value - item.DurationBreak;
        }

        // If it's a holiday, all hours go to holiday hours
        if (isHoliday)
        {
            breakdown.HolidayHours = itemTotalHours;
            breakdown.RegularHours = TimeSpan.Zero;
            breakdown.OvertimeHours = TimeSpan.Zero;
            breakdown.OtherRegularHours = TimeSpan.Zero;
            return breakdown;
        }

        // Calculate overtime based on accumulated weekly hours
        var maxWeeklyHours = item.OvertimeStartsAfter;
        accumulatedRegularHours += itemTotalHours;

        if (accumulatedRegularHours > maxWeeklyHours)
        {
            // Some hours are overtime
            var overtimeThisItem = accumulatedRegularHours - maxWeeklyHours;
            if (overtimeThisItem > itemTotalHours)
            {
                // All hours for this item are overtime
                breakdown.OvertimeHours = itemTotalHours;
                breakdown.RegularHours = TimeSpan.Zero;
            }
            else
            {
                // Part overtime, part regular
                breakdown.OvertimeHours = overtimeThisItem;
                breakdown.RegularHours = itemTotalHours - overtimeThisItem;
            }
        }
        else
        {
            // All hours are regular
            breakdown.RegularHours = itemTotalHours;
            breakdown.OvertimeHours = TimeSpan.Zero;
        }

        // OtherRegularHours (if needed)
        breakdown.OtherRegularHours = TimeSpan.Zero;

        return breakdown;
    }

    #endregion

    #region Amount Calculations (Replaces InvoiceFormulas logic)

    /// <summary>
    /// Calculates amount for regular hours
    /// Replaces: InvoiceFormulas.Regular(rate, hours)
    /// Formula: rate × hours
    /// </summary>
    protected decimal CalculateRegularAmount(decimal rate, double hours)
    {
        // Formula: rate × hours
        return decimal.Multiply(rate, (decimal)hours);
    }

    /// <summary>
    /// Calculates amount for overtime hours
    /// Replaces: InvoiceFormulas.Overtime(rate, overtimeRate, hours)
    /// Formula: rate × overtime multiplier × hours
    /// </summary>
    protected decimal CalculateOvertimeAmount(decimal rate, decimal overtimeMultiplier, double hours)
    {
        // Formula: rate × overtime multiplier × hours
        // Example: $20/hr × 1.5 × 8 hours = $240
        return decimal.Multiply(decimal.Multiply(rate, overtimeMultiplier), (decimal)hours);
    }

    /// <summary>
    /// Calculates amount for holiday hours
    /// Replaces: InvoiceFormulas.Holiday(rate, holidayRate, hours)
    /// Formula: rate × holiday multiplier × hours
    /// </summary>
    protected decimal CalculateHolidayAmount(decimal rate, decimal holidayMultiplier, double hours)
    {
        // Formula: rate × holiday multiplier × hours
        // Example: $20/hr × 2.0 × 8 hours = $320
        return decimal.Multiply(decimal.Multiply(rate, holidayMultiplier), (decimal)hours);
    }

    /// <summary>
    /// Calculates amount for missing hours
    /// Replaces: InvoiceFormulas.Missing(rate, hours)
    /// Formula: rate × hours
    /// </summary>
    protected decimal CalculateMissingAmount(decimal rate, double hours)
    {
        // Formula: rate × hours
        return decimal.Multiply(rate, (decimal)hours);
    }

    /// <summary>
    /// Calculates total gross amount
    /// Replaces: InvoiceFormulas.TotalGross(...)
    /// Formula: sum of all components
    /// </summary>
    protected decimal CalculateTotalGross(
        decimal regular,
        decimal missing,
        decimal missingOvertime,
        decimal holiday,
        decimal overtime)
    {
        // Sum all components
        return regular.Add(missing).Add(missingOvertime).Add(holiday).Add(overtime);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Checks if a date is a public holiday
    /// </summary>
    protected bool IsPublicHoliday(DateTime date, List<DateTime> holidays)
    {
        return holidays.Any(h => h.Date == date.Date);
    }

    /// <summary>
    /// Creates a TimeSheetTotal entity for a timesheet based on hours breakdown
    /// Replicates logic from TimeSheetTotalCalculator.Calculate()
    /// </summary>
    protected TimeSheetTotalEntity CreateTimeSheetTotalEntity(
        Guid timeSheetId,
        TimeSheetHoursBreakdown hoursBreakdown,
        TimeSpan accumulatedHours)
    {
        // If holiday hours exist, create holiday total
        if (hoursBreakdown.HolidayHours > TimeSpan.Zero)
        {
            return TimeSheetTotalEntity.CreateTotalForHoliday(
                timeSheetId,
                hoursBreakdown.HolidayHours,
                accumulatedHours);
        }

        // Calculate total hours
        var totalHours = hoursBreakdown.RegularHours
            + hoursBreakdown.OtherRegularHours
            + hoursBreakdown.OvertimeHours;

        // If other regular hours exist, use CreateTotalWithOtherRegular
        if (hoursBreakdown.OtherRegularHours > TimeSpan.Zero)
        {
            return TimeSheetTotalEntity.CreateTotalWithOtherRegular(
                timeSheetId,
                totalHours,
                hoursBreakdown.RegularHours,
                hoursBreakdown.OtherRegularHours,
                hoursBreakdown.OvertimeHours,
                TimeSpan.Zero, // nightShiftHours (not used in new system)
                accumulatedHours);
        }

        // Otherwise use standard CreateTotal
        return TimeSheetTotalEntity.CreateTotal(
            timeSheetId,
            totalHours,
            hoursBreakdown.RegularHours,
            hoursBreakdown.OvertimeHours,
            TimeSpan.Zero, // nightShiftHours (not used in new system)
            accumulatedHours);
    }

    /// <summary>
    /// Gets holidays for a given period
    /// </summary>
    protected async Task<List<DateTime>> GetHolidaysForPeriod(DateTime? from, DateTime? to)
    {
        if (!from.HasValue || !to.HasValue)
            return new List<DateTime>();

        var holidays = new List<DateTime>();
        var currentDate = from.Value;

        while (currentDate <= to.Value)
        {
            var holidaysInWeek = await catalogRepository.GetHolidaysInWeek(currentDate);
            holidays.AddRange(holidaysInWeek);
            currentDate = currentDate.AddDays(7);
        }

        return holidays.Distinct().ToList();
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
                var timeSheetTotal = CreateTimeSheetTotalEntity(
                    timesheet.TimeSheetId,
                    hoursBreakdown,
                    accumulatedRegularHours);

                var jobTitle = timesheet.JobTitle;
                var agencyRate = timesheet.AgencyRate;

                // Create items for this timesheet with TimeSheetTotal reference
                var regularHours = hoursBreakdown.RegularHours.TotalHours + hoursBreakdown.OtherRegularHours.TotalHours;
                if (regularHours > 0)
                {
                    var regularAmount = CalculateRegularAmount(agencyRate, regularHours);
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
                    var overtimeAmount = CalculateOvertimeAmount(agencyRate, rates.OverTime, overtimeHours);
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
                    var holidayAmount = CalculateHolidayAmount(agencyRate, rates.Holiday, holidayHours);
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
        if (!timesheets.Any()) return;

        // Group by worker and week
        var workerWeekGroups = timesheets
            .GroupBy(t => new { t.WorkerId, Week = t.Date.GetWeekEndingCurrentWeek() });

        foreach (var group in workerWeekGroups)
        {
            var first = group.First();
            var workerProfileId = first.WorkerProfileId;

            TimeSpan accumulatedRegularHours = TimeSpan.Zero;
            var holidays = await GetHolidaysForPeriod(group.Min(t => t.Date), group.Max(t => t.Date));

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

                var regularAmount = CalculateRegularAmount(workerRate, regularHours);
                var overtimeAmount = CalculateOvertimeAmount(workerRate, rates.OverTime, overtimeHours);
                var holidayAmount = CalculateHolidayAmount(workerRate, rates.Holiday, holidayHours);

                // Create TimeSheetTotalPayroll for this timesheet
                var timeSheetTotal = CreateTimeSheetTotalPayrollEntity(
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

    /// <summary>
    /// Creates a TimeSheetTotalPayroll entity for payroll/subcontractor reports
    /// Similar to CreateTimeSheetTotalEntity but returns TimeSheetTotalPayroll
    /// </summary>
    private TimeSheetTotalPayroll CreateTimeSheetTotalPayrollEntity(
        Guid timeSheetId,
        TimeSheetHoursBreakdown hoursBreakdown,
        TimeSpan accumulatedHours)
    {
        // If holiday hours exist, create holiday total
        if (hoursBreakdown.HolidayHours > TimeSpan.Zero)
        {
            return new TimeSheetTotalPayroll(
                TimeSheetTotalEntity.CreateTotalForHoliday(
                    timeSheetId,
                    hoursBreakdown.HolidayHours,
                    accumulatedHours));
        }

        // Calculate total hours
        var totalHours = hoursBreakdown.RegularHours
            + hoursBreakdown.OtherRegularHours
            + hoursBreakdown.OvertimeHours;

        // If other regular hours exist, use CreateTotalWithOtherRegular
        if (hoursBreakdown.OtherRegularHours > TimeSpan.Zero)
        {
            return TimeSheetTotalPayroll.CreateTotalWithOtherRegular(
                timeSheetId,
                totalHours,
                hoursBreakdown.RegularHours,
                hoursBreakdown.OtherRegularHours,
                hoursBreakdown.OvertimeHours,
                TimeSpan.Zero, // nightShiftHours (not used in new system)
                accumulatedHours);
        }

        // Otherwise use standard CreateTotal
        return TimeSheetTotalPayroll.CreateTotal(
            timeSheetId,
            totalHours,
            hoursBreakdown.RegularHours,
            hoursBreakdown.OvertimeHours,
            TimeSpan.Zero, // nightShiftHours (not used in new system)
            accumulatedHours);
    }

    #endregion
}
