using Covenant.Common.Entities.Accounting.Invoice;
using Xunit;

namespace Covenant.Tests.Billing
{
    public class InvoiceDiscountTest
    {
        [Fact]
        public void Create()
        {
            const int quantity = 1;
            const int unitPrice = 1;
            const string description = "New year";
            var discount = new InvoiceDiscount(quantity, unitPrice, description);
            Assert.Equal(quantity, discount.Quantity);
            Assert.Equal(unitPrice, discount.UnitPrice);
            Assert.Equal(description, discount.Description);
            Assert.NotEqual(Guid.Empty, discount.Id);
            Assert.Equal(1, discount.Amount);
        }

        [Fact]
        public void UnitPriceIsRequired()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var discount = new InvoiceDiscount(1, default, "Christmas");
            });
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void DefaultQuantityIsOne(double quantity)
        {
            var discount = new InvoiceDiscount(quantity, 20, "");
            Assert.Equal(1, discount.Quantity);
        }
    }
}