using Covenant.Common.Configuration;
using Covenant.Common.Models.Requests.TimeSheets;
using Covenant.Common.Repositories.Accounting.Deductions;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories.Workers;
using Covenant.Core.BL.Services.Accounting;
using Moq;
using Xunit;

namespace Covenant.Tests.Accounting;

public class TimesheetCalculatorServiceBreakTest
{
    private static readonly DateTime Day = new(2026, 10, 05);
    private static readonly TimeSpan Break = TimeSpan.FromMinutes(30);

    private readonly TimesheetCalculatorService _sut = new(
        Mock.Of<IDeductionsRepository>(),
        Mock.Of<IWorkerRepository>(),
        Mock.Of<ITimesheetRepository>(),
        Rates.DefaultRates,
        TimeLimits.DefaultTimeLimits);

    [Fact]
    public void Paid_Break_Keeps_The_Whole_Shift_As_Paid_Hours()
    {
        var accumulated = TimeSpan.Zero;

        var breakdown = Calculate(Day.AddHours(8.5), Break, breakIsPaid: true, ref accumulated);

        Assert.Equal(TimeSpan.FromHours(8.5), breakdown.RegularHours);
        Assert.Equal(TimeSpan.FromHours(8.5), accumulated);
    }

    [Fact]
    public void Unpaid_Break_Is_Deducted_From_Paid_Hours()
    {
        var accumulated = TimeSpan.Zero;

        var breakdown = Calculate(Day.AddHours(8.5), Break, breakIsPaid: false, ref accumulated);

        Assert.Equal(TimeSpan.FromHours(8), breakdown.RegularHours);
        Assert.Equal(TimeSpan.FromHours(8), accumulated);
    }

    [Fact]
    public void Shift_Shorter_Than_The_Break_Yields_Zero_Hours_Not_Negative()
    {
        var accumulated = TimeSpan.Zero;

        var breakdown = Calculate(Day, Break, breakIsPaid: false, ref accumulated, isHoliday: true);

        Assert.Equal(TimeSpan.Zero, breakdown.HolidayHours);
        Assert.Equal(TimeSpan.Zero, breakdown.RegularHours);
        Assert.Equal(TimeSpan.Zero, accumulated);
    }

    private TimeSheetHoursBreakdown Calculate(
        DateTime timeOut,
        TimeSpan durationBreak,
        bool breakIsPaid,
        ref TimeSpan accumulated,
        bool isHoliday = false) =>
        _sut.CalculateHoursBreakdown(
            Day,
            timeOut,
            durationBreak,
            breakIsPaid,
            isHoliday,
            holidayIsPaid: true,
            ref accumulated,
            TimeLimits.DefaultTimeLimits.MaxHoursWeek,
            TimeLimits.DefaultTimeLimits.MaxHoursWeek);
}
