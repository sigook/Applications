using ClosedXML.Excel;
using Covenant.Api.Controllers.Agencies.Accounting;
using Covenant.Common.Models.Accounting.Subcontractors;
using Covenant.Common.Models;
using Covenant.Integration.Tests.Configuration;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Accounting;

public class SubcontractorReportsControllerTest : BaseTestOrder,
    IClassFixture<SeededWebApplicationFactory<DeleteSubcontractorReportControllerTest.Startup, DeleteSubcontractorReportControllerTest.Data>>
{
    private const string Uri = ReportsController.RouteName + "/subcontractors";

    private readonly DeleteSubcontractorReportControllerTest.Data _data;
    private readonly HttpClient _client;

    public SubcontractorReportsControllerTest(SeededWebApplicationFactory<DeleteSubcontractorReportControllerTest.Startup, DeleteSubcontractorReportControllerTest.Data> factory)
    {
        _client = factory.CreateClient();
        _data = factory.Data;
    }

    [Fact]
    public async Task List_GroupsTheAgencyReportsByWeekEnding_NewestFirst()
    {
        var page = await _client.GetFromJsonAsync<PaginatedList<PayrollSubContractorListModel>>($"{Uri}?pageSize=50");

        Assert.Equal(
            [DeleteSubcontractorReportControllerTest.Data.WeekThree, DeleteSubcontractorReportControllerTest.Data.WeekTwo, DeleteSubcontractorReportControllerTest.Data.WeekOne],
            page.Items.Select(i => i.WeekEnding.Date));
        PayrollSubContractorListModel weekOne = page.Items.Single(i => i.WeekEnding.Date == DeleteSubcontractorReportControllerTest.Data.WeekOne);
        Assert.Equal(1, weekOne.NumberOfWorkers);
        Assert.Equal(_data.ReportWeekOne.TotalNet, weekOne.TotalNet);
    }

    [Fact]
    public async Task File_ForAWeek_ReturnsTheWorkerSummary()
    {
        HttpResponseMessage response = await _client.GetAsync($"{Uri}/file?weekEnding={DeleteSubcontractorReportControllerTest.Data.WeekOne:yyyy-MM-dd}");

        response.EnsureSuccessStatusCode();
        using var workbook = new XLWorkbook(await response.Content.ReadAsStreamAsync());
        Assert.True(workbook.Worksheet(1).RowsUsed().Count() > 1);
    }
}
