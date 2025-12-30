using Covenant.Common.Entities.Request;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.TimeSheetTotal.Models;

namespace Covenant.TimeSheetTotal.Services
{
    public static class TimeSheetTotalCalculator
    {
        public static ITimeSheetTotal Calculate(CalculateTimeSheetTotalParams p, ref TimeSpan accumulatedHours)
        {
            TimeSpan timeIn = p.TimeInApproved.TimeOfDay;
            TimeSpan timeOut = p.TimeOutApproved.TimeOfDay;

            TimeSpan totalHours = TotalHours(timeIn, timeOut, p.BreakIsPaid, p.DurationBreak);

            bool applyHoliday = WhenApplyHoliday(p.IsHoliday, p.HolidayIsPaid);

            if (applyHoliday) return Common.Entities.Request.TimeSheetTotal.CreateTotalForHoliday(p.TimeSheetId, totalHours, accumulatedHours);

            accumulatedHours += totalHours;

            TimeSpan nightShiftHours = NightShiftHours(p.PayNightShift, p.TimeStartNightShift, p.TimeEndNightShift, timeIn, timeOut);

            TimeSpan regularHours;
            TimeSpan overtimeHours = OvertimeHours(accumulatedHours, p.OvertimeStartsFrom, totalHours);
            TimeSpan otherRegularHours = OvertimeHours(accumulatedHours, p.MaxHoursWeek, totalHours);
            if (overtimeHours > TimeSpan.Zero)
            {
                otherRegularHours = otherRegularHours.Subtract(overtimeHours);
                if (otherRegularHours > TimeSpan.Zero)
                {
                    regularHours = RegularHours(totalHours, otherRegularHours.Add(overtimeHours), nightShiftHours);
                    return Common.Entities.Request.TimeSheetTotal.CreateTotalWithOtherRegular(p.TimeSheetId, totalHours,
                        regularHours, otherRegularHours, overtimeHours, nightShiftHours, accumulatedHours);
                }
                regularHours = RegularHours(totalHours, overtimeHours, nightShiftHours);
                return Common.Entities.Request.TimeSheetTotal.CreateTotal(p.TimeSheetId, totalHours,
                    regularHours, overtimeHours, nightShiftHours, accumulatedHours);
            }
            if (otherRegularHours > TimeSpan.Zero)
            {
                regularHours = RegularHours(totalHours, otherRegularHours, nightShiftHours);
                return Common.Entities.Request.TimeSheetTotal.CreateTotalWithOtherRegular(p.TimeSheetId, totalHours,
                    regularHours, otherRegularHours, TimeSpan.Zero, nightShiftHours, accumulatedHours);
            }

            regularHours = RegularHours(totalHours, overtimeHours, nightShiftHours);
            return Common.Entities.Request.TimeSheetTotal.CreateTotal(p.TimeSheetId, totalHours,
                regularHours, TimeSpan.Zero, nightShiftHours, accumulatedHours);
        }

        public static ITimeSheetTotal Calculate(TimeSheetApprovedBillingModel p, ref TimeSpan accumulatedHours)
        {
            TimeSpan timeIn = p.TimeInApproved.Value.TimeOfDay;
            TimeSpan timeOut = p.TimeOutApproved.Value.TimeOfDay;

            TimeSpan totalHours = TotalHours(timeIn, timeOut, p.BreakIsPaid, p.DurationBreak);

            bool applyHoliday = WhenApplyHoliday(p.IsHoliday, p.HolidayIsPaid);

            if (applyHoliday) return Common.Entities.Request.TimeSheetTotal.CreateTotalForHoliday(p.TimeSheetId, totalHours, accumulatedHours);

            accumulatedHours += totalHours;

            TimeSpan nightShiftHours = NightShiftHours(default, default, default, timeIn, timeOut);

            TimeSpan regularHours;
            TimeSpan overtimeHours = OvertimeHours(accumulatedHours, p.OvertimeStartsAfter, totalHours);
            TimeSpan otherRegularHours = OvertimeHours(accumulatedHours, p.MaxHoursWeek, totalHours);
            if (overtimeHours > TimeSpan.Zero)
            {
                otherRegularHours = otherRegularHours.Subtract(overtimeHours);
                if (otherRegularHours > TimeSpan.Zero)
                {
                    regularHours = RegularHours(totalHours, otherRegularHours.Add(overtimeHours), nightShiftHours);
                    return Common.Entities.Request.TimeSheetTotal.CreateTotalWithOtherRegular(p.TimeSheetId, totalHours,
                        regularHours, otherRegularHours, overtimeHours, nightShiftHours, accumulatedHours);
                }
                regularHours = RegularHours(totalHours, overtimeHours, nightShiftHours);
                return Common.Entities.Request.TimeSheetTotal.CreateTotal(p.TimeSheetId, totalHours,
                    regularHours, overtimeHours, nightShiftHours, accumulatedHours);
            }
            if (otherRegularHours > TimeSpan.Zero)
            {
                regularHours = RegularHours(totalHours, otherRegularHours, nightShiftHours);
                return Common.Entities.Request.TimeSheetTotal.CreateTotalWithOtherRegular(p.TimeSheetId, totalHours,
                    regularHours, otherRegularHours, TimeSpan.Zero, nightShiftHours, accumulatedHours);
            }

            regularHours = RegularHours(totalHours, overtimeHours, nightShiftHours);
            return Common.Entities.Request.TimeSheetTotal.CreateTotal(p.TimeSheetId, totalHours,
                regularHours, TimeSpan.Zero, nightShiftHours, accumulatedHours);
        }

        public static TimeSpan TotalHours(TimeSpan timeIn, TimeSpan timeOut, bool breakIsPaid, TimeSpan durationBreak)
            => (timeOut - timeIn).Subtract(breakIsPaid ? durationBreak : TimeSpan.Zero);

        public static TimeSpan OvertimeHours(TimeSpan accumulatedHours, TimeSpan overtimeStartsFrom, TimeSpan totalHours) =>
            accumulatedHours <= overtimeStartsFrom
                ? TimeSpan.Zero
                : accumulatedHours.Subtract(totalHours) > overtimeStartsFrom
                    ? totalHours
                    : accumulatedHours.Subtract(overtimeStartsFrom);

        public static TimeSpan RegularHours(TimeSpan totalHours, TimeSpan overtimeHours, TimeSpan nightShiftHours)
        {
            TimeSpan afterOvertime = totalHours.Subtract(overtimeHours);
            return afterOvertime.Subtract(nightShiftHours);
        }

        private static bool WhenApplyHoliday(bool isHoliday, bool holidayIsPaid) => isHoliday && holidayIsPaid;

        public static bool ShouldCalculateNightShift(bool payNightShift, TimeSpan? timeStartNightShift, TimeSpan? timeEndNightShift) =>
            payNightShift && timeStartNightShift.HasValue && timeEndNightShift.HasValue;

        public static TimeSpan NightShiftHours(
            bool payNightShift,
            TimeSpan? timeStartNightShift,
            TimeSpan? timeEndNightShift,
            TimeSpan timeIn,
            TimeSpan timeOut) =>
            ShouldCalculateNightShift(payNightShift, timeStartNightShift, timeEndNightShift)
                ? NightShiftCalculator.Calculate(timeIn, timeOut, timeStartNightShift.GetValueOrDefault(), timeEndNightShift.GetValueOrDefault())
                : TimeSpan.Zero;
    }
}