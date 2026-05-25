using System.Globalization;

namespace Covenant.Common.Utils.Extensions;

public static class DateExtensions
{
    public static int GetWeekOfYearStartSunday(this DateTime dateTime) =>
        CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay,
            DayOfWeek.Sunday);

    public static int GetNumberOfWeeksIn(this DateTime start, DateTime end)
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

    public static DateTime GetWeekEndingCurrentWeek(this DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Sunday => date.AddDays(6),
            DayOfWeek.Monday => date.AddDays(5),
            DayOfWeek.Tuesday => date.AddDays(4),
            DayOfWeek.Wednesday => date.AddDays(3),
            DayOfWeek.Thursday => date.AddDays(2),
            DayOfWeek.Friday => date.AddDays(1),
            DayOfWeek.Saturday => date.AddDays(0),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static DateTime GetPaymentDateForExternalWorkers(this DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Sunday => date.AddDays(12),
            DayOfWeek.Monday => date.AddDays(11),
            DayOfWeek.Tuesday => date.AddDays(10),
            DayOfWeek.Wednesday => date.AddDays(9),
            DayOfWeek.Thursday => date.AddDays(8),
            DayOfWeek.Friday => date.AddDays(7),
            DayOfWeek.Saturday => date.AddDays(6),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static DateTime GetPaymentDateForInternalWorkers(this DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Sunday => date.AddDays(5),
            DayOfWeek.Monday => date.AddDays(4),
            DayOfWeek.Tuesday => date.AddDays(3),
            DayOfWeek.Wednesday => date.AddDays(2),
            DayOfWeek.Thursday => date.AddDays(1),
            DayOfWeek.Friday => date.AddDays(0),
            DayOfWeek.Saturday => date.AddDays(-1),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static IEnumerable<DateTime> GetRangeOfDaysWorkerMustWorkToReceiveHolidayPay(this DateTime holiday)
    {
        DateTime oneDayAfterTheHoliday = holiday.AddDays(1);
        TimeSpan oneDay = TimeSpan.FromDays(1);
        DateTime oneDayBeforeTheHoliday = holiday.Subtract(oneDay);
        return [holiday, oneDayAfterTheHoliday, oneDayBeforeTheHoliday];
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
        return holiday.DayOfWeek switch
        {
            DayOfWeek.Monday => holiday.Subtract(TimeSpan.FromDays(2)),
            DayOfWeek.Tuesday => holiday.Subtract(TimeSpan.FromDays(3)),
            DayOfWeek.Wednesday => holiday.Subtract(TimeSpan.FromDays(4)),
            DayOfWeek.Thursday => holiday.Subtract(TimeSpan.FromDays(5)),
            DayOfWeek.Friday => holiday.Subtract(TimeSpan.FromDays(6)),
            DayOfWeek.Saturday => holiday.Subtract(TimeSpan.FromDays(7)),
            DayOfWeek.Sunday => holiday.Subtract(TimeSpan.FromDays(1)),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}