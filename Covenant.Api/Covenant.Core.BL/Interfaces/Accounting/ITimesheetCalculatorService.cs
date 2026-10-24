using Covenant.Common.Entities.Requests;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Requests.TimeSheets;

namespace Covenant.Core.BL.Interfaces.Accounting;

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
    Task<decimal> CalculateHolidayPayBase(Guid workerProfileId, DateTime lookbackStart, DateTime holidayWeekEnd);

    TimeSheetTotal CreateTimeSheetTotalEntity(
        Guid timeSheetId,
        TimeSheetHoursBreakdown hoursBreakdown,
        TimeSpan accumulatedHours);

    TimeSheetTotalPayroll CreateTimeSheetTotalPayrollEntity(
        Guid timeSheetId,
        TimeSheetHoursBreakdown hoursBreakdown,
        TimeSpan accumulatedHours);
}
