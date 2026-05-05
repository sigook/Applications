using Covenant.Common.Entities.Accounting.Invoice;
using Xunit;

namespace Covenant.Tests.Billing
{
    public class InvoiceUSATest
    {
        [Fact]
        public void ObjectInitializer_AssignsProperties()
        {
            const long invoiceNumberId = 548;
            var companyProfileId = Guid.NewGuid();
            var invoiceDate = new DateTime(2019, 01, 01);
            var items = new[] { new InvoiceUSAItem(5, 5, "Regular") };
            var discounts = new[] { new InvoiceUSADiscount(4, 4, "Error Missing hours") };

            var invoice = new InvoiceUSA
            {
                InvoiceNumberId = invoiceNumberId,
                CreatedAt = invoiceDate,
                CompanyProfileId = companyProfileId,
                Items = items,
                Discounts = discounts,
                SubTotal = 9m,
                Tax = 0.54m,
                TotalNet = 9.54m,
                InvoiceNumber = "US-0548-19"
            };

            Assert.Equal(invoiceNumberId, invoice.InvoiceNumberId);
            Assert.Equal("US-0548-19", invoice.InvoiceNumber);
            Assert.Equal(companyProfileId, invoice.CompanyProfileId);
            Assert.Equal(invoiceDate, invoice.CreatedAt.Date);
            Assert.Equal(items, invoice.Items);
            Assert.Equal(discounts, invoice.Discounts);
            Assert.Equal(9m, invoice.SubTotal);
            Assert.Equal(0.54m, invoice.Tax);
            Assert.Equal(9.54m, invoice.TotalNet);
        }
    }
}