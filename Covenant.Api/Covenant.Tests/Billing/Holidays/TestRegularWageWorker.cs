using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Core.BL.Services.Shared;
using Xunit;

namespace Covenant.Tests.Billing.Holidays
{
    public class TestRegularWageWorker
    {
        // ResolveHolidayPay only reads the RegularWageWorker argument, so the service
        // dependencies are not exercised here.
        private static readonly TimesheetCalculatorService Calculator = new(null, null, null, null);

        [Fact]
        public void Uses_Regular_Formula_When_Entitled()
        {
            var wages = new RegularWageWorker
            {
                RegularWage = 10,
                HolidayWasPaid = false,
                CustomPublicHolidayValue = default,
                IsEntitledToReceiveHolidayPay = true
            };
            var (amount, description) = Calculator.ResolveHolidayPay(wages);
            Assert.Equal(0.5m, amount);
            Assert.NotEmpty(description);
        }

        [Fact]
        public void Returns_Zero_When_Holiday_Was_Paid()
        {
            var (amount, description) = Calculator.ResolveHolidayPay(new RegularWageWorker { HolidayWasPaid = true });
            Assert.Equal(0, amount);
            Assert.NotEmpty(description);
        }

        [Fact]
        public void Returns_Zero_When_Not_Entitled()
        {
            var (amount, description) = Calculator.ResolveHolidayPay(new RegularWageWorker { IsEntitledToReceiveHolidayPay = false });
            Assert.Equal(0, amount);
            Assert.NotEmpty(description);
        }

        [Fact]
        public void Returns_Custom_Value_When_Present()
        {
            const int customPublicHolidayValue = 99;
            var (amount, description) = Calculator.ResolveHolidayPay(new RegularWageWorker
            {
                IsEntitledToReceiveHolidayPay = true,
                CustomPublicHolidayValue = customPublicHolidayValue
            });
            Assert.Equal(customPublicHolidayValue, amount);
            Assert.NotEmpty(description);
        }
    }
}
