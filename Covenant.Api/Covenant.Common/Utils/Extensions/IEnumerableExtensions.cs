using Covenant.Common.Models;
using System.Linq.Expressions;

namespace Covenant.Common.Utils.Extensions;

public static class IEnumerableExtensions
{
    public static IOrderedQueryable<TEntity> AddOrderBy<T, TEntity, TFilter>(this IQueryable<TEntity> query, TFilter filter, Expression<Func<TEntity, T>> expression)
        where TFilter : Pagination
    {
        if (filter.IsDescending)
        {
            return query.OrderByDescending(expression);
        }
        else
        {
            return query.OrderBy(expression);
        }
    }
}
