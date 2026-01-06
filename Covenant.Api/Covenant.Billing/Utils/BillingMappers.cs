using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Request;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Utils.Extensions;
using Covenant.TimeSheetTotal.Models;

namespace Covenant.Billing.Utils
{
    public static class BillingMappers
    {
        public static InvoiceTotal ToInvoiceTotal(this TotalDailyPrice source, ITimeSheetTotal timeSheetTotal) =>
            new InvoiceTotal
            {
                TimeSheetTotalId = timeSheetTotal.Id,
                AgencyRate = source.AgencyRate,
                Regular = source.Regular.DefaultMoneyRound(),
                OtherRegular = source.OtherRegular.DefaultMoneyRound(),
                Missing = source.Missing.DefaultMoneyRound(),
                MissingOvertime = source.MissingOvertime.DefaultMoneyRound(),
                NightShift = source.NightShift.DefaultMoneyRound(),
                Holiday = source.Holiday.DefaultMoneyRound(),
                Overtime = source.Overtime.DefaultMoneyRound(),
                TotalGross = source.TotalGross.DefaultMoneyRound(),
                TotalNet = source.TotalNet.DefaultMoneyRound(),
                TimeSheetTotal = timeSheetTotal is Covenant.Common.Entities.Request.TimeSheetTotal total
                    ? total
                    : new Covenant.Common.Entities.Request.TimeSheetTotal(timeSheetTotal)
            };

        public static List<InvoiceDiscount> ToInvoiceDiscounts(this IEnumerable<CreateInvoiceItemModel> source)
        {
            if (source is null) throw new ArgumentNullException();
            return source.Select(s => s.ToInvoiceDiscount()).ToList();
        }

        public static IEnumerable<InvoiceUSADiscount> ToInvoiceUSADiscounts(this IEnumerable<CreateInvoiceItemModel> source)
        {
            if (source is null) throw new ArgumentNullException();
            return source.Select(s => new InvoiceUSADiscount(s.Quantity, s.UnitPrice, s.Description));
        }

        private static InvoiceDiscount ToInvoiceDiscount(this CreateInvoiceItemModel model)
        {
            if (model is null) throw new ArgumentNullException();
            return new InvoiceDiscount(model.Quantity, model.UnitPrice, model.Description);
        }

        public static List<InvoiceAdditionalItem> ToInvoiceAdditionalItems(this IEnumerable<CreateInvoiceItemModel> source)
        {
            if (source is null) throw new ArgumentNullException();
            return source.Select(s => s.ToInvoiceAdditionalItem()).ToList();
        }

        public static IEnumerable<InvoiceUSAItem> ToInvoiceUSAAdditionalItems(this IEnumerable<CreateInvoiceItemModel> source)
        {
            if (source is null) throw new ArgumentNullException();
            return source.Select(s => new InvoiceUSAItem(s.Quantity, s.UnitPrice, s.Description));
        }

        public static InvoiceAdditionalItem ToInvoiceAdditionalItem(this CreateInvoiceItemModel source)
        {
            if (source is null) throw new ArgumentNullException();
            return new InvoiceAdditionalItem(source.Quantity, source.UnitPrice, source.Description);
        }

        public static IEnumerable<InvoiceUSAItem> ToInvoiceUSAItems(this IEnumerable<InvoiceSummaryItemModel> items) =>
            items.Select(s => new InvoiceUSAItem(s.Quantity, s.UnitPrice, s.Description));

        public static InvoicePreviewModel ToInvoicePreview(this Invoice invoice, IEnumerable<TimeSheetApprovedBillingModel> timeSheet)
        {
            var preview = new InvoicePreviewModel
            {
                SubTotal = invoice.SubTotal,
                Hst = invoice.Hst,
                Total = invoice.TotalNet,
                Discounts = invoice.Discounts.Select(d => new InvoiceSummaryItemModel(d.Description, d.Quantity, d.UnitPrice, d.Amount)).ToList()
            };
            preview.Items.AddRange((from it in invoice.InvoiceTotals
                                    join ts in timeSheet on it.TimeSheetTotal.TimeSheetId equals ts.TimeSheetId
                                    select new InvoiceItemTotalModel
                                    {
                                        RequestId = ts.RequestId,
                                        JobTitle = ts.JobTitle,
                                        Regular = it.Regular,
                                        OtherRegular = it.OtherRegular,
                                        MissingRateAgency = ts.MissingRateAgency,
                                        Missing = it.Missing,
                                        MissingOvertime = it.MissingOvertime,
                                        Holiday = it.Holiday,
                                        Overtime = it.Overtime,
                                        RegularHours = it.TimeSheetTotal.RegularHours.TotalHours,
                                        OtherRegularHours = it.TimeSheetTotal.OtherRegularHours.TotalHours,
                                        MissingHours = ts.MissingHours.TotalHours,
                                        MissingHoursOvertime = ts.MissingHoursOvertime.TotalHours,
                                        HolidayHours = it.TimeSheetTotal.HolidayHours.TotalHours,
                                        OvertimeHours = it.TimeSheetTotal.OvertimeHours.TotalHours
                                    }).ToList().ToInvoiceItems());

            preview.Items.AddRange(invoice.Holidays.Select(h =>
                new InvoiceSummaryItemModel($"Charge for Holiday {h.Holiday:D}", h.Hours, h.Amount)));
            preview.Items.AddRange(invoice.AdditionalItems.Select(a =>
                new InvoiceSummaryItemModel(a.Description, a.Quantity, a.UnitPrice, a.Total)));
            return preview;
        }

        public static InvoicePreviewModel ToInvoicePreview(this InvoiceUSA invoice)
        {
            var preview = new InvoicePreviewModel
            {
                SubTotal = invoice.SubTotal,
                Hst = invoice.Tax,
                Total = invoice.TotalNet,
                Discounts = invoice.Discounts.Select(d =>
                   new InvoiceSummaryItemModel(d.Description, d.Quantity, d.UnitPrice, d.Total)).ToList(),
            };
            preview.Items.AddRange(invoice.Items.Select(s => new InvoiceSummaryItemModel(s.Description, s.Quantity, s.UnitPrice, s.Total)));
            return preview;
        }
    }
}