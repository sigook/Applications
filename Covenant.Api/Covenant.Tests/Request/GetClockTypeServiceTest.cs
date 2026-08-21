using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Services;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Covenant.Tests.Request;

public class GetClockTypeServiceTest
{
    private const double TorontoLatitude = 43.6532;
    private const double TorontoLongitude = -79.3832;
    private const double VancouverLatitude = 49.2827;
    private const double VancouverLongitude = -123.1207;

    private readonly Guid _workerId = Guid.NewGuid();
    private readonly Guid _requestId = Guid.NewGuid();
    private readonly Guid _workerRequestId = Guid.NewGuid();
    private readonly Mock<ITimeService> _timeService = new();
    private readonly Mock<IWorkerRequestRepository> _workerRequestRepository = new();
    private readonly Mock<ITimesheetRepository> _timeSheetRepository = new();
    private readonly Mock<IIdentityServerService> _identityServerService = new();
    private readonly TimesheetService _sut;

    public GetClockTypeServiceTest()
    {
        _identityServerService.Setup(i => i.GetUserId()).Returns(_workerId);
        _sut = new TimesheetService(
            _timeService.Object,
            _workerRequestRepository.Object,
            _timeSheetRepository.Object,
            Mock.Of<IRequestRepository>(),
            Mock.Of<ICatalogRepository>(),
            Mock.Of<IConfiguration>(),
            _identityServerService.Object,
            Mock.Of<IMediator>(),
            new TelemetryClient(new TelemetryConfiguration { DisableTelemetry = true }));
    }

    [Fact]
    public async Task EveningInEasternTime_WithoutTimesheet_ReturnsClockIn()
    {
        var localNow = new DateTime(2026, 08, 17, 21, 30, 00);
        SetLocalTime(TorontoLatitude, TorontoLongitude, localNow);
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        SetLatestTimesheet(null);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude, localNow);

        Assert.Equal(ClockType.ClockIn, result.Value);
    }

    [Fact]
    public async Task EveningInPacificTime_WithoutTimesheet_ReturnsClockIn()
    {
        var localNow = new DateTime(2026, 08, 17, 21, 30, 00);
        SetLocalTime(VancouverLatitude, VancouverLongitude, localNow);
        SetJobLocation(VancouverLatitude, VancouverLongitude);
        SetLatestTimesheet(null);

        var result = await _sut.GetClockType(_requestId, VancouverLatitude, VancouverLongitude, localNow);

        Assert.Equal(ClockType.ClockIn, result.Value);
    }

    [Fact]
    public async Task JobTimeZoneWins_WhenWorkerIsInAnotherTimeZone()
    {
        var workerNow = new DateTime(2026, 08, 17, 22, 00, 00);
        SetLocalTime(VancouverLatitude, VancouverLongitude, workerNow);
        SetLocalTime(TorontoLatitude, TorontoLongitude, new DateTime(2026, 08, 18, 01, 00, 00));
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        SetLatestTimesheet(null);

        var result = await _sut.GetClockType(_requestId, VancouverLatitude, VancouverLongitude, workerNow);

        Assert.Equal(ClockType.None, result.Value);
        _timeService.Verify(t => t.GetCurrentLocalDateTime(TorontoLatitude, TorontoLongitude), Times.Once);
    }

    [Fact]
    public async Task WithoutJobLocation_FallsBackToWorkerCoordinates()
    {
        var localNow = new DateTime(2026, 08, 17, 21, 30, 00);
        SetLocalTime(TorontoLatitude, TorontoLongitude, localNow);
        SetJobLocation(null, null);
        SetLatestTimesheet(null);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude, localNow);

        Assert.Equal(ClockType.ClockIn, result.Value);
    }

    [Fact]
    public async Task PreviousDayWithoutTimesheet_ReturnsNone()
    {
        var localNow = new DateTime(2026, 08, 17, 21, 30, 00);
        SetLocalTime(TorontoLatitude, TorontoLongitude, localNow);
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        SetLatestTimesheet(null);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude, localNow.AddDays(-1));

        Assert.Equal(ClockType.None, result.Value);
    }

    [Fact]
    public async Task ClockInAndClockOutRegistered_ReturnsNone()
    {
        var localNow = new DateTime(2026, 08, 17, 21, 30, 00);
        SetLocalTime(TorontoLatitude, TorontoLongitude, localNow);
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        var timeSheet = TimeSheet.WorkerClockIn(_workerRequestId, localNow.AddHours(-8)).Value;
        timeSheet.AddClockOut(localNow.AddHours(-1));
        SetLatestTimesheet(timeSheet);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude, localNow);

        Assert.Equal(ClockType.None, result.Value);
    }

    [Fact]
    public async Task OnlyClockIn_WithinMaximumHours_ReturnsClockOut()
    {
        var localNow = new DateTime(2026, 08, 17, 21, 30, 00);
        SetLocalTime(TorontoLatitude, TorontoLongitude, localNow);
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        SetLatestTimesheet(TimeSheet.WorkerClockIn(_workerRequestId, localNow.AddHours(-8)).Value);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude, localNow);

        Assert.Equal(ClockType.ClockOut, result.Value);
    }

    [Fact]
    public async Task OnlyClockIn_BeyondMaximumHours_ReturnsNone()
    {
        var localNow = new DateTime(2026, 08, 17, 21, 30, 00);
        SetLocalTime(TorontoLatitude, TorontoLongitude, localNow);
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        var beyondLimit = TimeLimits.DefaultTimeLimits.MaximumHoursDay + 1;
        SetLatestTimesheet(TimeSheet.WorkerClockIn(_workerRequestId, localNow.AddHours(-beyondLimit)).Value);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude, localNow);

        Assert.Equal(ClockType.None, result.Value);
    }

    [Fact]
    public async Task WithoutDate_ReturnsNoneWithoutHittingRepositories()
    {
        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude, null);

        Assert.Equal(ClockType.None, result.Value);
        _timeSheetRepository.Verify(r => r.GetLatestTimesheet(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
    }

    private void SetLocalTime(double latitude, double longitude, DateTime localNow) =>
        _timeService.Setup(t => t.GetCurrentLocalDateTime(latitude, longitude)).Returns(new DateTimeOffset(localNow, TimeSpan.Zero));

    private void SetJobLocation(double? latitude, double? longitude) =>
        _workerRequestRepository.Setup(r => r.GetWorkerRequestInfo(_workerId, _requestId, It.IsAny<DateTime>()))
            .ReturnsAsync(new WorkerRequestInfoModel
            {
                WorkerRequestId = _workerRequestId,
                Latitude = latitude,
                Longitude = longitude
            });

    private void SetLatestTimesheet(TimeSheet timeSheet) =>
        _timeSheetRepository.Setup(r => r.GetLatestTimesheet(_workerId, _requestId, It.IsAny<DateTime>())).ReturnsAsync(timeSheet);
}
