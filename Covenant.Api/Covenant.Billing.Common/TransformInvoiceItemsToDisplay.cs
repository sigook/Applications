using Covenant.Common.Models.Accounting.Invoice;

namespace Covenant.Billing.Common
{
    public static class TransformInvoiceItemsToDisplay
    {
        public const string RegularLabel = "Regular";
        public const string MissingDifferentRateLabel = "Missing different rate";
        public const string MissingOvertimeDifferentRateLabel = "Missing Overtime different rate";
        public const string MissingLabel = "Missing";
        public const string MissingOvertimeLabel = "Missing Overtime";
        public const string NightShiftLabel = "Night shift";
        public const string HolidayLabel = "Holiday";
        public const string OvertimeLabel = "Overtime";

        public static string Description(string jobTitle, string label) => $"Charge for {jobTitle} / {label}";

        public static List<InvoiceSummaryItemModel> ToInvoiceItems(this IEnumerable<InvoiceItemTotalModel> items)
        {
            return items.GroupBy(r => r.RequestId).SelectMany((r, i) =>
            {
                var result = new List<InvoiceSummaryItemModel>();
                string jobTitle = r.First().JobTitle;
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, RegularLabel), r.Sum(a => a.RegularHours + a.OtherRegularHours), r.Sum(a => a.Regular + a.OtherRegular)));
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, OvertimeLabel), r.Sum(a => a.OvertimeHours), r.Sum(a => a.Overtime)));
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, HolidayLabel), r.Sum(a => a.HolidayHours), r.Sum(a => a.Holiday)));

                List<InvoiceItemTotalModel> missingSameRate = r.Where(a =>
                    a.MissingRateAgency <= decimal.Zero && (a.MissingHours > 0 || a.MissingOvertime > 0)).ToList();
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, MissingLabel), missingSameRate.Sum(m => m.MissingHours), missingSameRate.Sum(a => a.Missing)));
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, MissingOvertimeLabel), missingSameRate.Sum(m => m.MissingHoursOvertime), missingSameRate.Sum(a => a.MissingOvertime)));

                List<InvoiceItemTotalModel> differentRate = r.Where(a => a.MissingRateAgency > decimal.Zero).ToList();//If is greater than zero is because it has a different rate
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, MissingDifferentRateLabel), differentRate.Sum(a => a.MissingHours), differentRate.Sum(a => a.Missing)));
                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, MissingOvertimeDifferentRateLabel), differentRate.Sum(a => a.MissingHoursOvertime), differentRate.Sum(a => a.MissingOvertime)));

                result.Add(new InvoiceSummaryItemModel(Description(jobTitle, NightShiftLabel), r.Sum(a => a.NightShiftHours), r.Sum(a => a.NightShift)));

                return result.Where(m => m.Total > decimal.Zero);
            }).ToList();
        }
    }
}