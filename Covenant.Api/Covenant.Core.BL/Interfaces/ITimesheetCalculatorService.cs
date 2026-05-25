using Covenant.Common.Entities.Request;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Request.TimeSheet;
using TimeSheetTotalEntity = Covenant.Common.Entities.Request.TimeSheetTotal;

namespace Covenant.Core.BL.Interfaces;

public interface ITimesheetCalculatorService
{
    Task<DeductionsResult> CalculateDeductions(decimal totalEarnings, int numberOfWeeks, int year, Guid workerProfileId);

    TimeSheetHoursBreakdown CalculateHoursBreakdown(
        DateTime timeIn,
        DateTime timeOut,
        TimeSpan durationBreak,
        bool breakIsPaid,
        bool isHoliday,
        bool holidayIsPaid,
        ref TimeSpan accumulatedHours,
        TimeSpan overtimeStartsFrom,
        TimeSpan maxHoursWeek);

    decimal CalculateRegularAmount(decimal rate, double hours);
    decimal CalculateOvertimeAmount(decimal rate, decimal multiplier, double hours);
    decimal CalculateHolidayAmount(decimal rate, decimal multiplier, double hours);
    decimal CalculateMissingAmount(decimal rate, double hours);
    decimal CalculateVacationsAmount(decimal gross, decimal vacationsRate);
    decimal CalculateTotalGross(decimal regular, decimal missing, decimal missingOvertime, decimal holiday, decimal overtime);

    /// <summary>
    /// Public holiday pay base: the worker's full gross earnings across the given timesheets
    /// (regular + other-regular + overtime + worked-holiday + missing) plus vacation pay on that gross.
    /// </summary>
    decimal CalculateHolidayPayBase(IEnumerable<TimeSheetApprovedPayrollModel> timesheets);

    /// <summary>
    /// Resolves the public holiday pay amount from the entitlement flags and the four-week base:
    /// 0 if already paid or not entitled, the agency custom value when present, otherwise base / 20.
    /// </summary>
    (decimal Amount, string Description) ResolveHolidayPay(RegularWageWorker wages);

    TimeSheetTotalEntity CreateTimeSheetTotalEntity(
        Guid timeSheetId,
        TimeSheetHoursBreakdown hoursBreakdown,
        TimeSpan accumulatedHours);

    TimeSheetTotalPayroll CreateTimeSheetTotalPayrollEntity(
        Guid timeSheetId,
        TimeSheetHoursBreakdown hoursBreakdown,
        TimeSpan accumulatedHours);
}
