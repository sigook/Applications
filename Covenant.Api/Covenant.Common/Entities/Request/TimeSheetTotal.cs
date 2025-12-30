namespace Covenant.Common.Entities.Request
{
    public class TimeSheetTotal : ITimeSheetTotal
    {
        private TimeSheetTotal()
        {
        }

        public TimeSheetTotal(ITimeSheetTotal tst)
        {
            Id = tst.Id;
            TotalHours = tst.TotalHours;
            RegularHours = tst.RegularHours;
            OtherRegularHours = tst.OtherRegularHours;
            NightShiftHours = tst.NightShiftHours;
            HolidayHours = tst.HolidayHours;
            OvertimeHours = tst.OvertimeHours;
            AccumulateWeekHours = tst.AccumulateWeekHours;
            TimeSheetId = tst.TimeSheetId;
            TimeSheet = tst.TimeSheet;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public TimeSpan TotalHours { get; private set; }
        public TimeSpan RegularHours { get; private set; }
        public TimeSpan OtherRegularHours { get; private set; }
        public TimeSpan NightShiftHours { get; private set; }
        public TimeSpan HolidayHours { get; private set; }
        public TimeSpan OvertimeHours { get; private set; }
        public TimeSpan AccumulateWeekHours { get; private set; }
        public Guid TimeSheetId { get; private set; }
        public TimeSheet TimeSheet { get; internal set; }

        public static TimeSheetTotal CreateTotal(
            Guid timeSheetId,
            TimeSpan totalHours,
            TimeSpan regularHours,
            TimeSpan overtimeHours,
            TimeSpan nightShiftHours,
            TimeSpan accumulatedHours) =>
            CreateTotalWithOtherRegular(timeSheetId, totalHours, regularHours, TimeSpan.Zero, overtimeHours, nightShiftHours, accumulatedHours);

        public static TimeSheetTotal CreateTotalWithOtherRegular(
            Guid timeSheetId,
            TimeSpan totalHours,
            TimeSpan regularHours,
            TimeSpan otherRegularHours,
            TimeSpan overtimeHours,
            TimeSpan nightShiftHours,
            TimeSpan accumulatedHours) =>
            new TimeSheetTotal
            {
                TimeSheetId = timeSheetId,
                TotalHours = totalHours,
                RegularHours = regularHours,
                OtherRegularHours = otherRegularHours,
                OvertimeHours = overtimeHours,
                NightShiftHours = nightShiftHours,
                HolidayHours = TimeSpan.Zero,
                AccumulateWeekHours = accumulatedHours
            };

        public static TimeSheetTotal CreateTotalForHoliday(Guid timeSheetId, TimeSpan totalHours, TimeSpan accumulatedHours) =>
            new TimeSheetTotal
            {
                TimeSheetId = timeSheetId,
                TotalHours = totalHours,
                OtherRegularHours = TimeSpan.Zero,
                RegularHours = TimeSpan.Zero,
                NightShiftHours = TimeSpan.Zero,
                HolidayHours = totalHours,
                OvertimeHours = TimeSpan.Zero,
                AccumulateWeekHours = accumulatedHours
            };
    }
}