using Covenant.Api.AgencyModule.AgencyPersonnel.Controllers;
using RecruitingRequestsController = Covenant.Api.Controllers.Sigook.Agency.Recruiting.RequestsController;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.Requests;

public class SalesRequestsControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<SalesRequestsControllerTest.Startup>>
{
    private const string SalesRequestsUri = "api/agency/sales/Requests";

    private readonly CustomWebApplicationFactory<Startup> _factory;

    public SalesRequestsControllerTest(CustomWebApplicationFactory<Startup> factory) => _factory = factory;

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

    private static async Task<List<Guid>> RequestIds(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<AgencyRequestsPagedResponse>();
        return list.Items.Select(i => i.Id).ToList();
    }

    [Fact]
    public async Task SalesOnlySeesRequestsWhereItIsTheSalesRepresentative() =>
        Assert.Equal([Data.OwnRequest.Id], await RequestIds(await AsSales().GetAsync(SalesRequestsUri)));

    [Fact]
    public async Task AdminSeesEveryRequestInTheSalesModule()
    {
        List<Guid> ids = await RequestIds(await AsAdmin().GetAsync(SalesRequestsUri));
        Assert.Equal(3, ids.Count);
        Assert.Contains(Data.OwnRequest.Id, ids);
        Assert.Contains(Data.OtherSalesRequest.Id, ids);
        Assert.Contains(Data.UnassignedRequest.Id, ids);
    }

    [Fact]
    public async Task SalesCannotWidenItsScopeThroughTheQueryString() =>
        Assert.Equal([Data.OwnRequest.Id],
            await RequestIds(await AsSales().GetAsync($"{SalesRequestsUri}?salesUserId={Data.OtherSalesUser.Id}")));

    [Fact]
    public async Task SalesCannotReachTheAgencyRequestsEndpoint() =>
        Assert.Equal(HttpStatusCode.Forbidden, (await AsSales().GetAsync(RecruitingRequestsController.RouteName)).StatusCode);

    [Fact]
    public async Task RecruitingCannotReachTheSalesEndpoint() =>
        Assert.Equal(HttpStatusCode.Forbidden, (await AsRecruiting().GetAsync(SalesRequestsUri)).StatusCode);

    [Theory]
    [InlineData(CovenantConstants.Role.Recruiting)]
    [InlineData(CovenantConstants.Role.Sales)]
    public async Task OnlyAccountingManagersCanListAssignableRoles(string role)
    {
        HttpClient client = ClientAs(role, Data.RecruiterUser.Id);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync($"{AgencyPersonnelController.RouteName}/Roles")).StatusCode);
    }

    [Fact]
    public async Task AdminCannotSeeTheSuperAdminRole()
    {
        HttpResponseMessage response = await AsAdmin().GetAsync($"{AgencyPersonnelController.RouteName}/Roles");
        response.EnsureSuccessStatusCode();
        var roles = await response.Content.ReadFromJsonAsync<string[]>();
        Assert.Equal(CovenantConstants.Role.AgencyAssignable, roles);
    }

    [Fact]
    public async Task SuperAdminSeesEveryAssignableRole()
    {
        HttpClient client = ClientAs(CovenantConstants.Role.SuperAdmin, Data.AdminUser.Id);
        HttpResponseMessage response = await client.GetAsync($"{AgencyPersonnelController.RouteName}/Roles");
        response.EnsureSuccessStatusCode();
        var roles = await response.Content.ReadFromJsonAsync<string[]>();
        Assert.Equal(CovenantConstants.Role.SuperAdminAssignable, roles);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o => o.AddName("sales@mail.com"));
            services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
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
        public static readonly Agency FakeAgency = new Agency("Sales Agency", "4160000000");
        public static readonly Guid AgencyId = FakeAgency.Id;

        public static readonly User SalesUser = new User(CvnEmail.Create("sales@mail.com").Value);
        public static readonly User OtherSalesUser = new User(CvnEmail.Create("other.sales@mail.com").Value);
        public static readonly User AdminUser = new User(CvnEmail.Create("admin@mail.com").Value);
        public static readonly User RecruiterUser = new User(CvnEmail.Create("recruiter@mail.com").Value);

        public static readonly AgencyPersonnel SalesPersonnel = AgencyPersonnel.CreatePrimary(AgencyId, SalesUser.Id, "Sales Rep");
        public static readonly AgencyPersonnel OtherSalesPersonnel = AgencyPersonnel.CreatePrimary(AgencyId, OtherSalesUser.Id, "Other Rep");
        public static readonly AgencyPersonnel AdminPersonnel = AgencyPersonnel.CreatePrimary(AgencyId, AdminUser.Id, "Admin");
        public static readonly AgencyPersonnel RecruiterPersonnel = AgencyPersonnel.CreatePrimary(AgencyId, RecruiterUser.Id, "Recruiter");

        private static readonly CompanyName Name = CompanyName.Create("ACME").Value;

        public static readonly CompanyProfile FakeCompany = CompanyProfile.AgencyCreateCompany(
            new User(CvnEmail.Create("client@mail.com").Value), AgencyId, Name, Name, "6478976541",
            default, default, default, default,
            CompanyProfileIndustry.Create("Labour").Value,
            default, default, default, default,
            "creator@mail.com", CompanyStatus.Lead, null).Value;

        public static readonly CompanyProfileJobPositionRate Rate =
            CompanyProfileJobPositionRate.Create(FakeCompany.Id, "Forklift", 20, 15).Value;

        public static readonly Location FakeLocation = new Location
        {
            Address = "11 Main Street",
            City = new City { Province = new Province { Country = Country.Canada } }
        };

        public static readonly Request OwnRequest;
        public static readonly Request OtherSalesRequest;
        public static readonly Request UnassignedRequest;

        static Data()
        {
            FakeCompany.AddJobPositionRate(Rate);
            Guid rateId = FakeCompany.JobPositionRates.First().Id;
            OwnRequest = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, rateId, FakeLocation);
            OtherSalesRequest = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, rateId, FakeLocation);
            UnassignedRequest = FakeData.FakeRequest(AgencyId, FakeCompany.Company.Id, rateId, FakeLocation);
        }

        public static void Seed(CovenantContext context)
        {
            context.Agencies.Add(FakeAgency);
            context.User.AddRange(SalesUser, OtherSalesUser, AdminUser, RecruiterUser);
            context.AgencyPersonnel.AddRange(SalesPersonnel, OtherSalesPersonnel, AdminPersonnel, RecruiterPersonnel);
            context.AddRange(FakeCompany, OwnRequest, OtherSalesRequest, UnassignedRequest);
            context.RequestComissions.AddRange(
                new RequestComission { RequestId = OwnRequest.Id, AgencyPersonnelId = SalesPersonnel.Id },
                new RequestComission { RequestId = OtherSalesRequest.Id, AgencyPersonnelId = OtherSalesPersonnel.Id });
            context.SaveChanges();
        }
    }
}
