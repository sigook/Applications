using Covenant.Api.Validators.Sales;
using Covenant.Common.Models.Sales.Dashboard;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Companies;
using Covenant.Common.Repositories.Requests;
using Covenant.Core.BL.Interfaces.Requests;
using Covenant.Core.BL.Interfaces.Sales;
using Covenant.Core.BL.Services.Sales;
using Moq;
using System.Globalization;
using Xunit;

namespace Covenant.Tests.Sales;

public class SalesServicePeriodWindowTest
{
    private readonly Mock<ICompanyRepository> _companyRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<TimeProvider> _timeProvider = new();
    private readonly ISalesService _sut;
    private readonly Guid _agencyId = Guid.NewGuid();

    private DateTime _capturedFrom;
    private DateTime _capturedTo;
    private DateTime _capturedInteractionsFrom;
    private DateTime _capturedInteractionsTo;

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
            .Callback<Guid, Guid?, DateTime, DateTime>((_, _, from, to) =>
            {
                _capturedInteractionsFrom = from;
                _capturedInteractionsTo = to;
            })
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
            _timeProvider.Object,
            new GetDealsByStatusFilterValidator());
    }

    private static DateTimeOffset Utc(string instant) =>
        DateTimeOffset.Parse(instant, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

    private void SetNow(DateTimeOffset now)
    {
        _timeProvider.Setup(t => t.GetUtcNow()).Returns(now.ToUniversalTime());
        _timeProvider.Setup(t => t.LocalTimeZone).Returns(TimeZoneInfo.CreateCustomTimeZone("Test", now.Offset, "Test", "Test"));
    }

    private async Task<SalesPeriodRangeModel> MonthWindow(DateTimeOffset now)
    {
        SetNow(now);
        var result = await _sut.GetDealsByStatus(new GetDealsByStatusFilter());
        Assert.True(result);
        return result.Value.Period;
    }

    private async Task<SalesPeriodRangeModel> DateWindow(DateTime from, DateTime to)
    {
        SetNow(Utc("2026-09-09T15:00:00Z"));
        var result = await _sut.GetDealsByStatus(new GetDealsByStatusFilter { DateFrom = from, DateTo = to });
        Assert.True(result);
        return result.Value.Period;
    }

    private async Task<SalesDashboardSummaryModel> Summary(DateTimeOffset now)
    {
        SetNow(now);
        return await _sut.GetDashboardSummary(new GetSalesDashboardSummaryFilter());
    }

    [Fact]
    public async Task DateWindowCoversTheInclusiveCalendarDays()
    {
        var range = await DateWindow(new DateTime(2026, 9, 3), new DateTime(2026, 9, 17));

        Assert.Equal(new DateTime(2026, 9, 3), range.From);
        Assert.Equal(new DateTime(2026, 9, 17), range.To);
        Assert.Equal(new DateTime(2026, 9, 3, 0, 0, 0, DateTimeKind.Utc), _capturedFrom);
        Assert.Equal(new DateTime(2026, 9, 18, 0, 0, 0, DateTimeKind.Utc), _capturedTo);
        Assert.Equal("Sep 3 - Sep 17, 2026", range.Label);
    }

    [Fact]
    public async Task DateWindowOfASingleDayLabelsTheDay()
    {
        var range = await DateWindow(new DateTime(2026, 9, 13), new DateTime(2026, 9, 13));

        Assert.Equal(new DateTime(2026, 9, 13), range.From);
        Assert.Equal(new DateTime(2026, 9, 13), range.To);
        Assert.Equal(new DateTime(2026, 9, 14, 0, 0, 0, DateTimeKind.Utc), _capturedTo);
        Assert.Equal("Sep 13, 2026", range.Label);
    }

    [Fact]
    public async Task DateWindowIgnoresTheTimeOfDayOfTheBounds()
    {
        var range = await DateWindow(new DateTime(2026, 9, 3, 14, 30, 0), new DateTime(2026, 9, 17, 23, 59, 0));

        Assert.Equal(new DateTime(2026, 9, 3), range.From);
        Assert.Equal(new DateTime(2026, 9, 17), range.To);
        Assert.Equal(0, _capturedFrom.TimeOfDay.Ticks);
        Assert.Equal(0, _capturedTo.TimeOfDay.Ticks);
    }

    [Fact]
    public async Task DateWindowSpanningTwoYearsLabelsTheEndYear()
    {
        var range = await DateWindow(new DateTime(2026, 12, 27), new DateTime(2027, 1, 2));

        Assert.Equal("Dec 27 - Jan 2, 2027", range.Label);
        Assert.Equal(new DateTime(2027, 1, 3, 0, 0, 0, DateTimeKind.Utc), _capturedTo);
    }

    [Fact]
    public async Task DateWindowDoesNotReadTheClock()
    {
        await DateWindow(new DateTime(2026, 9, 3), new DateTime(2026, 9, 17));

        _timeProvider.Verify(t => t.GetUtcNow(), Times.Never);
    }

    [Fact]
    public async Task DateWindowRejectsAnEndBeforeTheStart()
    {
        SetNow(Utc("2026-09-09T15:00:00Z"));

        var result = await _sut.GetDealsByStatus(new GetDealsByStatusFilter { DateFrom = new DateTime(2026, 9, 17), DateTo = new DateTime(2026, 9, 3) });

        Assert.False(result);
        _companyRepository.Verify(
            r => r.GetDealsByStatus(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<DealStatus>>()),
            Times.Never);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task DateWindowRejectsHalfARange(bool hasFrom, bool hasTo)
    {
        SetNow(Utc("2026-09-09T15:00:00Z"));

        var result = await _sut.GetDealsByStatus(new GetDealsByStatusFilter
        {
            DateFrom = hasFrom ? new DateTime(2026, 9, 3) : null,
            DateTo = hasTo ? new DateTime(2026, 9, 17) : null
        });

        Assert.False(result);
    }

    [Fact]
    public async Task MonthWindowIsTheDefaultAndCoversTheCalendarMonth()
    {
        var range = await MonthWindow(Utc("2026-09-13T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 9, 1), range.From);
        Assert.Equal(new DateTime(2026, 9, 30), range.To);
        Assert.Equal(new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc), _capturedFrom);
        Assert.Equal(new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc), _capturedTo);
        Assert.Equal("September 2026", range.Label);
    }

    [Theory]
    [InlineData("2026-02-14T09:00:00Z", 2026, 28)]
    [InlineData("2028-02-14T09:00:00Z", 2028, 29)]
    public async Task MonthWindowCoversFebruaryIncludingLeapYears(string instant, int year, int lastDay)
    {
        var range = await MonthWindow(Utc(instant));

        Assert.Equal(new DateTime(year, 2, 1), range.From);
        Assert.Equal(new DateTime(year, 2, lastDay), range.To);
        Assert.Equal(new DateTime(year, 3, 1, 0, 0, 0, DateTimeKind.Utc), _capturedTo);
        Assert.Equal($"February {year}", range.Label);
    }

    [Fact]
    public async Task MonthWindowUsesTheUtcCalendarMonth()
    {
        var range = await MonthWindow(Utc("2026-10-01T00:30:00Z"));

        Assert.Equal(new DateTime(2026, 10, 1), range.From);
        Assert.Equal("October 2026", range.Label);
    }

    [Fact]
    public async Task SameInstantInAnyOffsetGivesTheSameMonthWindow()
    {
        // One instant, 2026-10-01T00:30Z, read from three machines in different zones.
        var utc = await MonthWindow(Utc("2026-10-01T00:30:00Z"));
        var utcFrom = _capturedFrom;
        var eastern = await MonthWindow(new DateTimeOffset(2026, 9, 30, 20, 30, 0, TimeSpan.FromHours(-4)));
        var easternFrom = _capturedFrom;
        var india = await MonthWindow(new DateTimeOffset(2026, 10, 1, 6, 0, 0, TimeSpan.FromHours(5.5)));

        Assert.Equal(utc.From, eastern.From);
        Assert.Equal(utc.From, india.From);
        Assert.Equal(utcFrom, easternFrom);
        Assert.Equal(utcFrom, _capturedFrom);
        Assert.Equal(new DateTime(2026, 10, 1), utc.From);
    }

    [Fact]
    public async Task LocalMidnightEastOfUtcFallsInThePreviousUtcMonth()
    {
        // The machine reads 2026-10-01T00:30+05:30, which is still 2026-09-30 in UTC.
        var range = await MonthWindow(new DateTimeOffset(2026, 10, 1, 0, 30, 0, TimeSpan.FromHours(5.5)));

        Assert.Equal(new DateTime(2026, 9, 1), range.From);
        Assert.Equal("September 2026", range.Label);
    }

    [Fact]
    public async Task WeekWindowRunsSundayToSaturday()
    {
        var summary = await Summary(Utc("2026-09-09T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 9, 6), summary.Week.From);
        Assert.Equal(new DateTime(2026, 9, 12), summary.Week.To);
        Assert.Equal(new DateTime(2026, 9, 6, 0, 0, 0, DateTimeKind.Utc), _capturedInteractionsFrom);
        Assert.Equal(new DateTime(2026, 9, 13, 0, 0, 0, DateTimeKind.Utc), _capturedInteractionsTo);
        Assert.Equal("Sep 6 - Sep 12, 2026", summary.Week.Label);
    }

    [Fact]
    public async Task WeekWindowStartsOnTheUtcSundayEvenLateAtNight()
    {
        var summary = await Summary(Utc("2026-09-13T03:30:00Z"));

        Assert.Equal(new DateTime(2026, 9, 13), summary.Week.From);
        Assert.Equal(new DateTime(2026, 9, 19), summary.Week.To);
        Assert.Equal("Sep 13 - Sep 19, 2026", summary.Week.Label);
    }

    [Theory]
    [InlineData("2026-03-10T15:00:00Z", 2026, 3, 8)]
    [InlineData("2026-11-03T15:00:00Z", 2026, 11, 1)]
    public async Task WeekWindowIsAlwaysSevenTwentyFourHourDays(string instant, int year, int month, int day)
    {
        var summary = await Summary(Utc(instant));

        Assert.Equal(new DateTime(year, month, day), summary.Week.From);
        Assert.Equal(TimeSpan.FromDays(7), _capturedInteractionsTo - _capturedInteractionsFrom);
        Assert.Equal(0, _capturedInteractionsFrom.TimeOfDay.Ticks);
        Assert.Equal(0, _capturedInteractionsTo.TimeOfDay.Ticks);
    }

    [Fact]
    public async Task WeekWindowSpanningTwoMonths()
    {
        var summary = await Summary(Utc("2026-04-01T09:00:00Z"));

        Assert.Equal(new DateTime(2026, 3, 29), summary.Week.From);
        Assert.Equal(new DateTime(2026, 4, 4), summary.Week.To);
        Assert.Equal("Mar 29 - Apr 4, 2026", summary.Week.Label);
    }

    [Fact]
    public async Task WeekWindowSpanningTwoYearsKeepsSevenDaysAndLabelsTheEndYear()
    {
        var summary = await Summary(Utc("2026-12-31T09:00:00Z"));

        Assert.Equal(new DateTime(2026, 12, 27), summary.Week.From);
        Assert.Equal(new DateTime(2027, 1, 2), summary.Week.To);
        Assert.Equal(new DateTime(2027, 1, 3, 0, 0, 0, DateTimeKind.Utc), _capturedInteractionsTo);
        Assert.Equal("Dec 27 - Jan 2, 2027", summary.Week.Label);
    }

    [Fact]
    public async Task QuarterWindowCoversTheCalendarQuarter()
    {
        var summary = await Summary(Utc("2026-09-13T15:00:00Z"));

        Assert.Equal(new DateTime(2026, 7, 1), summary.Quarter.From);
        Assert.Equal(new DateTime(2026, 9, 30), summary.Quarter.To);
        Assert.Equal(new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), _capturedFrom);
        Assert.Equal(new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc), _capturedTo);
        Assert.Equal("Q3 2026", summary.Quarter.Label);
    }

    [Fact]
    public async Task FirstUtcMinutesOfTheYearOpenTheFirstQuarter()
    {
        var summary = await Summary(Utc("2027-01-01T04:30:00Z"));

        Assert.Equal("Q1 2027", summary.Quarter.Label);
        Assert.Equal(new DateTime(2027, 1, 1), summary.Quarter.From);
        Assert.Equal(new DateTime(2027, 3, 31), summary.Quarter.To);
    }

    [Fact]
    public async Task LastSecondOfAQuarterStaysInThatQuarter()
    {
        var summary = await Summary(Utc("2026-09-30T23:59:59Z"));

        Assert.Equal("Q3 2026", summary.Quarter.Label);
        Assert.Equal(new DateTime(2026, 9, 30), summary.Quarter.To);
    }

    [Fact]
    public async Task BoundsAreUtcAndRangeDatesAreUnspecified()
    {
        var range = await MonthWindow(Utc("2026-09-09T15:00:00Z"));

        Assert.Equal(DateTimeKind.Utc, _capturedFrom.Kind);
        Assert.Equal(DateTimeKind.Utc, _capturedTo.Kind);
        Assert.Equal(DateTimeKind.Unspecified, range.From.Kind);
        Assert.Equal(DateTimeKind.Unspecified, range.To.Kind);
    }

    [Fact]
    public async Task DashboardSummaryReadsTheClockOnceSoBothWindowsShareTheInstant()
    {
        var summary = await Summary(Utc("2026-12-31T23:59:59Z"));

        _timeProvider.Verify(t => t.GetUtcNow(), Times.Once);
        Assert.Equal("Q4 2026", summary.Quarter.Label);
        Assert.Equal(new DateTime(2026, 12, 27), summary.Week.From);
    }
}
