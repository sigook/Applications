using Covenant.Common.Models.Accounting.PayStub;
using Xunit;

namespace Covenant.Tests.Billing.Holidays
{
    public class TestParamsToGetRegularWagesWorker
    {
        [Fact]
        public void Create()
        {
            var holiday = new DateTime(2020, 06, 29);
            var workerProfileId = Guid.NewGuid();
            var agencyId = Guid.NewGuid();
            var sub = new ParamsToGetRegularWages(workerProfileId, holiday);
            Assert.Equal(workerProfileId, sub.ProfileId);
            Assert.Equal(holiday, sub.Holiday);
            Assert.Equal(new DateTime(2020, 05, 31), sub.Start);
            Assert.Equal(new DateTime(2020, 06, 27), sub.End);
            Assert.Equal(new[]
            {
                holiday,
                new DateTime(2020,06,30),
                new DateTime(2020,06,28)
            }, sub.RangeOfDaysWorkerMustWorkToReceiveHolidayPay);
        }
    }
}