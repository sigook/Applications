using Covenant.Common.Configuration;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Requests;
using Covenant.Common.Models.Workers;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Services.Requests;
using MediatR;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;

namespace Covenant.Tests.Requests;

public class RegisterTimesheetPunchTest
{
    private const double TorontoLatitude = 43.6532;
    private const double TorontoLongitude = -79.3832;
    private static readonly TimeSpan EasternDaylight = TimeSpan.FromHours(-4);
    private static readonly WorkerLocationModel AtCheckpoint = new() { Latitude = TorontoLatitude, Longitude = TorontoLongitude };

    private readonly Guid _workerId = Guid.NewGuid();
    private readonly Guid _requestId = Guid.NewGuid();
    private readonly Guid _workerRequestId = Guid.NewGuid();
    private readonly FakeTimeProvider _timeProvider = new();
    private readonly Mock<IWorkerRequestRepository> _workerRequestRepository = new();
    private readonly Mock<ITimesheetRepository> _timeSheetRepository = new();
    private readonly TimesheetService _sut;

    public RegisterTimesheetPunchTest()
    {
        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(i => i.GetUserId()).Returns(_workerId);
        _sut = new TimesheetService(
            _timeProvider,
            _workerRequestRepository.Object,
            _timeSheetRepository.Object,
            Mock.Of<IRequestRepository>(),
            Mock.Of<ICatalogRepository>(),
            currentUserService.Object,
            Mock.Of<IMediator>(),
            new TelemetryClient(new TelemetryConfiguration { DisableTelemetry = true }),
            Options.Create(new PunchConfiguration()));
    }

    [Fact]
    public async Task NightShift_OpenSinceYesterday_ClocksOut()
    {
        SetNow(new DateTime(2026, 08, 18, 06, 00, 00));
        SetAssigned();
        var timeSheet = TimeSheet.WorkerClockIn(_workerRequestId, new DateTime(2026, 08, 17, 22, 00, 00)).Value;
        SetLatestTimesheet(timeSheet);

        var result = await _sut.Register(_requestId, AtCheckpoint);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Finish);
        Assert.Equal(new DateTime(2026, 08, 18, 06, 00, 00), timeSheet.ClockOut);
        _timeSheetRepository.Verify(r => r.Create(It.IsAny<TimeSheet>()), Times.Never);
    }

    [Fact]
    public async Task OpenBeyondMaximumHours_StartsNewClockIn()
    {
        var localNow = new DateTime(2026, 08, 18, 06, 00, 00);
        SetNow(localNow);
        SetAssigned();
        var stale = TimeSheet.WorkerClockIn(_workerRequestId, localNow.AddHours(-(TimeLimits.DefaultTimeLimits.MaximumHoursDay + 1))).Value;
        SetLatestTimesheet(stale);

        var result = await _sut.Register(_requestId, AtCheckpoint);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.Finish);
        Assert.Null(stale.ClockOut);
        _timeSheetRepository.Verify(r => r.Create(It.Is<TimeSheet>(t => t.ClockIn == localNow)), Times.Once);
    }

    [Fact]
    public async Task LatestTimesheetClosedAndApproved_StartsNewClockIn()
    {
        var localNow = new DateTime(2026, 08, 18, 07, 00, 00);
        SetNow(localNow);
        SetAssigned();
        var yesterday = TimeSheet.WorkerClockIn(_workerRequestId, localNow.AddDays(-1).AddHours(1)).Value;
        yesterday.AddClockOut(localNow.AddDays(-1).AddHours(9));
        yesterday.AddApprovedTime(yesterday.TimeIn.AddHours(8), yesterday.TimeIn.AddHours(16));
        SetLatestTimesheet(yesterday);

        var result = await _sut.Register(_requestId, AtCheckpoint);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.Finish);
    }

    [Fact]
    public async Task ClosedToday_IsRejected()
    {
        var localNow = new DateTime(2026, 08, 17, 16, 00, 00);
        SetNow(localNow);
        SetAssigned();
        var timeSheet = TimeSheet.WorkerClockIn(_workerRequestId, localNow.AddHours(-8)).Value;
        timeSheet.AddClockOut(localNow.AddHours(-1));
        SetLatestTimesheet(timeSheet);

        var result = await _sut.Register(_requestId, AtCheckpoint);

        Assert.True(result.IsFailure);
        Assert.Contains("already clocked out today", result.StringErrors);
        _timeSheetRepository.Verify(r => r.Create(It.IsAny<TimeSheet>()), Times.Never);
    }

    [Fact]
    public async Task ClosedYesterday_StartsNewClockIn()
    {
        var localNow = new DateTime(2026, 08, 18, 07, 00, 00);
        SetNow(localNow);
        SetAssigned();
        var yesterday = TimeSheet.WorkerClockIn(_workerRequestId, localNow.AddDays(-1).AddHours(1)).Value;
        yesterday.AddClockOut(localNow.AddDays(-1).AddHours(9));
        SetLatestTimesheet(yesterday);

        var result = await _sut.Register(_requestId, AtCheckpoint);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.Finish);
        _timeSheetRepository.Verify(r => r.Create(It.Is<TimeSheet>(t => t.ClockIn == localNow)), Times.Once);
    }

    [Fact]
    public async Task OpenButAlreadyApproved_IsRejected()
    {
        var localNow = new DateTime(2026, 08, 17, 16, 00, 00);
        SetNow(localNow);
        SetAssigned();
        var timeSheet = TimeSheet.WorkerClockIn(_workerRequestId, localNow.AddHours(-8)).Value;
        timeSheet.AddApprovedTime(localNow.Date.AddHours(8), localNow.Date.AddHours(16));
        SetLatestTimesheet(timeSheet);

        var result = await _sut.Register(_requestId, AtCheckpoint);

        Assert.True(result.IsFailure);
        Assert.Contains("already approved", result.StringErrors);
    }

    [Fact]
    public async Task NotAssignedToRequest_IsRejected()
    {
        SetNow(new DateTime(2026, 08, 17, 07, 00, 00));
        _workerRequestRepository.Setup(r => r.GetWorkerRequestInfo(_workerId, _requestId, It.IsAny<DateTime>()))
            .ReturnsAsync((WorkerRequestInfoModel)null);

        var result = await _sut.Register(_requestId, AtCheckpoint);

        Assert.True(result.IsFailure);
        Assert.Contains("not assigned", result.StringErrors);
    }

    private void SetNow(DateTime localNow) =>
        _timeProvider.SetUtcNow(new DateTimeOffset(localNow, EasternDaylight));

    private void SetAssigned() =>
        _workerRequestRepository.Setup(r => r.GetWorkerRequestInfo(_workerId, _requestId, It.IsAny<DateTime>()))
            .ReturnsAsync(new WorkerRequestInfoModel
            {
                WorkerRequestId = _workerRequestId,
                Latitude = TorontoLatitude,
                Longitude = TorontoLongitude,
                CountryCode = "CA"
            });

    private void SetLatestTimesheet(TimeSheet timeSheet) =>
        _timeSheetRepository.Setup(r => r.GetLatestTimesheet(_workerId, _requestId)).ReturnsAsync(timeSheet);
}
