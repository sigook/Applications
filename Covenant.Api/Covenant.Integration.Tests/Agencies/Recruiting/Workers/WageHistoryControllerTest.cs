using Covenant.Api.Authorization;
using Covenant.Common.Entities.Accounting.PayStubs;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Workers;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Workers;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using System.Net.Http.Json;
using Xunit;
using static Covenant.Api.Controllers.Agencies.Recruiting.Workers.WageHistoryController;

namespace Covenant.Integration.Tests.Agencies.Recruiting.Workers;

public class WageHistoryControllerTest : IClassFixture<CustomWebApplicationFactory<WageHistoryControllerTest.Startup>>
{
    private readonly HttpClient _client;

    public WageHistoryControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

    private static string RequestUri(string query = "") =>
        RouteName.Replace("{workerProfileId:guid}", Data.WorkerProfile.Id.ToString()) + query;

    [Fact]
    public async Task GetAccumulatesFromOldestToNewest()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri());
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<PayStubHistoryModel>>();
        Assert.Equal(3, list.TotalItems);
        Assert.Equal(new long[] { 3, 2, 1 }, list.Items.Select(i => i.NumberId).ToArray());
        PayStubHistoryModel newest = list.Items[0];
        PayStubHistoryModel oldest = list.Items[2];
        Assert.Equal(Data.PayStubs.Sum(p => p.TotalPaid), newest.AccumulatedTotalPaid);
        Assert.Equal(Data.PayStubs.Sum(p => p.TotalEarnings), newest.AccumulatedTotalEarnings);
        Assert.Equal(Data.PayStubs.Sum(p => p.Vacations), newest.AccumulatedVacations);
        Assert.Equal(Data.PayStubs.Sum(p => p.Items.Sum(i => i.Quantity)), newest.AccumulatedQuantity);
        Assert.Equal(Data.PayStubs.Sum(p => p.Items.Sum(i => i.Total)), newest.AccumulatedTotal);
        Assert.Equal(oldest.TotalPaid, oldest.AccumulatedTotalPaid);
        Assert.Equal(oldest.TotalEarnings, oldest.AccumulatedTotalEarnings);
        Assert.Single(oldest.Items);
    }

    [Fact]
    public async Task GetFilteredByPayStubNumber()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri("?PayStubNumber=PS-2"));
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<PayStubHistoryModel>>();
        Assert.Single(list.Items);
        Assert.Equal(2, list.Items[0].NumberId);
    }

    [Fact]
    public async Task GetSortedByTotalPaidAscending()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri("?SortBy=3&IsDescending=false"));
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<PayStubHistoryModel>>();
        Assert.Equal(list.Items.OrderBy(i => i.TotalPaid).Select(i => i.Id), list.Items.Select(i => i.Id));
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTestDatabase();
            services.AddSingleton<IWorkerRepository, WorkerRepository>();
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
        public static readonly WorkerProfile WorkerProfile = new(new User(CvnEmail.Create("w_wage@mail.com").Value))
        {
            AgencyId = AgencyId,
            Location = FakeData.FakeLocation()
        };
        public static readonly PayStub[] PayStubs =
        [
            FakePayStub(1, 300m, new DateTime(2024, 01, 05)),
            FakePayStub(2, 100m, new DateTime(2024, 01, 12)),
            FakePayStub(3, 200m, new DateTime(2024, 01, 19))
        ];

        private static PayStub FakePayStub(long numberId, decimal totalPaid, DateTime weekEnding)
        {
            var payStub = new PayStub
            {
                WorkerProfileId = WorkerProfile.Id,
                NumberId = numberId,
                PayStubNumberId = numberId,
                PayStubNumber = $"PS-{numberId}",
                Position = "Labour",
                DateWorkBegins = weekEnding.AddDays(-6),
                DateWorkEnd = weekEnding,
                PaymentDate = weekEnding.AddDays(3),
                WeekEnding = weekEnding,
                TotalEarnings = totalPaid + 50m,
                Vacations = 10m,
                TotalPaid = totalPaid,
                CreatedAt = weekEnding
            };
            payStub.Items.Add(PayStubItem.CreateRegular(8, 20m).Value);
            return payStub;
        }

        public static void Seed(CovenantContext context)
        {
            context.Agencies.Add(FakeData.FakeAgency(AgencyId));
            context.WorkerProfiles.Add(WorkerProfile);
            context.PayStubs.AddRange(PayStubs);
            context.SaveChanges();
        }
    }
}
