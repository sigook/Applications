using Covenant.Api.Authorization;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Requests.TimeSheets.Agencies;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Requests;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Requests;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using System.Net.Http.Json;
using Xunit;
using static Covenant.Api.Controllers.Agencies.Recruiting.Workers.TimeSheetHistoryController;

namespace Covenant.Integration.Tests.Agencies.Recruiting.Workers;

public class TimeSheetHistoryControllerTest : IClassFixture<CustomWebApplicationFactory<TimeSheetHistoryControllerTest.Startup>>
{
    private readonly HttpClient _client;

    public TimeSheetHistoryControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

    private static string RequestUri(string query = "") =>
        RouteName.Replace("{workerProfileId:guid}", Data.WorkerProfile.Id.ToString()) + query;

    [Fact]
    public async Task GetAccumulatesFromOldestToNewest()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri());
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<TimeSheetHistoryModel>>();
        Assert.Equal(3, list.TotalItems);
        TimeSheetHistoryModel newest = list.Items[0];
        TimeSheetHistoryModel oldest = list.Items[2];
        DateAssert.Equal(Data.TimeSheets[2].Date, newest.Date);
        Assert.Equal(Data.Request.NumberId, newest.NumberId);
        Assert.Equal(Data.Request.Id, newest.RequestId);
        Assert.Equal(8 + 8 + 4, newest.AccumulatedRegularHours);
        Assert.Equal(2, newest.AccumulatedOvertimeHours);
        Assert.Equal(1, newest.AccumulatedMissingHours);
        Assert.Equal(8, oldest.RegularHours);
        Assert.Equal(8, oldest.AccumulatedRegularHours);
        Assert.Equal(0, oldest.AccumulatedOvertimeHours);
        Assert.Equal(oldest.TotalHours, oldest.AccumulatedTotalHours);
    }

    [Fact]
    public async Task GetFilteredByDateRange()
    {
        DateTime from = Data.TimeSheets[1].Date;
        DateTime to = Data.TimeSheets[2].Date;
        HttpResponseMessage response = await _client.GetAsync(RequestUri($"?DateFrom={from:yyyy-MM-dd}&DateTo={to:yyyy-MM-dd}"));
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<TimeSheetHistoryModel>>();
        Assert.Equal(2, list.TotalItems);
        Assert.Equal(16, list.Items[1].AccumulatedRegularHours);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTestDatabase();
            services.AddSingleton<ITimesheetRepository, TimesheetRepository>();
            services.AddSingleton(TimeLimits.DefaultTimeLimits);
            services.AddSingleton(Rates.DefaultRates);
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o => o.AddAgencyPersonnelRole(Data.AgencyId));
            services.AddSingleton<AgencyIdFilter>();
        }

        public void Configure(IApplicationBuilder app, CovenantContext context)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
            Data.Seed(context);
        }
    }

    private static class Data
    {
        public static readonly Guid AgencyId = Guid.NewGuid();
        private static readonly DateTime FakeNow = new(2019, 01, 01);
        public static readonly Request Request = FakeData.FakeRequest(AgencyId, startAt: FakeNow);
        public static readonly WorkerProfile WorkerProfile = new(new User(CvnEmail.Create("w_timesheet@mail.com").Value))
        {
            AgencyId = AgencyId,
            Location = FakeData.FakeLocation()
        };
        private static readonly WorkerRequest WorkerRequest = WorkerRequest.AgencyBook(WorkerProfile.Id, Request.Id);
        public static readonly TimeSheet[] TimeSheets =
        [
            TimeSheet.CreateTimeSheet(WorkerRequest, FakeNow, TimeSpan.FromHours(8), now: FakeNow).Value,
            TimeSheet.CreateTimeSheet(WorkerRequest, FakeNow.AddDays(1), TimeSpan.FromHours(10), now: FakeNow).Value,
            TimeSheet.CreateTimeSheet(WorkerRequest, FakeNow.AddDays(2), TimeSpan.FromHours(4), now: FakeNow).Value
        ];

        public static void Seed(CovenantContext context)
        {
            TimeSheets[2].MissingHours = TimeSpan.FromHours(1);
            context.Agencies.Add(FakeData.FakeAgency(AgencyId));
            context.WorkerProfiles.Add(WorkerProfile);
            context.Requests.Add(Request);
            context.WorkerRequests.Add(WorkerRequest);
            context.TimeSheets.AddRange(TimeSheets);
            context.AddRange(
                TimeSheetTotal.CreateTotal(TimeSheets[0].Id, TimeSpan.FromHours(8), TimeSpan.FromHours(8), TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromHours(8)),
                TimeSheetTotal.CreateTotal(TimeSheets[1].Id, TimeSpan.FromHours(10), TimeSpan.FromHours(8), TimeSpan.FromHours(2), TimeSpan.Zero, TimeSpan.FromHours(18)),
                TimeSheetTotal.CreateTotal(TimeSheets[2].Id, TimeSpan.FromHours(4), TimeSpan.FromHours(4), TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromHours(22)));
            context.SaveChanges();
        }
    }
}
