using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Company.SalesDashboard;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Moq;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.Sales;

public class DashboardControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<DashboardControllerTest.Startup>>
{
    private const string DealsByStatusUri = "api/agency/sales/Dashboard/deals-by-status";
    private const string SummaryUri = "api/agency/sales/Dashboard/summary";

    private readonly CustomWebApplicationFactory<Startup> _factory;

    public DashboardControllerTest(CustomWebApplicationFactory<Startup> factory) => _factory = factory;

    private HttpClient ClientAs(string role, Guid sub)
    {
        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, role);
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.SubHeader, sub.ToString());
        return client;
    }

    private HttpClient AsSales() => ClientAs(CovenantConstants.Role.Sales, Data.SalesUser.Id);

    private HttpClient AsAdmin() => ClientAs(CovenantConstants.Role.Admin, Data.AdminUser.Id);

    private HttpClient AsRecruiting() => ClientAs(CovenantConstants.Role.Recruiting, Data.RecruiterUser.Id);

    private static async Task<DealsByStatusModel> DealsByStatus(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DealsByStatusModel>();
    }

    private static int CountOf(IEnumerable<DealStatusSummaryModel> items, DealStatus status) =>
        items.Single(i => i.Status == status).Count;

    [Fact]
    public async Task SalesOnlySeesItsOwnDealsInTheCurrentWeek()
    {
        DealsByStatusModel model = await DealsByStatus(await AsSales().GetAsync(DealsByStatusUri));

        Assert.Equal(7, model.Items.Count);
        Assert.Equal(2, CountOf(model.Items, DealStatus.Accepted));
        Assert.Equal(1, CountOf(model.Items, DealStatus.Sent));
        Assert.Equal(3, model.TotalCount);
        Assert.Equal(3500m, model.TotalValue);
    }

    [Fact]
    public async Task AdminSeesEveryRepsDealsInTheCurrentWeek()
    {
        DealsByStatusModel model = await DealsByStatus(await AsAdmin().GetAsync(DealsByStatusUri));

        Assert.Equal(4, model.TotalCount);
        Assert.Equal(1, CountOf(model.Items, DealStatus.Rejected));
    }

    [Fact]
    public async Task SalesCannotWidenItsScopeThroughTheQueryString()
    {
        DealsByStatusModel model = await DealsByStatus(
            await AsSales().GetAsync($"{DealsByStatusUri}?ownerId={Data.OtherSalesUser.Id}"));

        Assert.Equal(3, model.TotalCount);
    }

    [Fact]
    public async Task DealsOutsideTheWindowAreExcluded()
    {
        DealsByStatusModel week = await DealsByStatus(await AsSales().GetAsync($"{DealsByStatusUri}?period={(int)SalesPeriod.Week}"));
        DealsByStatusModel month = await DealsByStatus(await AsSales().GetAsync($"{DealsByStatusUri}?period={(int)SalesPeriod.Month}"));

        Assert.Equal(3, week.TotalCount);
        Assert.Equal(4, month.TotalCount);
        Assert.Equal(new DateTime(2026, 9, 6), week.Period.From);
        Assert.Equal(new DateTime(2026, 9, 12), week.Period.To);
        Assert.Equal("September 2026", month.Period.Label);
    }

    [Fact]
    public async Task RequestedStatusesNarrowTheResponse()
    {
        DealsByStatusModel model = await DealsByStatus(
            await AsSales().GetAsync($"{DealsByStatusUri}?statuses[0]={(int)DealStatus.Accepted}"));

        DealStatusSummaryModel item = Assert.Single(model.Items);
        Assert.Equal(DealStatus.Accepted, item.Status);
        Assert.Equal(2, item.Count);
    }

    [Fact]
    public async Task APeriodOutsideTheEnumFallsBackToTheDefaultWeek()
    {
        // The API suppresses the automatic ModelState 400 (ApiServicesConfiguration), so an
        // unbindable period keeps the filter default instead of failing, like every other filter.
        DealsByStatusModel model = await DealsByStatus(await AsSales().GetAsync($"{DealsByStatusUri}?period=99"));

        Assert.Equal(SalesPeriod.Week, model.Period.Period);
        Assert.Equal(new DateTime(2026, 9, 6), model.Period.From);
    }

    [Fact]
    public async Task SummaryReturnsTheQuarterPipelineAndTheWeekActivity()
    {
        HttpResponseMessage response = await AsSales().GetAsync(SummaryUri);
        response.EnsureSuccessStatusCode();
        var model = await response.Content.ReadFromJsonAsync<SalesDashboardSummaryModel>();

        Assert.Equal("Q3 2026", model.Quarter.Label);
        Assert.Equal(new DateTime(2026, 7, 1), model.Quarter.From);
        Assert.Equal(new DateTime(2026, 9, 30), model.Quarter.To);
        Assert.Equal(new DateTime(2026, 9, 6), model.Week.From);
        Assert.Equal(7, model.Pipeline.Count);
        Assert.Equal(4, model.Activity.Count);
        Assert.Equal(4, model.Pipeline.Sum(p => p.Count));
        Assert.Equal(2, model.Activity.Single(a => a.Type == InteractionType.Call).Count);
        Assert.Equal(1, model.Activity.Single(a => a.Type == InteractionType.Mail).Count);
        Assert.Equal(0, model.Activity.Single(a => a.Type == InteractionType.Sms).Count);
    }

    [Fact]
    public async Task RecruitingCannotReachTheDashboard() =>
        Assert.Equal(HttpStatusCode.Forbidden, (await AsRecruiting().GetAsync(SummaryUri)).StatusCode);

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o => o.AddName("sales@dashboard.com"));
            services.AddTestDatabase();
            var timeService = new Mock<ITimeService>();
            timeService.Setup(t => t.GetCurrentDateTimeOffset()).Returns(Data.Now);
            services.AddSingleton(timeService.Object);
        }

        public void Configure(IApplicationBuilder app, CovenantContext context)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            Data.Seed(context);
        }
    }

    private static class Data
    {
        // Wednesday 2026-09-09, 15:00 UTC. Week = Sun 2026-09-06 to Sat 2026-09-12, quarter = Q3 2026.
        public static readonly DateTimeOffset Now = new(2026, 9, 9, 15, 0, 0, TimeSpan.Zero);

        private static DateTime Utc(int day, int hour = 12) => new(2026, 9, day, hour, 0, 0, DateTimeKind.Utc);

        public static readonly Agency FakeAgency = new("Dashboard Agency", "4160000001") { User = FakeData.FakeUser() };
        public static readonly Agency OtherAgency = new("Other Agency", "4160000002") { User = FakeData.FakeUser() };

        public static readonly User SalesUser = new(CvnEmail.Create("sales@dashboard.com").Value);
        public static readonly User OtherSalesUser = new(CvnEmail.Create("other.sales@dashboard.com").Value);
        public static readonly User AdminUser = new(CvnEmail.Create("admin@dashboard.com").Value);
        public static readonly User RecruiterUser = new(CvnEmail.Create("recruiter@dashboard.com").Value);

        public static readonly AgencyPersonnel SalesPersonnel = AgencyPersonnel.CreatePrimary(FakeAgency.Id, SalesUser.Id, "Sales Rep");
        public static readonly AgencyPersonnel OtherSalesPersonnel = AgencyPersonnel.CreatePrimary(FakeAgency.Id, OtherSalesUser.Id, "Other Rep");
        public static readonly AgencyPersonnel AdminPersonnel = AgencyPersonnel.CreatePrimary(FakeAgency.Id, AdminUser.Id, "Admin");
        public static readonly AgencyPersonnel RecruiterPersonnel = AgencyPersonnel.CreatePrimary(FakeAgency.Id, RecruiterUser.Id, "Recruiter");

        public static readonly CompanyProfile FakeCompany =
            FakeData.FakeCompanyProfileForAgency(FakeAgency.Id, "Dashboard Client", companyEmail: "client@dashboard.com");

        public static readonly CompanyProfile OtherAgencyCompany =
            FakeData.FakeCompanyProfileForAgency(OtherAgency.Id, "Foreign Client", companyEmail: "foreign@dashboard.com");

        public static void Seed(CovenantContext context)
        {
            context.Agencies.AddRange(FakeAgency, OtherAgency);
            context.Users.AddRange(SalesUser, OtherSalesUser, AdminUser, RecruiterUser);
            context.AgencyPersonnel.AddRange(SalesPersonnel, OtherSalesPersonnel, AdminPersonnel, RecruiterPersonnel);
            context.AddRange(FakeCompany, OtherAgencyCompany);

            context.Deals.AddRange(
                // Sales rep, inside the current week.
                FakeData.FakeDeal(SalesUser.Id, FakeCompany.Id, Utc(7), DealStatus.Accepted, 2000m),
                FakeData.FakeDeal(SalesUser.Id, FakeCompany.Id, Utc(8), DealStatus.Accepted, 1000m),
                FakeData.FakeDeal(SalesUser.Id, FakeCompany.Id, Utc(9), DealStatus.Sent, 500m),
                // Sales rep, same month but the previous week.
                FakeData.FakeDeal(SalesUser.Id, FakeCompany.Id, Utc(2), DealStatus.ToSend, 700m),
                // Sales rep, previous quarter.
                FakeData.FakeDeal(SalesUser.Id, FakeCompany.Id, new DateTime(2026, 6, 30, 12, 0, 0, DateTimeKind.Utc), DealStatus.Closed, 900m),
                // Another rep, inside the current week.
                FakeData.FakeDeal(OtherSalesUser.Id, FakeCompany.Id, Utc(7), DealStatus.Rejected, 300m),
                // Another agency, inside the current week.
                FakeData.FakeDeal(SalesUser.Id, OtherAgencyCompany.Id, Utc(7), DealStatus.Accepted, 9999m));

            context.CompanyInteractions.AddRange(
                FakeData.FakeCompanyInteraction(SalesUser.Id, FakeCompany.Id, Utc(7), InteractionType.Call),
                FakeData.FakeCompanyInteraction(SalesUser.Id, FakeCompany.Id, Utc(8), InteractionType.Call),
                FakeData.FakeCompanyInteraction(SalesUser.Id, FakeCompany.Id, Utc(9), InteractionType.Mail),
                // Previous week: outside the activity window.
                FakeData.FakeCompanyInteraction(SalesUser.Id, FakeCompany.Id, Utc(2), InteractionType.Sms),
                // Another rep and another agency.
                FakeData.FakeCompanyInteraction(OtherSalesUser.Id, FakeCompany.Id, Utc(9), InteractionType.LinkedIn),
                FakeData.FakeCompanyInteraction(SalesUser.Id, OtherAgencyCompany.Id, Utc(9), InteractionType.Call));

            context.SaveChanges();
        }
    }
}
