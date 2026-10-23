using Covenant.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Extensions;

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
}
