using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Xunit;

namespace Covenant.Tests.Billing
{
    public class InvoiceUSATest
    {
        [Fact]
        public void Create()
        {
            const long invoiceNumberId = 548;
            var companyProfileId = Guid.NewGuid();
            var invoiceDate = new DateTime(2019, 01, 01);
            var items = new[] { new InvoiceUSAItem(5, 5, "Regular") };
            var discounts = new[] { new InvoiceUSADiscount(4, 4, "Error Missing hours") };
            var result = InvoiceUSA.Create(invoiceNumberId, invoiceDate, companyProfileId, items, discounts, new ProvinceTaxModel { Tax1 = 0.06m });
            Assert.True(result);
            Assert.Equal(invoiceNumberId, result.Value.InvoiceNumberId);
            Assert.Equal("US-0548-19", result.Value.InvoiceNumber);
            Assert.Equal(companyProfileId, result.Value.CompanyProfileId);
            Assert.Equal(invoiceDate, result.Value.CreatedAt.Date);
            Assert.Equal(items, result.Value.Items);
            Assert.Equal(discounts, result.Value.Discounts);
        }

        [Fact]
        public void SubtotalCannotBeNegative()
        {
            var discounts = new[] { new InvoiceUSADiscount(4, 4, "Error Missing hours") };
            var result = InvoiceUSA.Create(default, default, default, Array.Empty<InvoiceUSAItem>(), discounts, new ProvinceTaxModel { Tax1 = 0.06m });
            Assert.False(result);
            Assert.Equal($"Subtotal cannot be a negative value -16", result.Errors.Single().Message);
        }
    }
}