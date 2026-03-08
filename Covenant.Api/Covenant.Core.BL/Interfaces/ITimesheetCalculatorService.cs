using Covenant.Common.Entities.Request;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Request.TimeSheet;
using TimeSheetTotalEntity = Covenant.Common.Entities.Request.TimeSheetTotal;

namespace Covenant.Core.BL.Interfaces;

public interface ITimesheetCalculatorService
{
    Task<DeductionsResult> CalculateDeductions(
        decimal totalEarnings,
        int numberOfWeeks,
        int year,
        Guid workerProfileId);

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

    TimeSheetTotalEntity CreateTimeSheetTotalEntity(
        Guid timeSheetId,
        TimeSheetHoursBreakdown hoursBreakdown,
        TimeSpan accumulatedHours);

    TimeSheetTotalPayroll CreateTimeSheetTotalPayrollEntity(
        Guid timeSheetId,
        TimeSheetHoursBreakdown hoursBreakdown,
        TimeSpan accumulatedHours);
}
