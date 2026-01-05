using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Xunit;

namespace Covenant.Tests.Request
{
    public class TimeSheetTest
    {
        [Fact]
        public void AgencyCreateTimeSheet()
        {
            var workerRequestId = Guid.NewGuid();
            DateTime date = DateTime.Now;
            TimeSpan hours = TimeSpan.FromHours(8);
            Result<TimeSheet> result = TimeSheet.CreateTimeSheet(workerRequestId, date, hours);
            Assert.True(result);
            Assert.Equal(workerRequestId, result.Value.WorkerRequestId);
            Assert.Equal(date.Date, result.Value.Date);
            Assert.Equal(date.Date, result.Value.TimeIn);
            Assert.Equal(date.Date.AddHours(hours.TotalHours), result.Value.TimeOut);
            Assert.Equal(hours.TotalHours, result.Value.TotalHours);
            Assert.False(result.Value.IsHoliday);
            Assert.Equal(date.Date, result.Value.TimeInApproved);
            Assert.Equal(date.Date.AddHours(hours.TotalHours), result.Value.TimeOutApproved);
            Assert.Equal(TimeSheet.DefaultComment, result.Value.Comment);
            Assert.Equal(default, result.Value.MissingHours);
            Assert.Equal(default, result.Value.MissingHoursOvertime);
            Assert.Equal(default, result.Value.MissingRateWorker);
            Assert.Equal(default, result.Value.MissingRateAgency);
            Assert.Equal(default, result.Value.DeductionsOthers);
            Assert.Equal(default, result.Value.DeductionsOthersDescription);
            Assert.Equal(default, result.Value.BonusOrOthers);
            Assert.Equal(default, result.Value.BonusOrOthersDescription);
        }

        [Fact]
        public void AgencyCreateTimeSheetMinimumDateIsOneYearBeforeNow()
        {
            DateTime date = DateTime.Now.AddYears(-1);
            Result<TimeSheet> result = TimeSheet.CreateTimeSheet(Guid.NewGuid(), date, TimeSpan.FromHours(8));
            Assert.False(result);
            Assert.Contains("Date", result.Errors.Single().Message);

            date = date.AddDays(1);
            result = TimeSheet.CreateTimeSheet(Guid.NewGuid(), date, TimeSpan.FromHours(8));
            Assert.True(result);
        }

        [Fact]
        public void AgencyCreateTimeSheetHoursMustBeLessThan24()
        {
            TimeSpan hours = TimeSpan.FromHours(24);
            Result<TimeSheet> result = TimeSheet.CreateTimeSheet(Guid.NewGuid(), DateTime.Now, hours);
            Assert.False(result);
            Assert.Contains("hours", result.Errors.Single().Message);

            hours = hours.Subtract(TimeSpan.FromSeconds(1));
            result = TimeSheet.CreateTimeSheet(Guid.NewGuid(), DateTime.Now, hours);
            Assert.True(result);
        }

        [Fact]
        public void WorkerCreateTimeSheet()
        {
            var workerRequestId = Guid.NewGuid();
            var clockIn = new DateTime(2019, 01, 01, 08, 05, 00);
            var clockInRounded = new DateTime(2019, 01, 01, 08, 00, 00);
            const bool isHoliday = true;
            Result<TimeSheet> result = TimeSheet.WorkerClockIn(workerRequestId, clockIn, isHoliday, clockInRounded);
            Assert.True(result);
            Assert.Equal(workerRequestId, result.Value.WorkerRequestId);
            Assert.Equal(isHoliday, result.Value.IsHoliday);
            Assert.Equal(clockIn, result.Value.ClockIn);
            Assert.Equal(clockInRounded, result.Value.ClockInRounded);
            Assert.Equal(clockInRounded.Date, result.Value.TimeIn);
            Assert.Equal(clockInRounded.Date, result.Value.Date);
            Assert.Null(result.Value.ClockOut);
            Assert.Null(result.Value.ClockOutRounded);
            Assert.Null(result.Value.TimeOut);
        }

