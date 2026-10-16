using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request.Agency;
using Covenant.Common.Repositories.Requests;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Requests;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using System.Net.Http.Json;
using Xunit;
using static Covenant.Api.Controllers.Agency.Recruiting.Workers.RequestHistoryController;

namespace Covenant.Integration.Tests.Agency.Recruiting.Workers;

public class RequestHistoryControllerTest : IClassFixture<CustomWebApplicationFactory<RequestHistoryControllerTest.Startup>>
{
    private readonly HttpClient _client;

    public RequestHistoryControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

    private static string RequestUri(string query = "") =>
        RouteName.Replace("{workerProfileId}", Data.WorkerProfile.Id.ToString()) + query;

    [Fact]
    public async Task Get()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri());
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerRequestHistoryModel>>();
        Assert.Equal(2, list.TotalItems);
        Assert.True(list.Items[0].NumberId > list.Items[1].NumberId);
        WorkerRequestHistoryModel first = list.Items.Single(i => i.Id == Data.Request.Id);
        Assert.Equal(WorkerRequestStatus.Booked, first.WorkerRequestStatus);
        DateAssert.Equal(Data.WorkerTimeSheet.Date, first.StartWorking.GetValueOrDefault());
        DateAssert.Equal(Data.WorkerTimeSheet.Date, first.FinishWorking.GetValueOrDefault());
    }

    [Fact]
    public async Task GetFilteredByNumberId()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri($"?NumberId={Data.SecondRequest.NumberId}"));
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerRequestHistoryModel>>();
        Assert.Single(list.Items);
        Assert.Equal(Data.SecondRequest.Id, list.Items[0].Id);
    }

    [Fact]
    public async Task GetOtherAgencyWorkerIsEmpty()
    {
        HttpResponseMessage response = await _client.GetAsync(
            RouteName.Replace("{workerProfileId}", Data.OtherAgencyWorkerProfile.Id.ToString()));
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerRequestHistoryModel>>();
        Assert.Empty(list.Items);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTestDatabase();
            services.AddSingleton<IRequestRepository, RequestRepository>();
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
        public static readonly Request SecondRequest = FakeData.FakeRequest(AgencyId, startAt: FakeNow);
        public static readonly WorkerProfile WorkerProfile = new(new User(CvnEmail.Create("w_history@mail.com").Value))
        {
            AgencyId = AgencyId,
            Location = FakeData.FakeLocation()
        };
        public static readonly WorkerProfile OtherWorkerProfile = new(new User(CvnEmail.Create("w_other@mail.com").Value))
        {
            AgencyId = AgencyId,
            Location = FakeData.FakeLocation()
        };
        public static readonly WorkerProfile OtherAgencyWorkerProfile = FakeData.FakeWorkerProfile(email: "w_other_agency@mail.com");
        private static readonly WorkerRequest WorkerRequest = WorkerRequest.AgencyBook(WorkerProfile.Id, Request.Id);
        private static readonly WorkerRequest SecondWorkerRequest = WorkerRequest.AgencyBook(WorkerProfile.Id, SecondRequest.Id);
        private static readonly WorkerRequest OtherWorkerRequest = WorkerRequest.AgencyBook(OtherWorkerProfile.Id, Request.Id);
        public static readonly TimeSheet WorkerTimeSheet = TimeSheet.CreateTimeSheet(WorkerRequest, FakeNow.AddDays(5), TimeSpan.FromHours(8), now: FakeNow).Value;
        private static readonly TimeSheet OtherWorkerTimeSheet = TimeSheet.CreateTimeSheet(OtherWorkerRequest, FakeNow, TimeSpan.FromHours(8), now: FakeNow).Value;

        public static void Seed(CovenantContext context)
        {
            context.Agencies.Add(FakeData.FakeAgency(AgencyId));
            context.WorkerProfiles.AddRange(WorkerProfile, OtherWorkerProfile, OtherAgencyWorkerProfile);
            context.Requests.AddRange(Request, SecondRequest);
            context.WorkerRequests.AddRange(WorkerRequest, SecondWorkerRequest, OtherWorkerRequest);
            context.TimeSheets.AddRange(WorkerTimeSheet, OtherWorkerTimeSheet);
            context.SaveChanges();
        }
    }
}
