using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using TimeSheetTotalEntity = Covenant.Common.Entities.Request.TimeSheetTotal;

namespace Covenant.Core.BL.Services.Shared;

public class TimesheetCalculatorService : ITimesheetCalculatorService
{
    private readonly IDeductionsRepository _deductionsRepository;
    private readonly IWorkerRepository _workerRepository;
    private readonly Rates _rates;

    public TimesheetCalculatorService(
        IDeductionsRepository deductionsRepository,
        IWorkerRepository workerRepository,
        Rates rates)
    {
        _deductionsRepository = deductionsRepository;
        _workerRepository = workerRepository;
        _rates = rates;
    }

    #region Deductions

    public async Task<DeductionsResult> CalculateDeductions(decimal totalEarnings, int numberOfWeeks, int year, Guid workerProfileId)
    {
        var workerProfileTaxCategory = await _workerRepository.GetWorkerProfileTaxCategory(workerProfileId);
        var federalCategory = workerProfileTaxCategory?.FederalCategory ?? TaxCategory.Cc1;
        var provincialCategory = workerProfileTaxCategory?.ProvincialCategory ?? TaxCategory.Cc1;

        var cpp = numberOfWeeks switch
        {
            1 => await _deductionsRepository.GetCppWeekly(totalEarnings, year),
            2 => await _deductionsRepository.GetCppBiWeekly(totalEarnings, year),
            3 => await _deductionsRepository.GetCppSemiMonthly(totalEarnings, year),
            _ => await _deductionsRepository.GetCppMonthly(totalEarnings, year),
        };
        cpp = workerProfileTaxCategory?.Cpp ?? cpp;

        var ei = workerProfileTaxCategory?.Ei ?? decimal.Multiply(totalEarnings, _rates.EmploymentInsurance).DefaultMoneyRound();

        var federalTax = numberOfWeeks switch
        {
            1 => await _deductionsRepository.GetFederalTaxWeekly(totalEarnings, year, federalCategory),
            2 => await _deductionsRepository.GetFederalTaxBiWeekly(totalEarnings, year, federalCategory),
            3 => await _deductionsRepository.GetFederalTaxSemiMonthly(totalEarnings, year, federalCategory),
            _ => await _deductionsRepository.GetFederalTaxMonthly(totalEarnings, year, federalCategory),
        };

        var provincialTax = numberOfWeeks switch
        {
            1 => await _deductionsRepository.GetProvincialTaxWeekly(totalEarnings, year, provincialCategory),
            2 => await _deductionsRepository.GetProvincialTaxBiWeekly(totalEarnings, year, provincialCategory),
            3 => await _deductionsRepository.GetProvincialTaxSemiMonthly(totalEarnings, year, provincialCategory),
            _ => await _deductionsRepository.GetProvincialTaxMonthly(totalEarnings, year, provincialCategory),
        };

        return new DeductionsResult
        {
            Cpp = cpp,
            Ei = ei,
            FederalTax = federalTax ?? 0,
            ProvincialTax = provincialTax ?? 0,
        };
    }

    #endregion

    #region Hour Calculations

    /// <summary>
    /// Calculates the hours breakdown for a single timesheet entry.
    /// Consolidates logic from TimeSheetTotalCalculator.Calculate() and BaseInvoiceService.CalculateHoursForItem().
    /// Supports: BreakIsPaid, HolidayIsPaid, OtherRegularHours (two-threshold system).
    /// Does NOT support: NightShift (deprecated).
    /// </summary>
    public TimeSheetHoursBreakdown CalculateHoursBreakdown(
        DateTime timeIn,
        DateTime timeOut,
        TimeSpan durationBreak,
        bool breakIsPaid,
        bool isHoliday,
        bool holidayIsPaid,
        ref TimeSpan accumulatedHours,
        TimeSpan overtimeStartsFrom,
        TimeSpan maxHoursWeek)
    {
        var breakdown = new TimeSheetHoursBreakdown();

        // Step 1: Calculate total hours (TimeOut - TimeIn - Break if applicable)
        var totalHours = (timeOut - timeIn).Subtract(breakIsPaid ? durationBreak : TimeSpan.Zero);

        // Step 2: If holiday and paid, all hours go to holiday (don't accumulate)
        if (isHoliday && holidayIsPaid)
        {
            breakdown.HolidayHours = totalHours;
            breakdown.RegularHours = TimeSpan.Zero;
            breakdown.OtherRegularHours = TimeSpan.Zero;
            breakdown.OvertimeHours = TimeSpan.Zero;
            return breakdown;
        }

        // Step 3: Accumulate hours for overtime calculation
        accumulatedHours += totalHours;

        // Step 4: Calculate overtime (hours beyond overtimeStartsFrom)
        var overtimeHours = CalculateExcessHours(accumulatedHours, overtimeStartsFrom, totalHours);

        // Step 5: Calculate other regular hours (hours beyond maxHoursWeek, minus overtime)
        var otherRegularRaw = CalculateExcessHours(accumulatedHours, maxHoursWeek, totalHours);
        var otherRegularHours = TimeSpan.Zero;

        if (overtimeHours > TimeSpan.Zero)
        {
            otherRegularHours = otherRegularRaw.Subtract(overtimeHours);
            if (otherRegularHours < TimeSpan.Zero)
                otherRegularHours = TimeSpan.Zero;
        }
        else if (otherRegularRaw > TimeSpan.Zero)
        {
            otherRegularHours = otherRegularRaw;
        }

        // Step 6: Calculate regular hours (total - overtime - otherRegular)
        var regularHours = totalHours.Subtract(overtimeHours).Subtract(otherRegularHours);

        breakdown.RegularHours = regularHours;
        breakdown.OtherRegularHours = otherRegularHours;
        breakdown.OvertimeHours = overtimeHours;
        breakdown.HolidayHours = TimeSpan.Zero;

        return breakdown;
    }