        [Fact]
        public void AddApprovedTime()
        {
            var entity = TimeSheet.CreateTimeSheet(Guid.NewGuid(), DateTime.Now, TimeSpan.FromHours(8)).Value;
            DateTime timeInApproved = entity.TimeIn.AddHours(1);
            DateTime timeOutApproved = entity.TimeOut.GetValueOrDefault().AddHours(1);
            Result result = entity.AddApprovedTime(timeInApproved, timeOutApproved);
            Assert.True(result);
            Assert.Equal(timeInApproved, entity.TimeInApproved);
            Assert.Equal(timeOutApproved, entity.TimeOutApproved);

            result = entity.AddApprovedTime(entity.TimeIn, entity.TimeIn.AddDays(1));
            Assert.False(result);
            Assert.Equal("Time in and Time out must be in the same date", result.Errors.First().Message);
            Assert.Equal(timeInApproved, entity.TimeInApproved);
            Assert.Equal(timeOutApproved, entity.TimeOutApproved);

            result = entity.AddApprovedTime(entity.TimeIn.AddHours(2), entity.TimeIn.AddHours(1));
            Assert.Equal("Time in cannot be greater than Time out", result.Errors.First().Message);
            Assert.Equal(timeInApproved, entity.TimeInApproved);
            Assert.Equal(timeOutApproved, entity.TimeOutApproved);

            result = entity.AddApprovedTime(entity.TimeIn.AddDays(1), entity.TimeIn.AddDays(1));
            Assert.False(result);
            Assert.StartsWith("Time in date must be equal to", result.Errors.First().Message);
            Assert.Equal(timeInApproved, entity.TimeInApproved);
            Assert.Equal(timeOutApproved, entity.TimeOutApproved);
        }

        [Fact]
        public void AddClockOut()
        {
            var clockIn = new DateTime(2021, 01, 01, 08, 00, 00);
            var clockOut = new DateTime(2021, 01, 01, 16, 00, 00);
            var timeSheet = TimeSheet.WorkerClockIn(Guid.NewGuid(), clockIn).Value;
            Result result = timeSheet.AddClockOut(clockOut);
            Assert.True(result);
            Assert.Equal(clockIn, timeSheet.ClockIn);
            Assert.Null(timeSheet.ClockInRounded);
            Assert.Equal(clockOut, timeSheet.ClockOut);
            Assert.Null(timeSheet.ClockOutRounded);
            Assert.Equal(clockIn.Date, timeSheet.TimeIn);
            Assert.Equal(clockIn.Date.AddHours(8), timeSheet.TimeOut);
        }

        [Fact]
        public void AddClockOutRounded()
        {
            var clockIn = new DateTime(2021, 01, 01, 08, 05, 00);
            var clockInRounded = new DateTime(2021, 01, 01, 08, 00, 00);
            var clockOut = new DateTime(2021, 01, 01, 16, 08, 00);
            var clockOutRounded = new DateTime(2021, 01, 01, 16, 00, 00);
            var timeSheet = TimeSheet.WorkerClockIn(Guid.NewGuid(), clockIn, clockInRounded: clockInRounded).Value;
            Result result = timeSheet.AddClockOut(clockOut, clockOutRounded);
            Assert.True(result);
            Assert.Equal(clockIn, timeSheet.ClockIn);
            Assert.Equal(clockInRounded, timeSheet.ClockInRounded);
            Assert.Equal(clockOut, timeSheet.ClockOut);
            Assert.Equal(clockOutRounded, timeSheet.ClockOutRounded);
            Assert.Equal(clockInRounded.Date, timeSheet.TimeIn);
            Assert.Equal(clockInRounded.Date.AddHours(8), timeSheet.TimeOut);
        }

