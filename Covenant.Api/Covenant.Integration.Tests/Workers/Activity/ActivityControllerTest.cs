using Covenant.Api.Controllers.Workers.Activity;
using Covenant.Common.Enums;
using Covenant.Common.Models.Requests.TimeSheets;
using Covenant.Common.Models.Workers;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Xunit;

namespace Covenant.Integration.Tests.Workers.Activity;

public class ActivityControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<ActivityTestStartup, ActivityTestData>>
{
    private readonly ActivityTestData _data;
    private readonly HttpClient _client;

    public ActivityControllerTest(SeededWebApplicationFactory<ActivityTestStartup, ActivityTestData> factory)
    {
        _data = factory.Data;
        _client = _data.AsWorker(factory.CreateClient);
    }

    private static string WeekQuery(string action) =>
        $"{ActivityController.RouteName}/{action}?weekStart={ActivityTestData.WeekStart.AddDays(3):yyyy-MM-dd}";

    [Fact]
    public async Task GetTimeSheets_ListsTheWeekFromSundayToSaturdayAcrossAssignments()
    {
        var week = await _client.GetFromJsonAsync<WorkerWeekTimeSheetsModel>(WeekQuery("timesheets"));

        DateAssert.Equal(ActivityTestData.WeekStart, week.WeekStart);
        DateAssert.Equal(ActivityTestData.WeekStart.AddDays(6), week.WeekEnd);
        Assert.Equal(7, week.Items.Count);
        Assert.DoesNotContain(week.Items, ts => ts.Id == _data.NextWeekTimeSheet.Id);
    }

    [Fact]
    public async Task GetTimeSheets_SplitsRegularAndOvertimePerRequestLessTheUnpaidBreak()
    {
        var week = await _client.GetFromJsonAsync<WorkerWeekTimeSheetsModel>(WeekQuery("timesheets"));

        Assert.Equal(51.5, week.Hours.Total, 3);
        Assert.Equal(48, week.Hours.Regular, 3);
        Assert.Equal(3.5, week.Hours.Overtime, 3);
        Assert.Equal(4, week.Hours.PendingApproval, 3);
    }

    [Fact]
    public async Task GetTimeSheets_ReturnsTheStatusOfEachDay()
    {
        var week = await _client.GetFromJsonAsync<WorkerWeekTimeSheetsModel>(WeekQuery("timesheets"));

        Assert.Equal(WorkerTimeSheetStatus.Open, week.Items.Single(ts => ts.Id == _data.OpenTimeSheet.Id).Status);
        WorkerWeekTimeSheetModel pending = week.Items.Single(ts => ts.Id == _data.PendingTimeSheet.Id);
        Assert.Equal(WorkerTimeSheetStatus.Pending, pending.Status);
        Assert.Equal("Beta Logistics", pending.ClientName);
        Assert.Equal(5, week.Items.Count(ts => ts.Status == WorkerTimeSheetStatus.Approved));
    }

    [Fact]
    public async Task GetTimeSheets_ReturnsTheAdjustmentsOfEachDay()
    {
        var week = await _client.GetFromJsonAsync<WorkerWeekTimeSheetsModel>(WeekQuery("timesheets"));

        WorkerWeekTimeSheetModel pending = week.Items.Single(ts => ts.Id == _data.PendingTimeSheet.Id);
        Assert.Equal(TimeSpan.FromHours(1), pending.MissingHours);
        Assert.Equal(TimeSpan.FromMinutes(30), pending.MissingHoursOvertime);
        Assert.Equal(50, pending.BonusOrOthers);
        Assert.Equal("Safety bonus", pending.BonusOrOthersDescription);
        Assert.Equal(15, pending.DeductionsOthers);
        Assert.Equal("Boots", pending.DeductionsOthersDescription);
        Assert.Equal(12.5m, pending.Reimbursements);
        Assert.Equal("Parking", pending.ReimbursementsDescription);
    }

    [Fact]
    public async Task GetSummary_ReturnsTheLastPayStubAndTheWeekHours()
    {
        var summary = await _client.GetFromJsonAsync<WorkerActivitySummaryModel>(WeekQuery("summary"));

        Assert.Equal(_data.LastPayStub.Id, summary.LastPayStub.Id);
        Assert.Equal(51.5, summary.WeekHours.Total, 3);
    }
}
