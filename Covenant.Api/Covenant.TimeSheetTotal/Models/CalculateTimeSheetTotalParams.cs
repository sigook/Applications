namespace Covenant.TimeSheetTotal.Models
{
    public readonly struct CalculateTimeSheetTotalParams
    {
        public CalculateTimeSheetTotalParams(
            bool breakIsPaid,
            TimeSpan durationBreak,
            bool holidayIsPaid,
            bool payNightShift,
            TimeSpan? timeStartNightShift,
            TimeSpan? timeEndNightShift,
            Guid timeSheetId,
            DateTime timeInApproved,
            DateTime timeOutApproved,
            bool isHoliday,
            TimeSpan maxHoursWeek,
            TimeSpan overtimeStartsFrom)
        {
            BreakIsPaid = breakIsPaid;
            DurationBreak = durationBreak;
            HolidayIsPaid = holidayIsPaid;
            PayNightShift = payNightShift;
            TimeStartNightShift = timeStartNightShift;
            TimeEndNightShift = timeEndNightShift;
            TimeSheetId = timeSheetId;
            TimeInApproved = timeInApproved;
            TimeOutApproved = timeOutApproved;
            IsHoliday = isHoliday;
            MaxHoursWeek = maxHoursWeek;
            OvertimeStartsFrom = overtimeStartsFrom;
        }

        public CalculateTimeSheetTotalParams(
            bool breakIsPaid,
            TimeSpan durationBreak,
            bool holidayIsPaid,
            Guid timeSheetId,
            DateTime timeInApproved,
            DateTime timeOutApproved,
            bool isHoliday,
            TimeSpan maxHoursWeek,
            TimeSpan overtimeStartsFrom) : this(breakIsPaid, durationBreak, holidayIsPaid,
            default,//Deprecated
            default,//Deprecated
            default,//Deprecated
            timeSheetId, timeInApproved, timeOutApproved, isHoliday, maxHoursWeek, overtimeStartsFrom)
        {
            BreakIsPaid = breakIsPaid;
            DurationBreak = durationBreak;
            HolidayIsPaid = holidayIsPaid;
            TimeSheetId = timeSheetId;
            TimeInApproved = timeInApproved;
            TimeOutApproved = timeOutApproved;
            IsHoliday = isHoliday;
            MaxHoursWeek = maxHoursWeek;
            OvertimeStartsFrom = overtimeStartsFrom;
        }
        public bool BreakIsPaid { get; }
        public TimeSpan DurationBreak { get; }
        public bool HolidayIsPaid { get; }
        public bool PayNightShift { get; }
        public TimeSpan? TimeStartNightShift { get; }
        public TimeSpan? TimeEndNightShift { get; }
        public Guid TimeSheetId { get; }
        public DateTime TimeInApproved { get; }
        public DateTime TimeOutApproved { get; }
        public bool IsHoliday { get; }
        public TimeSpan MaxHoursWeek { get; }
        public TimeSpan OvertimeStartsFrom { get; }
    }
}