using Covenant.Common.Models.Accounting.PayStub;
using Xunit;

namespace Covenant.Tests.Billing.Holidays
{
    public class TestRegularWageWorker
    {
        [Fact]
        public void Create()
        {
            const int vacations = 10;
            const int regularWage = 10;
            var sub = new RegularWageWorker
            {
                Vacations = vacations,
                RegularWage = regularWage,
                HolidayWasPaid = false,
                CustomPublicHolidayValue = default,
                IsEntitledToReceiveHolidayPay = true
            };
            Assert.Equal(vacations, sub.Vacations);
            Assert.Equal(regularWage, sub.RegularWage);
            Assert.Equal(default, sub.CustomPublicHolidayValue);
            Assert.False(sub.HolidayWasPaid);
            Assert.True(sub.IsEntitledToReceiveHolidayPay);
            Assert.Equal(vacations + regularWage, sub.RegularWagePlusVacations);
            Assert.Equal(1, sub.AmountToPay);
            Assert.NotEmpty(sub.Description);
        }

        [Fact]
        public void Holiday_Was_Paid()
        {
            var sub = new RegularWageWorker
            {
                HolidayWasPaid = true
            };
            Assert.True(sub.HolidayWasPaid);
            Assert.Equal(0, sub.AmountToPay);
            Assert.NotEmpty(sub.Description);
        }

        [Fact]
        public void Is_Not_Entitled_To_Receive_Holiday_Pay()
        {
            var sub = new RegularWageWorker
            {
                IsEntitledToReceiveHolidayPay = false
            };
            Assert.False(sub.IsEntitledToReceiveHolidayPay);
            Assert.Equal(0, sub.AmountToPay);
            Assert.NotEmpty(sub.Description);
        }

        [Fact]
        public void Holiday_Custom_Value()
        {
            const int customPublicHolidayValue = 99;
            var sub = new RegularWageWorker
            {
                IsEntitledToReceiveHolidayPay = true,
                CustomPublicHolidayValue = customPublicHolidayValue
            };
            Assert.Equal(customPublicHolidayValue, sub.AmountToPay);
            Assert.NotEmpty(sub.Description);
        }
    }
}