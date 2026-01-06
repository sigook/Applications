using Covenant.Common.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Context;

public static class PostgresFunctions
{
    public static void AddPostgresFunctions(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(() => date_trunc("week", DateTime.MinValue));
        modelBuilder.HasDbFunction(() => get_week_start_sunday(default));
    }

    [DbFunction("date_trunc")]
    public static DateTime date_trunc(string text, DateTime time)
    {
        // Provide a fallback implementation for in-memory database / client-side evaluation
        return text.ToLower() switch
        {
            "week" => time.Date.AddDays(-(int)time.DayOfWeek),
            "day" => time.Date,
            "month" => new DateTime(time.Year, time.Month, 1),
            "year" => new DateTime(time.Year, 1, 1),
            _ => time.Date
        };
    }

    [DbFunction("get_week_start_sunday")]
    public static int get_week_start_sunday(DateTime date) => date.GetWeekOfYearStartSunday();
}