        [Fact]
        public void AddClockOut_WhenTotalHoursIsMoreThan1DayByDefaultSet23Hours59Minutes()
        {
            var clockIn = new DateTime(2021, 01, 01, 08, 00, 00);
            var clockOut = new DateTime(2021, 01, 02, 08, 00, 00);
            var timeSheet = TimeSheet.WorkerClockIn(Guid.NewGuid(), clockIn).Value;
            Result result = timeSheet.AddClockOut(clockOut);
            Assert.True(result);
            Assert.Equal(clockIn, timeSheet.ClockIn);
            Assert.Equal(clockOut, timeSheet.ClockOut);
            Assert.Equal(clockIn.Date, timeSheet.TimeIn);
            Assert.Equal(clockIn.Date.Add(new TimeSpan(23, 59, 00)), timeSheet.TimeOut);
        }

        [Fact]
        public void WorkerHas5MinutesToClockOutAgain()
        {
            var fakeNow = new DateTime(2019, 01, 01);
            foreach (int minutes in new[] { 3, 4, 5 })
            {
                var timeSheet = TimeSheet.WorkerClockIn(Guid.NewGuid(), fakeNow).Value;
                timeSheet.AddClockOut(fakeNow);
                Result result = timeSheet.AddClockOut(fakeNow.AddMinutes(minutes));
                Assert.True(result);
                Assert.Equal(minutes, TimeSpan.FromHours(timeSheet.TotalHours).Minutes);
            }
            foreach (int minutes in new[] { 6, 7 })
            {
                var timeSheet = TimeSheet.WorkerClockIn(Guid.NewGuid(), fakeNow).Value;
                timeSheet.AddClockOut(fakeNow.AddHours(TimeSheet.TotalOfMinutesToWaitToClockOut));
                Result result = timeSheet.AddClockOut(timeSheet.ClockOut.GetValueOrDefault().AddMinutes(minutes));
                Assert.False(result);
                Assert.Equal("You already clock out today.", result.Errors.Single().Message);
            }
        }

        [Fact]
        public void ItMustWaitAtLeast3MinutesToClockOut()
        {
            var fakeNow = new DateTime(2019, 01, 01);
            var timeSheet = TimeSheet.WorkerClockIn(Guid.NewGuid(), fakeNow).Value;
            timeSheet.AddClockOut(fakeNow);

            foreach (int minutes in new[] { 0, 1, 2 })
            {
                Result result = timeSheet.AddClockOut(fakeNow.AddMinutes(minutes));
                Assert.False(result);
                Assert.Equal("Please wait 3 minutes to clock out.", result.Errors.Single().Message);
            }

            foreach (int minutes in new[] { 3, 4 })
            {
                Result result = timeSheet.AddClockOut(fakeNow.AddMinutes(minutes));
                Assert.True(result);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Register_ClockOut_Next_Date(int extra)
        {
            var clockIn = new DateTime(2019, 01, 01, 23, 00, 00);
            var timeSheet = TimeSheet.WorkerClockIn(Guid.NewGuid(), clockIn.Date).Value;

            Result result = timeSheet.AddClockOut(clockIn.AddHours(TimeLimits.DefaultTimeLimits.MaximumHoursDay + extra));
            Assert.True(result);
            Assert.Equal(new DateTime(2019, 01, 02), timeSheet.ClockOut.GetValueOrDefault().Date);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Register_ClockOut_Same_Date(int extra)
        {
            var now = new DateTime(2019, 01, 01, 15, 00, 00);
            DateTime clockIn = now.Subtract(TimeSpan.FromHours(TimeLimits.DefaultTimeLimits.MaximumHoursDay + extra));
            var timeSheet = TimeSheet.WorkerClockIn(Guid.NewGuid(), clockIn).Value;

            Result result = timeSheet.AddClockOut(now);
            Assert.True(result);
            Assert.Equal(clockIn.Date, timeSheet.ClockOut.GetValueOrDefault().Date);
        }
    }
}