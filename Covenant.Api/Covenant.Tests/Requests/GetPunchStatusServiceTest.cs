using Covenant.Common.Configuration;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Requests;
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

public class GetPunchStatusServiceTest
{
    private const double TorontoLatitude = 43.6532;
    private const double TorontoLongitude = -79.3832;
    private static readonly TimeSpan EasternDaylight = TimeSpan.FromHours(-4);
    private static readonly DateTime MondayNow = new(2026, 08, 17, 14, 00, 00);
    private static readonly DateTime Sunday = new(2026, 08, 16);

    private readonly Guid _workerId = Guid.NewGuid();
    private readonly Guid _requestId = Guid.NewGuid();
    private readonly Guid _workerRequestId = Guid.NewGuid();
    private readonly FakeTimeProvider _timeProvider = new();
    private readonly Mock<IWorkerRequestRepository> _workerRequestRepository = new();
    private readonly Mock<ITimesheetRepository> _timeSheetRepository = new();
    private readonly TimesheetService _sut;

    public GetPunchStatusServiceTest()
    {
        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(i => i.GetUserId()).Returns(_workerId);
        _timeProvider.SetUtcNow(new DateTimeOffset(MondayNow, EasternDaylight));
        _workerRequestRepository.Setup(r => r.GetWorkerRequestInfo(_workerId, _requestId, It.IsAny<DateTime>()))
            .ReturnsAsync(new WorkerRequestInfoModel
            {
                WorkerRequestId = _workerRequestId,
                Latitude = TorontoLatitude,
                Longitude = TorontoLongitude
            });
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
    public async Task OpenPunch_ReturnsClockOutWithItsClockIn()
    {
        var open = TimeSheet.WorkerClockIn(_workerRequestId, MondayNow.AddHours(-3)).Value;
        SetLatest(open);
        SetWeek(open);

        var status = (await _sut.GetPunchStatus(_requestId, TorontoLatitude, TorontoLongitude)).Value;

        Assert.Equal(ClockType.ClockOut, status.ClockType);
        Assert.Equal(MondayNow.AddHours(-3), status.ClockIn);
        Assert.Null(status.ClockOut);
        Assert.Equal(0, status.WeekHours);
    }

    [Fact]
    public async Task ClosedToday_ReturnsNoneWithTodayStamps()
    {
        var closed = Closed(MondayNow.AddHours(-9), MondayNow.AddHours(-1));
        SetLatest(closed);
        SetWeek(closed);

        var status = (await _sut.GetPunchStatus(_requestId, TorontoLatitude, TorontoLongitude)).Value;

        Assert.Equal(ClockType.None, status.ClockType);
        Assert.Equal(MondayNow.AddHours(-9), status.ClockIn);
        Assert.Equal(MondayNow.AddHours(-1), status.ClockOut);
        Assert.Equal(8, status.DayHours);
        Assert.Equal(8, status.WeekHours);
    }

    [Fact]
    public async Task NothingToday_ReturnsEmptyStampsAndWeekTotal()
    {
        var sunday = Closed(Sunday.AddHours(8), Sunday.AddHours(12));
        SetLatest(sunday);
        SetWeek(sunday);

        var status = (await _sut.GetPunchStatus(_requestId, TorontoLatitude, TorontoLongitude)).Value;

        Assert.Equal(ClockType.ClockIn, status.ClockType);
        Assert.Null(status.ClockIn);
        Assert.Null(status.ClockOut);
        Assert.Equal(0, status.DayHours);
        Assert.Equal(4, status.WeekHours);
        _timeSheetRepository.Verify(r => r.GetTimesheetsSince(_workerId, _requestId, Sunday), Times.Once);
    }

    [Fact]
    public async Task NotAssigned_ReturnsNoneWithoutQueries()
    {
        _workerRequestRepository.Setup(r => r.GetWorkerRequestInfo(_workerId, _requestId, It.IsAny<DateTime>()))
            .ReturnsAsync((WorkerRequestInfoModel)null);

        var status = (await _sut.GetPunchStatus(_requestId, TorontoLatitude, TorontoLongitude)).Value;

        Assert.Equal(ClockType.None, status.ClockType);
        Assert.Null(status.ClockIn);
        Assert.Equal(0, status.WeekHours);
        _timeSheetRepository.Verify(r => r.GetTimesheetsSince(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
    }

    private TimeSheet Closed(DateTime clockIn, DateTime clockOut)
    {
        var timeSheet = TimeSheet.WorkerClockIn(_workerRequestId, clockIn).Value;
        timeSheet.AddClockOut(clockOut);
        return timeSheet;
    }

    private void SetLatest(TimeSheet timeSheet) =>
        _timeSheetRepository.Setup(r => r.GetLatestTimesheet(_workerId, _requestId)).ReturnsAsync(timeSheet);

    private void SetWeek(params TimeSheet[] timeSheets) =>
        _timeSheetRepository.Setup(r => r.GetTimesheetsSince(_workerId, _requestId, It.IsAny<DateTime>()))
            .ReturnsAsync(timeSheets.ToList());
}
