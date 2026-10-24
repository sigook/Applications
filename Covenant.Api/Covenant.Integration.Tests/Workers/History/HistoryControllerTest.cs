using Covenant.Api.Controllers.Workers.History;
using Covenant.Common.Models.Requests.Workers;
using Covenant.Common.Models;
using Covenant.Integration.Tests.Configuration;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Workers.History;

public class HistoryControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<WorkerPortalTestStartup, WorkerPortalTestData>>
{
    private readonly WorkerPortalTestData _data;
    private readonly HttpClient _client;

    public HistoryControllerTest(SeededWebApplicationFactory<WorkerPortalTestStartup, WorkerPortalTestData> factory)
    {
        _data = factory.Data;
        _client = _data.AsWorker(factory.CreateClient);
    }

    [Fact]
    public async Task Get_ListsTheCancelledRequestsTheWorkerWasBookedOn()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<WorkerRequestListModel>>($"{HistoryController.RouteName}?pageSize=20");

        WorkerRequestListModel row = Assert.Single(page.Items, r => r.Id == _data.CancelledRequest.Id);
        Assert.Equal(_data.CancelledRequest.NumberId, row.NumberId);
    }

    [Fact]
    public async Task Get_DoesNotListRequestsThatAreNotCancelled()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<WorkerRequestListModel>>($"{HistoryController.RouteName}?pageSize=20");

        Assert.DoesNotContain(page.Items, r => r.Id == _data.OpenRequest.Id);
    }

    [Fact]
    public async Task GetById_ReturnsTheRequestDetail()
    {
        var detail = await _client.GetFromJsonAsync<WorkerRequestDetailModel>($"{HistoryController.RouteName}/{_data.CancelledRequest.Id}");

        Assert.Equal(_data.CancelledRequest.Id, detail.Id);
    }

    [Fact]
    public async Task GetById_UnknownRequest_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"{HistoryController.RouteName}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
