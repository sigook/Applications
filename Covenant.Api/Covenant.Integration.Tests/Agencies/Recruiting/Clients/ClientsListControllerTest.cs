using ClosedXML.Excel;
using Covenant.Api.Controllers.Agencies.Recruiting.Clients;
using Covenant.Common.Constants;
using Covenant.Common.Models.Companies.Agencies;
using Covenant.Common.Models;
using Covenant.Integration.Tests.Configuration;
using System.Net;
using Xunit;
using SalesClientsController = Covenant.Api.Controllers.Agencies.Sales.ClientsController;

namespace Covenant.Integration.Tests.Agencies.Recruiting.Clients;

public class ClientsListControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<ClientsTestStartup, ClientsTestData>>
{
    private readonly SeededWebApplicationFactory<ClientsTestStartup, ClientsTestData> _factory;
    private readonly ClientsTestData _data;

    public ClientsListControllerTest(SeededWebApplicationFactory<ClientsTestStartup, ClientsTestData> factory)
    {
        _factory = factory;
        _data = factory.Data;
    }

    private HttpClient AsRecruiter() => _data.AsRecruiter(_factory.CreateClient);

    private HttpClient AsSales() => _data.ClientAs(_factory.CreateClient, CovenantConstants.Role.Sales, _data.Sales);

    private HttpClient AsSalesAdmin() => _data.ClientAs(_factory.CreateClient, CovenantConstants.Role.Admin, _data.Sales);

    private static async Task<List<string>> Names(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var page = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyProfileListModel>>();
        return [.. page.Items.Select(c => c.FullName)];
    }

    private static async Task<List<string>> SheetNames(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        using var workbook = new XLWorkbook(await response.Content.ReadAsStreamAsync());
        return [.. workbook.Worksheet(1).RowsUsed().Skip(1).SelectMany(r => r.CellsUsed()).Select(c => c.GetString())];
    }

    [Fact]
    public async Task Recruiting_SeesEveryClientOfItsAgency_SortedByName()
    {
        List<string> names = await Names(await AsRecruiter().GetAsync($"{ClientsListController.RouteName}?pageSize=50"));

        Assert.Equal(["Alpha Client", "Beta Client", "Delta Other Sales Client", "Gamma Sales Client"], names);
    }

    [Fact]
    public async Task Recruiting_CannotNarrowToASalesRepThroughTheQueryString()
    {
        List<string> names = await Names(await AsRecruiter().GetAsync(
            $"{ClientsListController.RouteName}?pageSize=50&salesPersonnelId={_data.SalesPersonnel.Id}"));

        Assert.Equal(4, names.Count);
    }

    [Fact]
    public async Task Recruiting_File_ContainsAgencyClientsOnly()
    {
        List<string> cells = await SheetNames(await AsRecruiter().GetAsync($"{ClientsListController.RouteName}/File"));

        Assert.Contains("Alpha Client", cells);
        Assert.DoesNotContain("Foreign Client", cells);
    }

    [Fact]
    public async Task Recruiting_FileWithDetails_IsGenerated()
    {
        HttpResponseMessage response = await AsRecruiter().GetAsync($"{ClientsListController.RouteName}/FileWithDetails");

        response.EnsureSuccessStatusCode();
        Assert.Equal(CovenantConstants.ExcelMime, response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Sales_SeesOnlyClientsWhereItIsTheSalesRepresentative()
    {
        List<string> names = await Names(await AsSales().GetAsync($"{SalesClientsController.RouteName}?pageSize=50"));

        Assert.Equal(["Gamma Sales Client"], names);
    }

    [Fact]
    public async Task Sales_CannotWidenItsScopeThroughTheQueryString()
    {
        List<string> names = await Names(await AsSales().GetAsync(
            $"{SalesClientsController.RouteName}?pageSize=50&salesPersonnelId={_data.OtherSalesPersonnel.Id}"));

        Assert.Equal(["Gamma Sales Client"], names);
    }

    [Fact]
    public async Task Sales_File_IsScopedToOwnClients()
    {
        List<string> cells = await SheetNames(await AsSales().GetAsync($"{SalesClientsController.RouteName}/File"));

        Assert.Contains("Gamma Sales Client", cells);
        Assert.DoesNotContain("Alpha Client", cells);
        Assert.DoesNotContain("Delta Other Sales Client", cells);
    }

    [Fact]
    public async Task Admin_SeesEveryClientInTheSalesModule()
    {
        List<string> names = await Names(await AsSalesAdmin().GetAsync($"{SalesClientsController.RouteName}?pageSize=50"));

        Assert.Equal(4, names.Count);
    }

    [Fact]
    public async Task Recruiting_CannotReachTheSalesClients()
    {
        HttpResponseMessage response = await AsRecruiter().GetAsync(SalesClientsController.RouteName);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
