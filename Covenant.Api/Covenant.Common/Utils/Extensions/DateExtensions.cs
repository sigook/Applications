using System.Globalization;

namespace Covenant.Common.Utils.Extensions
{
    public static class DateExtensions
    {
        public static int GetWeekOfYearStartSunday(this DateTime dateTime) =>
            CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay,
                DayOfWeek.Sunday);

        public static int GetNumberOfWeeksIn(DateTime start, DateTime end)
        {
            if (start > end) return 0;
            if (start.Date == end.Date) return 1;
            int weekStart = start.GetWeekOfYearStartSunday();
            int weekEnd = end.GetWeekOfYearStartSunday();
            if (start.Year == end.Year)
            {
                if (weekStart == weekEnd) return 1;
                if (weekStart < weekEnd)
                {
                    var weeks = 0;
                    for (int i = weekStart; i <= weekEnd; i++) weeks++;
                    return weeks;
                }
            }
            int lastWeekOfTheYear = new DateTime(start.Year, 12, 31).GetWeekOfYearStartSunday();
            return lastWeekOfTheYear - weekStart + weekEnd;
        }

        public static DateTime GetWeekEndingWeekBefore(this DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday: return date.AddDays(-1);
                case DayOfWeek.Monday: return date.AddDays(-2);
                case DayOfWeek.Tuesday: return date.AddDays(-3);
                case DayOfWeek.Wednesday: return date.AddDays(-4);
                case DayOfWeek.Thursday: return date.AddDays(-5);
                case DayOfWeek.Friday: return date.AddDays(-6);
                case DayOfWeek.Saturday: return date.AddDays(-7);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static DateTime GetWeekEndingCurrentWeek(this DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday: return date.AddDays(6);
                case DayOfWeek.Monday: return date.AddDays(5);
                case DayOfWeek.Tuesday: return date.AddDays(4);
                case DayOfWeek.Wednesday: return date.AddDays(3);
                case DayOfWeek.Thursday: return date.AddDays(2);
                case DayOfWeek.Friday: return date.AddDays(1);
                case DayOfWeek.Saturday: return date.AddDays(0);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static DateTime GetPaymentDateForExternalWorkers(this DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday: return date.AddDays(12);
                case DayOfWeek.Monday: return date.AddDays(11);
                case DayOfWeek.Tuesday: return date.AddDays(10);
                case DayOfWeek.Wednesday: return date.AddDays(9);
                case DayOfWeek.Thursday: return date.AddDays(8);
                case DayOfWeek.Friday: return date.AddDays(7);
                case DayOfWeek.Saturday: return date.AddDays(6);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static DateTime GetPaymentDateForInternalWorkers(this DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday: return date.AddDays(5);
                case DayOfWeek.Monday: return date.AddDays(4);
                case DayOfWeek.Tuesday: return date.AddDays(3);
                case DayOfWeek.Wednesday: return date.AddDays(2);
                case DayOfWeek.Thursday: return date.AddDays(1);
                case DayOfWeek.Friday: return date.AddDays(0);
                case DayOfWeek.Saturday: return date.AddDays(-1);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static IEnumerable<DateTime> GetRangeOfDaysWorkerMustWorkToReceiveHolidayPay(this DateTime holiday)
        {
            DateTime oneDayAfterTheHoliday = holiday.AddDays(1);
            TimeSpan oneDay = TimeSpan.FromDays(1);
            DateTime oneDayBeforeTheHoliday = holiday.Subtract(oneDay);
            return new[] { holiday, oneDayAfterTheHoliday, oneDayBeforeTheHoliday };
        }

        public static DateTime GetStart(this DateTime end)
        {
            const int days = 7;
            const int weeks = 4;
            const int day = 1;
            const int fourWeeks = days * weeks;
            return end.Subtract(TimeSpan.FromDays(fourWeeks - day));
        }

        public static DateTime GetEnd(this DateTime holiday)
        {
            switch (holiday.DayOfWeek)
            {
                case DayOfWeek.Monday: return holiday.Subtract(TimeSpan.FromDays(2));
                case DayOfWeek.Tuesday: return holiday.Subtract(TimeSpan.FromDays(3));
                case DayOfWeek.Wednesday: return holiday.Subtract(TimeSpan.FromDays(4));
                case DayOfWeek.Thursday: return holiday.Subtract(TimeSpan.FromDays(5));
                case DayOfWeek.Friday: return holiday.Subtract(TimeSpan.FromDays(6));
                case DayOfWeek.Saturday: return holiday.Subtract(TimeSpan.FromDays(7));
                case DayOfWeek.Sunday: return holiday.Subtract(TimeSpan.FromDays(8));
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static string ToDisplayTime(this TimeSpan time)
        {
            return time.ToString(@"hh\:mm");
        }
    }
}