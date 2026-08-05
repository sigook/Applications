using Covenant.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Common.Utils.Extensions;

public static class PaginatedListExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedList<T>(this IQueryable<T> source, Pagination pagination)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pagination.PageIndex - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();
        return new PaginatedList<T>
        {
            PageIndex = pagination.PageIndex,
            TotalPages = (int)Math.Ceiling(count / (double)pagination.PageSize),
            Items = items,
            TotalItems = count
        };
    }

    public static async Task<PaginatedList<T>> ToPaginatedList<T>(this IEnumerable<T> source, Pagination pagination)
    {
        var items = source.Skip((pagination.PageIndex - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
        var count = source.Count();
        var result = new PaginatedList<T>
        {
            PageIndex = pagination.PageIndex,
            TotalPages = (int)Math.Ceiling(count / (double)pagination.PageSize),
            Items = [.. items],
            TotalItems = count
        };
        await Task.CompletedTask;
        return result;
    }
}
