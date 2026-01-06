using Covenant.Common.Configuration;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.TimeSheetTotal.Models;
using Xunit;

namespace Covenant.Tests.Billing
{
    public class TestTimeSheetTotalMapper
    {
        [Fact]
        public void TotalizatorParams()
        {
            const int length = 4;
            var models = Enumerable.Range(1, length).Select(i =>
                new TimeSheetApprovedModel(default, default, default, default,
                    true, TimeLimits.DefaultTimeLimits.MaxHoursWeek,
                    Guid.NewGuid(), 1 + i, 2 + i, true, TimeSpan.FromHours(1 + i),
                    TimeSpan.FromHours(2 + i), true, TimeSpan.FromHours(3 + i), true, Guid.NewGuid(),
                    default, default, default, default, Guid.NewGuid(), default,
                    new DateTime(2019, 01, 01 + i),
                    new DateTime(2019, 01, 01 + i, 08, 00, 00),
                    new DateTime(2019, 01, 01 + i, 18, 00, 00),
                    TimeSpan.FromHours(4 + i), TimeSpan.FromHours(5 + i), 3 + i, 4 + i, true,
                    default, default, default, default)
            ).ToList();
            IReadOnlyCollection<TotalizatorParams> collectionResult = models.TotalizatorParams();
            Assert.Equal(4, collectionResult.Count);
            for (var i = 0; i < length; i++)
            {
                TimeSheetApprovedModel model = models.ElementAt(i);
                TotalizatorParams result = collectionResult.ElementAt(i);
                Assert.Equal(model.RequestId, result.RequestId);
                Assert.Equal(model.Date, result.Date);
                Assert.Equal(model.IsHoliday, result.IsHoliday);
                Assert.Equal(model.TimeOutApproved, result.TimeOutApproved);
                Assert.Equal(model.TimeInApproved, result.TimeInApproved);
                Assert.Equal(model.AgencyRate, result.AgencyRate);
                Assert.Equal(model.MissingRateAgency, result.MissingRateAgency);
                Assert.Equal(model.MissingHoursOvertime, result.MissingHoursOvertime);
                Assert.Equal(model.MissingHours, result.MissingHours);
                Assert.Equal(model.WorkerRate, result.WorkerRate);
                Assert.Equal(model.MissingRateWorker, result.MissingRateWorker);
                Assert.Equal(model.BreakIsPaid, result.BreakIsPaid);
                Assert.Equal(model.DurationBreak, result.DurationBreak);
                Assert.Equal(model.HolidayIsPaid, result.HolidayIsPaid);
                Assert.Equal(model.PayNightShift, result.PayNightShift);
                Assert.Equal(model.TimeStartNightShift, result.TimeStartNightShift);
                Assert.Equal(model.TimeEndNightShift, result.TimeEndNightShift);
                Assert.Equal(model.TimeSheetId, result.TimeSheetId);
            }
        }

        [Fact]
        public void ToCalculateTimeSheetTotalParams()
        {
            var source = new TotalizatorParams();
            TimeSpan maxHoursWeek = TimeSpan.FromHours(44);
            var result = source.ToCalculateTimeSheetTotalParams(maxHoursWeek, maxHoursWeek);
            Assert.Equal(source.BreakIsPaid, result.BreakIsPaid);
            Assert.Equal(source.DurationBreak, result.DurationBreak);
            Assert.Equal(source.HolidayIsPaid, result.HolidayIsPaid);
            Assert.Equal(source.PayNightShift, result.PayNightShift);
            Assert.Equal(source.TimeStartNightShift, result.TimeStartNightShift);
            Assert.Equal(source.TimeEndNightShift, result.TimeEndNightShift);
            Assert.Equal(source.TimeSheetId, result.TimeSheetId);
            Assert.Equal(source.TimeInApproved, result.TimeOutApproved);
            Assert.Equal(source.TimeOutApproved, result.TimeOutApproved);
            Assert.Equal(source.IsHoliday, result.IsHoliday);
            Assert.Equal(maxHoursWeek, result.MaxHoursWeek);
            Assert.Equal(maxHoursWeek, result.OvertimeStartsFrom);
        }
    }
}