using Covenant.Common.Constants;
using Covenant.Common.Models.Accounting.PayStub;
using Xunit;

namespace Covenant.Tests.Billing.Holidays
{
    public class TestCompanyRegularChargesByWorker
    {
        [Fact]
        public void Regular_Case()
        {
            var workerId = Guid.NewGuid();
            var workerProfileId = Guid.NewGuid();
            const int agencyRate = 1;
            const int regular = 10;
            const int otherRegular = 20;
            var sub = new CompanyRegularChargesByWorker(workerId,
                workerProfileId, agencyRate, regular, otherRegular);

            Assert.Equal(workerId, sub.WorkerId);
            Assert.Equal(workerProfileId, sub.WorkerProfileId);
            Assert.Equal(agencyRate, sub.AgencyRate);
            Assert.Equal(regular + otherRegular, sub.Regular);
            Assert.Equal(30, sub.TotalHours);
            Assert.Equal(1.5, sub.HoursToPay);
            Assert.Equal(1.5m, sub.AmountToPay);
        }

        [Fact]
        public void If_Total_Hours_Are_Greater_Than_The_Limit_Use_The_Limit()
        {
            const double limit = CovenantConstants.PublicHolidays.LimitForWorkerWorkedHours;
            decimal regular = new decimal(limit) + 1m;
            var sub = new CompanyRegularChargesByWorker(default,
                default, 1, regular, 0);
            Assert.Equal(limit, sub.TotalHours);
        }

        [Fact]
        public void If_AgencyRate_Is_Zero_Everything_IsZero()
        {
            var sub = new CompanyRegularChargesByWorker(default,
                default, decimal.Zero, default, 0);
            Assert.Equal(0, sub.TotalHours);
            Assert.Equal(0, sub.HoursToPay);
            Assert.Equal(0, sub.AmountToPay);
        }
    }
}