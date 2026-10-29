using Covenant.Api.Controllers.Workers.History;
using Covenant.Common.Models.Requests.Workers;
using Covenant.Common.Models;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Integration.Tests.Workers.Activity;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Workers.History;

public class HistoryControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<ActivityTestStartup, ActivityTestData>>
{
    private readonly ActivityTestData _data;
    private readonly HttpClient _client;

    public HistoryControllerTest(SeededWebApplicationFactory<ActivityTestStartup, ActivityTestData> factory)
    {
        _data = factory.Data;
        _client = _data.AsWorker(factory.CreateClient);
    }

    [Fact]
    public async Task Get_ListsEveryAssignmentNewestFirstWithTheWorkerFacingStatus()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<WorkerRequestListModel>>($"{HistoryController.RouteName}?pageSize=20");

        Assert.Equal(
            [_data.ActiveRequest.Id, _data.CompletedRequest.Id, _data.EndedRequest.Id, _data.RejectedRequest.Id],
            page.Items.Select(r => r.Id));
        Assert.Equal(["Active", "Completed", "Ended", "Ended"], page.Items.Select(r => r.Status));
    }

    [Fact]
    public async Task Get_ReturnsTheClientAndTheApprovedHoursLessTheUnpaidBreak()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<WorkerRequestListModel>>($"{HistoryController.RouteName}?pageSize=20");

        WorkerRequestListModel active = Assert.Single(page.Items, r => r.Id == _data.ActiveRequest.Id);
        Assert.Equal("Acme Foods", active.ClientName);
        Assert.Equal(62.5, active.HoursWorked, 3);
        Assert.Null(active.EndedAt);
    }

    [Fact]
    public async Task Get_FinishedAssignment_ReturnsTheLastDay()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<WorkerRequestListModel>>($"{HistoryController.RouteName}?pageSize=20");

        WorkerRequestListModel completed = Assert.Single(page.Items, r => r.Id == _data.CompletedRequest.Id);
        DateAssert.Equal(new DateTime(2026, 10, 24), completed.EndedAt.Value);
    }

    [Theory]
    [InlineData("Active", 1)]
    [InlineData("Completed", 1)]
    [InlineData("Ended", 2)]
    public async Task Get_FilteredByStatus_ListsOnlyThatStatus(string status, int expected)
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<WorkerRequestListModel>>($"{HistoryController.RouteName}?status={status}");

        Assert.Equal(expected, page.Items.Count);
        Assert.All(page.Items, r => Assert.Equal(status, r.Status));
    }

    [Fact]
    public async Task Get_FilteredByPast_ListsCompletedAndEndedButNotActive()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<WorkerRequestListModel>>($"{HistoryController.RouteName}?status=Past");

        Assert.Equal(["Completed", "Ended", "Ended"], page.Items.Select(r => r.Status));
    }

    [Fact]
    public async Task GetById_ReturnsTheRequestDetail()
    {
        var detail = await _client.GetFromJsonAsync<WorkerRequestDetailModel>($"{HistoryController.RouteName}/{_data.CompletedRequest.Id}");

        Assert.Equal(_data.CompletedRequest.Id, detail.Id);
    }

    [Fact]
    public async Task GetById_UnknownRequest_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"{HistoryController.RouteName}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
