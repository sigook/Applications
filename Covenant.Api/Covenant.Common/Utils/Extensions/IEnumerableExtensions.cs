using Covenant.Common.Constants;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.Invoice;
using System.Linq.Expressions;

namespace Covenant.Common.Utils.Extensions
{
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

        public static string Description(string jobTitle, string label) => $"Charge for {jobTitle} / {label}";


    }
}