    /// <summary>
    /// Calculates how many hours exceed the given threshold.
    /// Replicates TimeSheetTotalCalculator.OvertimeHours() logic.
    /// </summary>
    private static TimeSpan CalculateExcessHours(TimeSpan accumulatedHours, TimeSpan threshold, TimeSpan totalHours)
    {
        if (accumulatedHours <= threshold)
            return TimeSpan.Zero;

        // If accumulated hours before this item were already over threshold, all hours are excess
        if (accumulatedHours.Subtract(totalHours) > threshold)
            return totalHours;

        // Otherwise, only the hours beyond threshold are excess
        return accumulatedHours.Subtract(threshold);
    }

    #endregion

    #region Amount Calculations

    public decimal CalculateRegularAmount(decimal rate, double hours)
    {
        return decimal.Multiply(rate, (decimal)hours);
    }

    public decimal CalculateOvertimeAmount(decimal rate, decimal multiplier, double hours)
    {
        return decimal.Multiply(decimal.Multiply(rate, multiplier), (decimal)hours);
    }

    public decimal CalculateHolidayAmount(decimal rate, decimal multiplier, double hours)
    {
        return decimal.Multiply(decimal.Multiply(rate, multiplier), (decimal)hours);
    }

    public decimal CalculateMissingAmount(decimal rate, double hours)
    {
        return decimal.Multiply(rate, (decimal)hours);
    }

    public decimal CalculateVacationsAmount(decimal gross, decimal vacationsRate)
    {
        return decimal.Multiply(gross, vacationsRate).DefaultMoneyRound();
    }

    public decimal CalculateTotalGross(
        decimal regular,
        decimal missing,
        decimal missingOvertime,
        decimal holiday,
        decimal overtime)
    {
        return regular.Add(missing).Add(missingOvertime).Add(holiday).Add(overtime);
    }

    #endregion

    #region TimeSheetTotal Entity Creation

    public TimeSheetTotalEntity CreateTimeSheetTotalEntity(
        Guid timeSheetId,
        TimeSheetHoursBreakdown hoursBreakdown,
        TimeSpan accumulatedHours)
    {
        if (hoursBreakdown.HolidayHours > TimeSpan.Zero)
        {
            return TimeSheetTotalEntity.CreateTotalForHoliday(
                timeSheetId,
                hoursBreakdown.HolidayHours,
                accumulatedHours);
        }

        var totalHours = hoursBreakdown.RegularHours
            + hoursBreakdown.OtherRegularHours
            + hoursBreakdown.OvertimeHours;

        if (hoursBreakdown.OtherRegularHours > TimeSpan.Zero)
        {
            return TimeSheetTotalEntity.CreateTotalWithOtherRegular(
                timeSheetId,
                totalHours,
                hoursBreakdown.RegularHours,
                hoursBreakdown.OtherRegularHours,
                hoursBreakdown.OvertimeHours,
                TimeSpan.Zero,
                accumulatedHours);
        }

        return TimeSheetTotalEntity.CreateTotal(
            timeSheetId,
            totalHours,
            hoursBreakdown.RegularHours,
            hoursBreakdown.OvertimeHours,
            TimeSpan.Zero,
            accumulatedHours);
    }

    public TimeSheetTotalPayroll CreateTimeSheetTotalPayrollEntity(
        Guid timeSheetId,
        TimeSheetHoursBreakdown hoursBreakdown,
        TimeSpan accumulatedHours)
    {
        if (hoursBreakdown.HolidayHours > TimeSpan.Zero)
        {
            return new TimeSheetTotalPayroll(
                TimeSheetTotalEntity.CreateTotalForHoliday(
                    timeSheetId,
                    hoursBreakdown.HolidayHours,
                    accumulatedHours));
        }

        var totalHours = hoursBreakdown.RegularHours
            + hoursBreakdown.OtherRegularHours
            + hoursBreakdown.OvertimeHours;

        if (hoursBreakdown.OtherRegularHours > TimeSpan.Zero)
        {
            return TimeSheetTotalPayroll.CreateTotalWithOtherRegular(
                timeSheetId,
                totalHours,
                hoursBreakdown.RegularHours,
                hoursBreakdown.OtherRegularHours,
                hoursBreakdown.OvertimeHours,
                TimeSpan.Zero,
                accumulatedHours);
        }

        return TimeSheetTotalPayroll.CreateTotal(
            timeSheetId,
            totalHours,
            hoursBreakdown.RegularHours,
            hoursBreakdown.OvertimeHours,
            TimeSpan.Zero,
            accumulatedHours);
    }

    #endregion
}
