using Covenant.Common.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Contexts;

public static class PostgresFunctions
{
    public static void AddPostgresFunctions(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(() => get_week_start_sunday(default));
        modelBuilder.HasDbFunction(() => unaccent_lower(default));
        modelBuilder.HasDbFunction(() => word_similarity(default, default));
    }

    [DbFunction("get_week_start_sunday")]
    public static int get_week_start_sunday(DateTime date) => date.GetWeekOfYearStartSunday();

    [DbFunction("unaccent_lower")]
    public static string unaccent_lower(string value) => value?.RemoveDiacritics().ToLowerInvariant();

    [DbFunction("word_similarity")]
    public static double word_similarity(string term, string value) => throw new NotSupportedException();
}
