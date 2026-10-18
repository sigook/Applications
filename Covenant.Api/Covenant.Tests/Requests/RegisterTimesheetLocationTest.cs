using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Worker;
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

public class RegisterTimesheetLocationTest
{
    private const double CheckpointLatitude = 37.69717;
    private const double CheckpointLongitude = -77.66907;
    private const double MetersPerDegreeLatitude = 111320.0;

    private readonly Guid _workerId = Guid.NewGuid();
    private readonly Guid _requestId = Guid.NewGuid();
    private readonly Mock<IWorkerRequestRepository> _workerRequestRepository = new();
    private readonly TimesheetService _sut;

    public RegisterTimesheetLocationTest()
    {
        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService.Setup(i => i.GetUserId()).Returns(_workerId);
        _workerRequestRepository.Setup(r => r.GetWorkerRequestInfo(_workerId, _requestId, It.IsAny<DateTime>()))
            .ReturnsAsync(new WorkerRequestInfoModel
            {
                WorkerRequestId = Guid.NewGuid(),
                Latitude = CheckpointLatitude,
                Longitude = CheckpointLongitude
            });
        _sut = new TimesheetService(
            new FakeTimeProvider(),
            _workerRequestRepository.Object,
            Mock.Of<ITimesheetRepository>(),
            Mock.Of<IRequestRepository>(),
            Mock.Of<ICatalogRepository>(),
            currentUserService.Object,
            Mock.Of<IMediator>(),
            new TelemetryClient(new TelemetryConfiguration { DisableTelemetry = true }),
            Options.Create(new PunchConfiguration { CheckpointRadiusMeters = 100 }));
    }

    [Theory]
    [InlineData(-165, 0, "165 m to the south")]
    [InlineData(0, -130, "130 m to the west")]
    [InlineData(200, 0, "200 m to the north")]
    [InlineData(0, 150, "150 m to the east")]
    [InlineData(-120, -120, "170 m to the south-west")]
    [InlineData(2460, 0, "2.5 km to the north")]
    [InlineData(875000, 0, "875 km to the north")]
    public async Task TooFar_ReportsDistanceAndDirectionFromCheckpoint(double metersNorth, double metersEast, string expected)
    {
        var result = await _sut.Register(_requestId, WorkerAt(metersNorth, metersEast));

        Assert.True(result.IsFailure);
        Assert.Equal($"You are too far from check point: {expected}. Please get closer.", result.StringErrors);
    }

    [Theory]
    [InlineData(-80, 0)]
    [InlineData(99.5, 0)]
    public async Task WithinRadius_DoesNotRejectByDistance(double metersNorth, double metersEast)
    {
        var result = await _sut.Register(_requestId, WorkerAt(metersNorth, metersEast));

        Assert.DoesNotContain("too far", result.StringErrors);
    }

    [Fact]
    public async Task JustBeyondRadius_Rejects()
    {
        var result = await _sut.Register(_requestId, WorkerAt(101, 0));

        Assert.True(result.IsFailure);
        Assert.Contains("too far", result.StringErrors);
    }

    private static WorkerLocationModel WorkerAt(double metersNorth, double metersEast)
    {
        var metersPerDegreeLongitude = MetersPerDegreeLatitude * Math.Cos(CheckpointLatitude * Math.PI / 180.0);
        return new WorkerLocationModel
        {
            Latitude = CheckpointLatitude + metersNorth / MetersPerDegreeLatitude,
            Longitude = CheckpointLongitude + metersEast / metersPerDegreeLongitude
        };
    }
}
