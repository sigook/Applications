using Covenant.Billing.Utils;
using Covenant.Common.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Utils.Extensions;
using Covenant.TimeSheetTotal.Models;
using Xunit;

namespace Covenant.Tests.Billing
{
    public class TestBillingMappers
    {
        [Fact]
        public void ToInvoiceTotal()
        {
            var tst = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotal(Guid.NewGuid(), TimeSpan.FromHours(1),
                TimeSpan.FromHours(2), TimeSpan.FromHours(3), TimeSpan.FromHours(4),
                TimeSpan.FromHours(5));
            var source = new TotalDailyPrice(Rates.DefaultRates, 1, 2, TimeSpan.FromHours(11),
                TimeSpan.FromHours(12), TimeSpan.FromHours(13), TimeSpan.FromHours(14), TimeSpan.FromHours(15),
                TimeSpan.FromHours(15), TimeSpan.FromHours(16));
            var result = source.ToInvoiceTotal(tst);
            Assert.Equal(tst.Id, result.TimeSheetTotalId);
            Assert.Equal(source.AgencyRate, result.AgencyRate);
            Assert.Equal(source.Regular, result.Regular);
            Assert.Equal(source.OtherRegular, result.OtherRegular);
            Assert.Equal(source.Missing, result.Missing);
            Assert.Equal(source.MissingOvertime, result.MissingOvertime);
            Assert.Equal(source.NightShift, result.NightShift);
            Assert.Equal(source.Holiday, result.Holiday);
            Assert.Equal(source.Overtime, result.Overtime);
            Assert.Equal(source.TotalGross, result.TotalGross);
            Assert.Equal(source.TotalNet, result.TotalNet);
            Assert.Equal(tst, result.TimeSheetTotal);
        }

        [Fact]
        public void ToInvoiceDiscounts()
        {
            var source = new List<CreateInvoiceItemModel>
            {
                new CreateInvoiceItemModel(1, 100, $"Description {Guid.NewGuid()}")
            };
            var result = source.ToInvoiceDiscounts();
            Assert.Equal(source.Count, result.Count);
            foreach (var s in source)
            {
                Assert.Contains(result, r => r.Description == s.Description && r.Quantity.Equals(s.Quantity) && r.UnitPrice == s.UnitPrice);
            }
        }

        [Fact]
        public void ToInvoiceAdditionalItems()
        {
            var source = new List<CreateInvoiceItemModel>
            {
                new CreateInvoiceItemModel(1, 2, $"Description {Guid.NewGuid()}")
            };
            var result = source.ToInvoiceAdditionalItems();
            Assert.Equal(source.Count, result.Count);
            foreach (var s in source)
            {
                Assert.Contains(result, r => r.Description == s.Description && r.Quantity.Equals(s.Quantity) && r.UnitPrice.Equals(s.UnitPrice) && r.Total.Equals(s.Total));
            }
        }

        [Fact]
        public void ToInvoicePreview()
        {
            var timeSheet = new TimeSheetApprovedBillingModel
            {
                PaidHolidays = default,
                OvertimeStartsAfter = default,
                RequestId = Guid.NewGuid(),
                JobTitle = "General Labour",
                AgencyRate = default,
                BreakIsPaid = default,
                DurationBreak = default,
                HolidayIsPaid = default,
                WorkerId = default,
                TimeSheetId = default,
                Week = default,
                Date = default,
                TimeInApproved = default,
                TimeOutApproved = default,
                MissingHours = default,
                MissingHoursOvertime = default,
                MissingRateAgency = default,
                IsHoliday = default
            };

            var invoice = new Invoice(Guid.NewGuid(), default, default, default,
                default, default, default, default, 10, 20, 30);
            invoice.AddDiscounts(new[] { new InvoiceDiscount(5, 15, "Discount") });
            invoice.AddHolidays(new[] { InvoiceHoliday.Create(new DateTime(2021, 01, 01), 13, 400.01m).Value });
            invoice.AddAdditionalItems(new[] { new InvoiceAdditionalItem(7, 8, "Missing Hours") });
            var invoiceTotal = new InvoiceTotal
            {
                TimeSheetTotalId = timeSheet.TimeSheetId,
                AgencyRate = 22,
                Regular = 25,
                OtherRegular = 5,
                Missing = 34,
                MissingOvertime = 43,
                NightShift = default,
                Holiday = 54,
                Overtime = 75.99m,
                TotalGross = 80,
                TotalNet = 80,
                TimeSheetTotal = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotal(
                    timeSheet.TimeSheetId, TimeSpan.FromHours(1), TimeSpan.FromHours(2), TimeSpan.FromHours(3), default, default)
            };
            invoice.AddInvoiceTotals(new[] { invoiceTotal });

            var preview = invoice.ToInvoicePreview(new[] { timeSheet });
            Assert.Equal(invoice.SubTotal, preview.SubTotal);
            Assert.Equal(invoice.Hst, preview.Hst);
            Assert.Equal(invoice.TotalNet, preview.Total);
            foreach (InvoiceDiscount discount in invoice.Discounts)
            {
                Assert.Contains(preview.Discounts, pd =>
                    pd.Quantity.Equals(discount.Quantity) &&
                    pd.UnitPrice.Equals(discount.UnitPrice) &&
                    pd.Description.Equals(discount.Description) &&
                    pd.Total.Equals(discount.Amount));
            }

            foreach (InvoiceHoliday holiday in invoice.Holidays)
            {
                Assert.Contains(preview.Items, ih =>
                    ih.Quantity.Equals(holiday.Hours) &&
                    ih.UnitPrice.Equals(holiday.UnitPrice) &&
                    ih.Total.Equals(holiday.Amount) &&
                    !string.IsNullOrEmpty(ih.Description));
            }

            foreach (InvoiceAdditionalItem aItem in invoice.AdditionalItems)
            {
                Assert.Contains(preview.Items, ai =>
                    ai.Quantity.Equals(aItem.Quantity) &&
                    ai.UnitPrice.Equals(aItem.UnitPrice) &&
                    ai.Total.Equals(aItem.Total) &&
                    ai.Description.Equals(aItem.Description));
            }

            var regular = preview.Items.Single(r =>
                r.Description.Equals(IEnumerableExtensions.Description(timeSheet.JobTitle, CovenantConstants.Invoice.RegularLabel)));
            Assert.Equal(invoiceTotal.RegularPlusOtherRegular, regular.Total);
            var overtime = preview.Items.Single(r =>
                r.Description.Equals(IEnumerableExtensions.Description(timeSheet.JobTitle, CovenantConstants.Invoice.OvertimeLabel)));
            Assert.Equal(invoiceTotal.Overtime, overtime.Total);
        }
    }
}