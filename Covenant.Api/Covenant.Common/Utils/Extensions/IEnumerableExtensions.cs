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

        public static List<InvoiceSummaryItemModel> ToInvoiceItems(this IEnumerable<InvoiceItemTotalModel> items)
        {
            return items.GroupBy(r => r.RequestId).SelectMany((r, i) =>
            {
                var result = new List<InvoiceSummaryItemModel>();
                string jobTitle = r.First().JobTitle;
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, CovenantConstants.Invoice.RegularLabel), r.Sum(a => a.RegularHours + a.OtherRegularHours), r.Sum(a => a.Regular + a.OtherRegular)));
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, CovenantConstants.Invoice.OvertimeLabel), r.Sum(a => a.OvertimeHours), r.Sum(a => a.Overtime)));
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, CovenantConstants.Invoice.HolidayLabel), r.Sum(a => a.HolidayHours), r.Sum(a => a.Holiday)));

                List<InvoiceItemTotalModel> missingSameRate = r.Where(a =>
                    a.MissingRateAgency <= decimal.Zero && (a.MissingHours > 0 || a.MissingOvertime > 0)).ToList();
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, CovenantConstants.Invoice.MissingLabel), missingSameRate.Sum(m => m.MissingHours), missingSameRate.Sum(a => a.Missing)));
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, CovenantConstants.Invoice.MissingOvertimeLabel), missingSameRate.Sum(m => m.MissingHoursOvertime), missingSameRate.Sum(a => a.MissingOvertime)));

                List<InvoiceItemTotalModel> differentRate = r.Where(a => a.MissingRateAgency > decimal.Zero).ToList();//If is greater than zero is because it has a different rate
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, CovenantConstants.Invoice.MissingDifferentRateLabel), differentRate.Sum(a => a.MissingHours), differentRate.Sum(a => a.Missing)));
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, CovenantConstants.Invoice.MissingOvertimeDifferentRateLabel), differentRate.Sum(a => a.MissingHoursOvertime), differentRate.Sum(a => a.MissingOvertime)));

                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, CovenantConstants.Invoice.NightShiftLabel), r.Sum(a => a.NightShiftHours), r.Sum(a => a.NightShift)));

                return result.Where(m => m.Total > decimal.Zero);
            }).ToList();
        }

        public static string Description(string jobTitle, string label) => $"Charge for {jobTitle} / {label}";
    }
}
