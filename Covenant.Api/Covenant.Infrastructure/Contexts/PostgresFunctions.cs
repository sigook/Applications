using Covenant.Common.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Contexts;

public static class PostgresFunctions
{
    public static void AddPostgresFunctions(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(() => get_week_start_sunday(default));
    }

    [DbFunction("get_week_start_sunday")]
    public static int get_week_start_sunday(DateTime date) => date.GetWeekOfYearStartSunday();
}