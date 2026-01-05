using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Xunit;

namespace Covenant.Tests.Billing.PayStubTest
{
    public class TestPayStubItem
    {
        [Fact]
        public void CreateItem()
        {
            const string description = "Description";
            const int quantity = 1;
            const int unitPrice = 100;
            const PayStubItemType type = PayStubItemType.Regular;
            Result<PayStubItem> result = PayStubItem.CreateItem(description, quantity, unitPrice, type);
            Assert.True(result);
            Assert.Equal(description, result.Value.Description);
            Assert.Equal(quantity, result.Value.Quantity);
            Assert.Equal(unitPrice, result.Value.UnitPrice);
            Assert.Equal(type, result.Value.Type);
        }

        [Fact]
        public void CreateItem_Description_Is_Required()
        {
            Result<PayStubItem> result = PayStubItem.CreateItem(default, default, default, default);
            Assert.False(result);
            Assert.Equal(PayStubItem.DescriptionIsRequired, result.Errors.Single().Message);
        }

        [Fact]
        public void CreateItem_Quantity_Must_Be_Greater_Than_Zero()
        {
            Result<PayStubItem> result = PayStubItem.CreateItem("Description", default, default, default);
            Assert.False(result);
            Assert.EndsWith(PayStubItem.QuantityMustBeGraterThanZero, result.Errors.Single().Message);
        }

        [Fact]
        public void CreateItem_Unit_Price_Must_Be_Greater_Than_Zero()
        {
            Result<PayStubItem> result = PayStubItem.CreateItem("Description", 1, default, default);
            Assert.False(result);
            Assert.EndsWith(PayStubItem.UnitPriceMustBeGreaterThanZero, result.Errors.Single().Message);
        }

        [Fact]
        public void CreateItem_BonusOthers_Has_Default_Description()
        {
            Result<PayStubItem> result = PayStubItem.CreateBonusOthersItem(1, default);
            Assert.True(result);
            Assert.Equal(PayStubItem.BonusOthersLabel, result.Value.Description);
        }
    }
}