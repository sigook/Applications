using Covenant.Common.Models.Accounting.PayStub;
using Xunit;

namespace Covenant.Tests.Billing.Holidays
{
    public class TestParamsToGetRegularChargesCompanyByWorker
    {
        [Fact]
        public void Create()
        {
            var companyProfileId = Guid.NewGuid();
            var agencyId = Guid.NewGuid();
            var holiday = new DateTime(2020, 03, 30);
            var sub = new ParamsToGetRegularWages(companyProfileId, holiday);
            Assert.Equal(companyProfileId, sub.ProfileId);
            Assert.Equal(holiday, sub.Holiday);
            Assert.Equal(new DateTime(2020, 03, 01), sub.Start);
            Assert.Equal(new DateTime(2020, 03, 28), sub.End);
            Assert.Equal(new[]{holiday,
                    new DateTime(2020,03,31),
                    new DateTime(2020,03,29)},
                sub.RangeOfDaysWorkerMustWorkToReceiveHolidayPay);
        }
    }
}