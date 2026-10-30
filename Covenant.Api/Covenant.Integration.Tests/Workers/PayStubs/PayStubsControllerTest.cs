using Covenant.Api.Controllers.Workers.PayStubs;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Workers.Activity;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Workers.PayStubs;

public class PayStubsControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<ActivityTestStartup, ActivityTestData>>
{
    private readonly ActivityTestData _data;
    private readonly HttpClient _client;

    public PayStubsControllerTest(SeededWebApplicationFactory<ActivityTestStartup, ActivityTestData> factory)
    {
        _data = factory.Data;
        _client = _data.AsWorker(factory.CreateClient);
    }

    [Fact]
    public async Task Get_ListsOnlyTheWorkersPayStubsNewestWeekFirst()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<WorkerPayStubListModel>>(PayStubsController.RouteName);

        Assert.Equal([_data.LastPayStub.Id, _data.PreviousPayStub.Id, _data.OldPayStub.Id], page.Items.Select(p => p.Id));
    }

    [Fact]
    public async Task Get_FilteredByYear_ListsThatYearOnly()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<WorkerPayStubListModel>>($"{PayStubsController.RouteName}?year=2025");

        Assert.Equal(_data.OldPayStub.Id, Assert.Single(page.Items).Id);
    }

    [Fact]
    public async Task Get_ReturnsTheWorkedHoursAmountsAndClients()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<WorkerPayStubListModel>>(PayStubsController.RouteName);

        WorkerPayStubListModel last = page.Items.First();
        Assert.Equal(42, last.Hours, 3);
        Assert.Equal(700m, last.TotalEarnings);
        Assert.Equal(78m, last.TotalDeductions);
        Assert.Equal(622m, last.TotalPaid);
        Assert.Equal(["Acme Foods"], last.Companies);
    }

    [Fact]
    public async Task GetYears_ReturnsTheYearsWithPayStubsNewestFirst()
    {
        var years = await _client.GetFromJsonAsync<List<int>>($"{PayStubsController.RouteName}/years");

        Assert.Equal([2026, 2025], years);
    }

    [Fact]
    public async Task GetYtd_SumsTheWorkersPayStubsOfTheYear()
    {
        var ytd = await _client.GetFromJsonAsync<PayStubYtdModel>($"{PayStubsController.RouteName}/ytd/2026");

        Assert.Equal(1200m, ytd.Earnings);
        Assert.Equal(1044m, ytd.TotalPaid);
    }

    [Fact]
    public async Task GetPdf_PayStubOfAnotherWorker_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"{PayStubsController.RouteName}/{_data.OtherWorkerPayStub.Id}/pdf");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
