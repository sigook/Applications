using System.Globalization;
using Covenant.Api.Validators.Company;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Company.SalesDashboard;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Moq;
using Xunit;

namespace Covenant.Tests.Sales;

public class SalesServicePeriodWindowTest
{
    private readonly Mock<ICompanyRepository> _companyRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<ITimeService> _timeService = new();
    private readonly ISalesService _sut;
    private readonly Guid _agencyId = Guid.NewGuid();

    private DateTime _capturedFrom;
    private DateTime _capturedTo;

    public SalesServicePeriodWindowTest()
    {
        _currentUserService.Setup(i => i.GetAgencyId()).Returns(_agencyId);
        _currentUserService.Setup(i => i.IsAdmin()).Returns(true);
        _companyRepository
            .Setup(r => r.GetDealsByStatus(_agencyId, It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<DealStatus>>()))
            .Callback<Guid, Guid?, DateTime, DateTime, List<DealStatus>>((_, _, from, to, _) =>
            {
                _capturedFrom = from;
                _capturedTo = to;
            })
            .ReturnsAsync([]);
        _companyRepository
            .Setup(r => r.GetInteractionsByType(_agencyId, It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync([]);
        _sut = new SalesService(
            Mock.Of<IRequestService>(),
            Mock.Of<IRequestRepository>(),
            _companyRepository.Object,
            _currentUserService.Object,
            Mock.Of<IUploadedFilesService>(),
            Mock.Of<IDocumentService>(),
            new CreateCompanyInteractionModelValidator(),
            new UpdateCompanyInteractionModelValidator(),
            new CreateDealModelValidator(),
            new UpdateDealModelValidator(),
            _timeService.Object,
            new GetDealsByStatusFilterValidator());
    }

    private static DateTimeOffset Utc(string instant) =>
        DateTimeOffset.Parse(instant, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

    private async Task<SalesPeriodRangeModel> Window(SalesPeriod period, DateTimeOffset now)
    {
        _timeService.Setup(t => t.GetCurrentDateTimeOffset()).Returns(now);
        var result = await _sut.GetDealsByStatus(new GetDealsByStatusFilter { Period = period });
        Assert.True(result);
        return result.Value.Period;
    }

    [Fact]
    public async Task DayWindowUsesTheUtcCalendarDay()
    {
        var range = await Window(SalesPeriod.Day, Utc("2026-09-13T03:30:00Z"));

        Assert.Equal(new DateTime(2026, 9, 13), range.From);
        Assert.Equal(new DateTime(2026, 9, 13), range.To);
        Assert.Equal(new DateTime(2026, 9, 13, 0, 0, 0, DateTimeKind.Utc), _capturedFrom);
        Assert.Equal(new DateTime(2026, 9, 14, 0, 0, 0, DateTimeKind.Utc), _capturedTo);
        Assert.Equal("Sep 13, 2026", range.Label);
    }

    [Fact]
    public async Task WeekWindowRunsSundayToSaturday()
    {
        var range = await Window(SalesPeriod.Week, Utc("2026-09-09T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 9, 6), range.From);
        Assert.Equal(new DateTime(2026, 9, 12), range.To);
        Assert.Equal(new DateTime(2026, 9, 6, 0, 0, 0, DateTimeKind.Utc), _capturedFrom);
        Assert.Equal(new DateTime(2026, 9, 13, 0, 0, 0, DateTimeKind.Utc), _capturedTo);
        Assert.Equal("Sep 6 - Sep 12, 2026", range.Label);
    }

    [Fact]
    public async Task WeekWindowStartsOnTheUtcSundayEvenLateAtNight()
    {
        var range = await Window(SalesPeriod.Week, Utc("2026-09-13T03:30:00Z"));

        Assert.Equal(new DateTime(2026, 9, 13), range.From);
        Assert.Equal(new DateTime(2026, 9, 19), range.To);
        Assert.Equal("Sep 13 - Sep 19, 2026", range.Label);
    }

    [Theory]
    [InlineData("2026-03-10T15:00:00Z", 2026, 3, 8)]
    [InlineData("2026-11-03T15:00:00Z", 2026, 11, 1)]
    public async Task WeekWindowIsAlwaysSevenTwentyFourHourDays(string instant, int year, int month, int day)
    {
        var range = await Window(SalesPeriod.Week, Utc(instant));

        Assert.Equal(new DateTime(year, month, day), range.From);
        Assert.Equal(TimeSpan.FromDays(7), _capturedTo - _capturedFrom);
        Assert.Equal(0, _capturedFrom.TimeOfDay.Ticks);
        Assert.Equal(0, _capturedTo.TimeOfDay.Ticks);
    }

    [Fact]
    public async Task MonthWindowCoversTheCalendarMonth()
    {
        var range = await Window(SalesPeriod.Month, Utc("2026-09-13T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 9, 1), range.From);
        Assert.Equal(new DateTime(2026, 9, 30), range.To);
        Assert.Equal(new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc), _capturedFrom);
        Assert.Equal(new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc), _capturedTo);
        Assert.Equal("September 2026", range.Label);
    }

    [Fact]
    public async Task QuarterWindowCoversTheCalendarQuarter()
    {
        var range = await Window(SalesPeriod.Quarter, Utc("2026-09-13T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 7, 1), range.From);
        Assert.Equal(new DateTime(2026, 9, 30), range.To);
        Assert.Equal(new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc), _capturedTo);
        Assert.Equal("Q3 2026", range.Label);
    }

    [Fact]
    public async Task FirstUtcMinutesOfTheYearOpenTheFirstQuarter()
    {
        var range = await Window(SalesPeriod.Quarter, Utc("2027-01-01T04:30:00Z"));

        Assert.Equal("Q1 2027", range.Label);
        Assert.Equal(new DateTime(2027, 1, 1), range.From);
        Assert.Equal(new DateTime(2027, 3, 31), range.To);
    }

    [Fact]
    public async Task SameInstantInAnyOffsetGivesTheSameWindow()
    {
        // One instant, 2026-09-13T04:30Z, read from three machines in different zones.
        var utc = await Window(SalesPeriod.Day, Utc("2026-09-13T04:30:00Z"));
        var utcFrom = _capturedFrom;
        var eastern = await Window(SalesPeriod.Day, new DateTimeOffset(2026, 9, 13, 0, 30, 0, TimeSpan.FromHours(-4)));
        var easternFrom = _capturedFrom;
        var india = await Window(SalesPeriod.Day, new DateTimeOffset(2026, 9, 13, 10, 0, 0, TimeSpan.FromHours(5.5)));

        Assert.Equal(utc.From, eastern.From);
        Assert.Equal(utc.From, india.From);
        Assert.Equal(utcFrom, easternFrom);
        Assert.Equal(utcFrom, _capturedFrom);
        Assert.Equal(new DateTime(2026, 9, 13), utc.From);
    }

    [Fact]
    public async Task LocalMidnightEastOfUtcFallsInThePreviousUtcDay()
    {
        // The machine reads 2026-09-13T00:30+05:30, which is still 2026-09-12 in UTC.
        var range = await Window(SalesPeriod.Day, new DateTimeOffset(2026, 9, 13, 0, 30, 0, TimeSpan.FromHours(5.5)));

        Assert.Equal(new DateTime(2026, 9, 12), range.From);
    }

    [Fact]
    public async Task WeekWindowSpanningTwoMonths()
    {
        var range = await Window(SalesPeriod.Week, Utc("2026-04-01T09:00:00Z"));

        Assert.Equal(new DateTime(2026, 3, 29), range.From);
        Assert.Equal(new DateTime(2026, 4, 4), range.To);
        Assert.Equal("Mar 29 - Apr 4, 2026", range.Label);
    }

    [Fact]
    public async Task WeekWindowSpanningTwoYearsKeepsSevenDaysAndLabelsTheEndYear()
    {
        var range = await Window(SalesPeriod.Week, Utc("2026-12-31T09:00:00Z"));

        Assert.Equal(new DateTime(2026, 12, 27), range.From);
        Assert.Equal(new DateTime(2027, 1, 2), range.To);
        Assert.Equal(new DateTime(2027, 1, 3, 0, 0, 0, DateTimeKind.Utc), _capturedTo);
        Assert.Equal("Dec 27 - Jan 2, 2027", range.Label);
    }

    [Theory]
    [InlineData("2026-02-14T09:00:00Z", 2026, 28)]
    [InlineData("2028-02-14T09:00:00Z", 2028, 29)]
    public async Task MonthWindowCoversFebruaryIncludingLeapYears(string instant, int year, int lastDay)
    {
        var range = await Window(SalesPeriod.Month, Utc(instant));

        Assert.Equal(new DateTime(year, 2, 1), range.From);
        Assert.Equal(new DateTime(year, 2, lastDay), range.To);
        Assert.Equal(new DateTime(year, 3, 1, 0, 0, 0, DateTimeKind.Utc), _capturedTo);
        Assert.Equal($"February {year}", range.Label);
    }

    [Fact]
    public async Task DayWindowOnTheLastDayOfAQuarterStaysInThatQuarter()
    {
        var instant = Utc("2026-09-30T23:59:59Z");

        var day = await Window(SalesPeriod.Day, instant);
        var dayTo = _capturedTo;
        var quarter = await Window(SalesPeriod.Quarter, instant);

        Assert.Equal(new DateTime(2026, 9, 30), day.From);
        Assert.Equal(new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc), dayTo);
        Assert.Equal("Q3 2026", quarter.Label);
        Assert.Equal(new DateTime(2026, 9, 30), quarter.To);
    }

    [Fact]
    public async Task BoundsAreUtcAndRangeDatesAreUnspecified()
    {
        var range = await Window(SalesPeriod.Week, Utc("2026-09-09T15:00:00Z"));

        Assert.Equal(DateTimeKind.Utc, _capturedFrom.Kind);
        Assert.Equal(DateTimeKind.Utc, _capturedTo.Kind);
        Assert.Equal(DateTimeKind.Unspecified, range.From.Kind);
        Assert.Equal(DateTimeKind.Unspecified, range.To.Kind);
    }

    [Fact]
    public async Task DashboardSummaryReadsTheClockOnceSoBothWindowsShareTheInstant()
    {
        _timeService.Setup(t => t.GetCurrentDateTimeOffset()).Returns(Utc("2026-12-31T23:59:59Z"));

        var summary = await _sut.GetDashboardSummary(new GetSalesDashboardSummaryFilter());

        _timeService.Verify(t => t.GetCurrentDateTimeOffset(), Times.Once);
        Assert.Equal("Q4 2026", summary.Quarter.Label);
        Assert.Equal(new DateTime(2026, 12, 27), summary.Week.From);
    }
}
