using Covenant.Api.Controllers.Agencies.Recruiting.Workers;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Models.Workers.Agencies;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Recruiting.Workers;

public class HolidaysControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<HolidaysControllerTest.Startup, HolidaysControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;
    private readonly HttpClient _client;

    public HolidaysControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _data = factory.Data;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, CovenantConstants.Role.Recruiting);
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    private string Uri(WorkerProfile worker) => HolidaysController.RouteName.Replace("{workerProfileId}", worker.Id.ToString());

    private Task<List<WorkerProfileHolidayModel>> Holidays(WorkerProfile worker) =>
        _client.GetFromJsonAsync<List<WorkerProfileHolidayModel>>(Uri(worker));

    [Fact]
    public async Task Get_ListsRecentHolidaysOfTheWorkerCountry_NewestFirst()
    {
        List<WorkerProfileHolidayModel> holidays = await Holidays(_data.Worker);

        Assert.Equal([_data.Upcoming.Id, _data.Recent.Id], holidays.Select(h => h.HolidayId));
        Assert.All(holidays, h => Assert.Equal(0, h.StatPaidWorker));
    }

    [Fact]
    public async Task Post_SetsAndThenUpdatesTheStatPayForTheWorker()
    {
        (await _client.PostAsJsonAsync(Uri(_data.PaidWorker), new WorkerProfileHolidayModel { HolidayId = _data.Recent.Id, StatPaidWorker = 120m })).EnsureSuccessStatusCode();
        (await _client.PostAsJsonAsync(Uri(_data.PaidWorker), new WorkerProfileHolidayModel { HolidayId = _data.Recent.Id, StatPaidWorker = 150m })).EnsureSuccessStatusCode();

        WorkerProfileHolidayModel holiday = Assert.Single(await Holidays(_data.PaidWorker), h => h.HolidayId == _data.Recent.Id);
        Assert.Equal(150m, holiday.StatPaidWorker);
        Assert.Equal(1, await Context.WorkerProfileHolidays.CountAsync(h => h.WorkerProfileId == _data.PaidWorker.Id));
    }

    [Fact]
    public async Task NewHoliday_IsCreatedForTheWorkerCountryOnlyOnce()
    {
        DateTime date = DateTime.Today.AddDays(20);

        (await _client.PostAsJsonAsync($"{Uri(_data.Worker)}/new-holiday", new WorkerProfileHolidayModel { Date = date })).EnsureSuccessStatusCode();
        (await _client.PostAsJsonAsync($"{Uri(_data.Worker)}/new-holiday", new WorkerProfileHolidayModel { Date = date })).EnsureSuccessStatusCode();

        Assert.Equal(1, await Context.Holidays.CountAsync(h => h.Date == date && h.CountryCode == "CA"));
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(_ => { });
            services.AddTestDatabase();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }

    public class Data : ITestData
    {
        private readonly City city = FakeData.FakeCity(FakeData.FakeProvince(FakeData.FakeCountry("CA")));

        public WorkerProfile Worker { get; }
        public WorkerProfile PaidWorker { get; }
        public Holiday Upcoming { get; } = new(DateTime.Today.AddDays(10), "CA");
        public Holiday Recent { get; } = new(DateTime.Today.AddDays(-10), "CA");
        public Holiday Old { get; } = new(DateTime.Today.AddMonths(-3), "CA");
        public Holiday OtherCountry { get; } = new(DateTime.Today.AddDays(5), "USA");

        public Data()
        {
            var agency = FakeData.FakeAgency(city: city);
            Worker = FakeData.FakeWorkerProfile(agency, "holidays.worker@mail.com", city);
            PaidWorker = FakeData.FakeWorkerProfile(agency, "holidays.paid@mail.com", city);
        }

        public void Seed(CovenantContext context)
        {
            context.Holidays.AddRange(Upcoming, Recent, Old, OtherCountry);
            context.WorkerProfiles.AddRange(Worker, PaidWorker);
            context.SaveChanges();
        }
    }
}
