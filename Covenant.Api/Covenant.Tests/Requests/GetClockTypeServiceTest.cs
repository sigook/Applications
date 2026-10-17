using Covenant.Common.Configuration;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Services.Requests;
using MediatR;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;

namespace Covenant.Tests.Requests;

public class GetClockTypeServiceTest
{
    private const double TorontoLatitude = 43.6532;
    private const double TorontoLongitude = -79.3832;
    private const double VancouverLatitude = 49.2827;
    private const double VancouverLongitude = -123.1207;
    private static readonly TimeSpan EasternDaylight = TimeSpan.FromHours(-4);
    private static readonly TimeSpan PacificDaylight = TimeSpan.FromHours(-7);

    private readonly Guid _workerId = Guid.NewGuid();
    private readonly Guid _requestId = Guid.NewGuid();
    private readonly Guid _workerRequestId = Guid.NewGuid();
    private readonly FakeTimeProvider _timeProvider = new();
    private readonly Mock<IWorkerRequestRepository> _workerRequestRepository = new();
    private readonly Mock<ITimesheetRepository> _timeSheetRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly TimesheetService _sut;

    public GetClockTypeServiceTest()
    {
        _currentUserService.Setup(i => i.GetUserId()).Returns(_workerId);
        _sut = new TimesheetService(
            _timeProvider,
            _workerRequestRepository.Object,
            _timeSheetRepository.Object,
            Mock.Of<IRequestRepository>(),
            Mock.Of<ICatalogRepository>(),
            _currentUserService.Object,
            Mock.Of<IMediator>(),
            new TelemetryClient(new TelemetryConfiguration { DisableTelemetry = true }));
    }

    [Fact]
    public async Task WithoutTimesheet_ReturnsClockIn()
    {
        SetNow(new DateTime(2026, 08, 17, 07, 00, 00), EasternDaylight);
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        SetLatestTimesheet(null);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude);

        Assert.Equal(ClockType.ClockIn, result.Value);
    }

    [Fact]
    public async Task NotAssignedToRequest_ReturnsNone()
    {
        SetNow(new DateTime(2026, 08, 17, 07, 00, 00), EasternDaylight);
        _workerRequestRepository.Setup(r => r.GetWorkerRequestInfo(_workerId, _requestId, It.IsAny<DateTime>()))
            .ReturnsAsync((WorkerRequestInfoModel)null);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude);

        Assert.Equal(ClockType.None, result.Value);
        _timeSheetRepository.Verify(r => r.GetLatestTimesheet(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task NightShift_OpenSinceYesterday_ReturnsClockOut()
    {
        var localNow = new DateTime(2026, 08, 18, 06, 00, 00);
        SetNow(localNow, EasternDaylight);
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        SetLatestTimesheet(TimeSheet.WorkerClockIn(_workerRequestId, new DateTime(2026, 08, 17, 22, 00, 00)).Value);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude);

        Assert.Equal(ClockType.ClockOut, result.Value);
    }

    [Fact]
    public async Task OpenWithinMaximumHours_ReturnsClockOut()
    {
        var localNow = new DateTime(2026, 08, 17, 21, 30, 00);
        SetNow(localNow, EasternDaylight);
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        SetLatestTimesheet(TimeSheet.WorkerClockIn(_workerRequestId, localNow.AddHours(-TimeLimits.DefaultTimeLimits.MaximumHoursDay)).Value);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude);

        Assert.Equal(ClockType.ClockOut, result.Value);
    }

    [Fact]
    public async Task OpenBeyondMaximumHours_ReturnsClockIn()
    {
        var localNow = new DateTime(2026, 08, 17, 21, 30, 00);
        SetNow(localNow, EasternDaylight);
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        var beyondLimit = TimeLimits.DefaultTimeLimits.MaximumHoursDay + 1;
        SetLatestTimesheet(TimeSheet.WorkerClockIn(_workerRequestId, localNow.AddHours(-beyondLimit)).Value);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude);

        Assert.Equal(ClockType.ClockIn, result.Value);
    }

    [Fact]
    public async Task LatestTimesheetClosed_ReturnsClockIn()
    {
        var localNow = new DateTime(2026, 08, 17, 21, 30, 00);
        SetNow(localNow, EasternDaylight);
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        var timeSheet = TimeSheet.WorkerClockIn(_workerRequestId, localNow.AddHours(-8)).Value;
        timeSheet.AddClockOut(localNow.AddHours(-1));
        SetLatestTimesheet(timeSheet);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude);

        Assert.Equal(ClockType.ClockIn, result.Value);
    }

    [Fact]
    public async Task OpenButAlreadyApproved_ReturnsNone()
    {
        var localNow = new DateTime(2026, 08, 17, 21, 30, 00);
        SetNow(localNow, EasternDaylight);
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        var timeSheet = TimeSheet.WorkerClockIn(_workerRequestId, localNow.AddHours(-8)).Value;
        timeSheet.AddApprovedTime(localNow.Date.AddHours(8), localNow.Date.AddHours(16));
        SetLatestTimesheet(timeSheet);

        var result = await _sut.GetClockType(_requestId, TorontoLatitude, TorontoLongitude);

        Assert.Equal(ClockType.None, result.Value);
    }

    [Fact]
    public async Task ElapsedHoursUseJobTimeZone_WhenWorkerIsInAnotherTimeZone()
    {
        var jobNow = new DateTime(2026, 08, 17, 21, 00, 00);
        SetNow(jobNow, EasternDaylight);
        SetJobLocation(TorontoLatitude, TorontoLongitude);
        SetLatestTimesheet(TimeSheet.WorkerClockIn(_workerRequestId, jobNow.AddHours(-13)).Value);

        var result = await _sut.GetClockType(_requestId, VancouverLatitude, VancouverLongitude);

        Assert.Equal(ClockType.ClockOut, result.Value);
    }

    [Fact]
    public async Task WithoutJobLocation_UsesWorkerTimeZone()
    {
        var workerNow = new DateTime(2026, 08, 17, 21, 00, 00);
        SetNow(workerNow, PacificDaylight);
        SetJobLocation(null, null);
        SetLatestTimesheet(TimeSheet.WorkerClockIn(_workerRequestId, workerNow.AddHours(-13)).Value);

        var result = await _sut.GetClockType(_requestId, VancouverLatitude, VancouverLongitude);

        Assert.Equal(ClockType.ClockOut, result.Value);
    }

    private void SetNow(DateTime localNow, TimeSpan offset) =>
        _timeProvider.SetUtcNow(new DateTimeOffset(localNow, offset));

    private void SetJobLocation(double? latitude, double? longitude) =>
        _workerRequestRepository.Setup(r => r.GetWorkerRequestInfo(_workerId, _requestId, It.IsAny<DateTime>()))
            .ReturnsAsync(new WorkerRequestInfoModel
            {
                WorkerRequestId = _workerRequestId,
                Latitude = latitude,
                Longitude = longitude
            });

    private void SetLatestTimesheet(TimeSheet timeSheet) =>
        _timeSheetRepository.Setup(r => r.GetLatestTimesheet(_workerId, _requestId)).ReturnsAsync(timeSheet);
}
