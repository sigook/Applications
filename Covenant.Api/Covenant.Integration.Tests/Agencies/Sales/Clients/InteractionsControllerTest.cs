using Covenant.Api.Controllers.Agencies.Sales.Clients;
using Covenant.Common.Constants;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities.Sales;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Models.Sales;
using Covenant.Common.Models;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Sales.Clients;

public class InteractionsControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<InteractionsControllerTest.Startup, InteractionsControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;

    public InteractionsControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _data = factory.Data;
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    private HttpClient ClientAs(string role, User user)
    {
        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, role);
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.SubHeader, user.Id.ToString());
        return client;
    }

    private HttpClient AsSales() => ClientAs(CovenantConstants.Role.Sales, Data.SalesUser);

    private HttpClient AsOtherSales() => ClientAs(CovenantConstants.Role.Sales, Data.OtherSalesUser);

    private HttpClient AsAdmin() => ClientAs(CovenantConstants.Role.Admin, Data.AdminUser);

    private static string Uri(CompanyProfile company) => InteractionsController.RouteName.Replace("{profileId}", company.Id.ToString());

    private static CreateCompanyInteractionModel Model(string description) => new()
    {
        Description = description,
        InteractionPurpose = InteractionPurpose.Intro,
        InteractionType = InteractionType.Call,
        InteractionStatus = InteractionStatus.Completed
    };

    private async Task<Guid> Create(HttpClient client, CompanyProfile company, string description)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(Uri(company), Model(description));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    [Fact]
    public async Task Post_LogsInteractionOwnedByCurrentUser()
    {
        Guid id = await Create(AsSales(), _data.Company, "Intro call");

        CompanyInteraction interaction = await Context.CompanyInteractions.AsNoTracking().SingleAsync(i => i.Id == id);
        Assert.Equal(Data.SalesUser.Id, interaction.UserId);
        Assert.Equal(_data.Company.Id, interaction.CompanyProfileId);
        Assert.Equal(InteractionType.Call, interaction.InteractionType);
    }

    [Fact]
    public async Task Post_WithoutDescription_ReturnsBadRequest()
    {
        HttpResponseMessage response = await AsSales().PostAsJsonAsync(Uri(_data.Company), Model(""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_SalesSeesOnlyOwnInteractions_AdminSeesAll()
    {
        Guid own = await Create(AsSales(), _data.ListCompany, "Mine");
        Guid other = await Create(AsOtherSales(), _data.ListCompany, "Theirs");

        var salesList = await AsSales().GetFromJsonAsync<PaginatedList<CompanyInteractionListModel>>(Uri(_data.ListCompany));
        Assert.Equal([own], salesList.Items.Select(i => i.Id));

        var adminList = await AsAdmin().GetFromJsonAsync<PaginatedList<CompanyInteractionListModel>>(Uri(_data.ListCompany));
        Assert.Equal(2, adminList.TotalItems);
        Assert.Contains(adminList.Items, i => i.Id == other);
    }

    [Fact]
    public async Task Put_OwnInteraction_UpdatesIt()
    {
        Guid id = await Create(AsSales(), _data.Company, "Before");

        HttpResponseMessage response = await AsSales().PutAsJsonAsync($"{Uri(_data.Company)}/{id}", new UpdateCompanyInteractionModel
        {
            Description = "After",
            InteractionPurpose = InteractionPurpose.Intro,
            InteractionType = InteractionType.Mail,
            InteractionStatus = InteractionStatus.Completed
        });

        response.EnsureSuccessStatusCode();
        CompanyInteraction interaction = await Context.CompanyInteractions.AsNoTracking().SingleAsync(i => i.Id == id);
        Assert.Equal("After", interaction.Description);
        Assert.Equal(InteractionType.Mail, interaction.InteractionType);
    }

    [Fact]
    public async Task PutAndDelete_InteractionOfAnotherRep_ReturnsBadRequest_ButAdminCan()
    {
        Guid id = await Create(AsOtherSales(), _data.Company, "Someone else's");

        HttpResponseMessage put = await AsSales().PutAsJsonAsync($"{Uri(_data.Company)}/{id}", Model("Hijacked"));
        HttpResponseMessage delete = await AsSales().DeleteAsync($"{Uri(_data.Company)}/{id}");

        Assert.Equal(HttpStatusCode.BadRequest, put.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, delete.StatusCode);
        Assert.Equal("Someone else's", (await Context.CompanyInteractions.AsNoTracking().SingleAsync(i => i.Id == id)).Description);

        (await AsAdmin().DeleteAsync($"{Uri(_data.Company)}/{id}")).EnsureSuccessStatusCode();
        Assert.False(await Context.CompanyInteractions.AnyAsync(i => i.Id == id));
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
        public static readonly User SalesUser = FakeData.FakeUser("sales@interactions.com");
        public static readonly User OtherSalesUser = FakeData.FakeUser("other.sales@interactions.com");
        public static readonly User AdminUser = FakeData.FakeUser("admin@interactions.com");
        public static readonly User ForeignUser = FakeData.FakeUser("foreign@interactions.com");

        private readonly Agency agency = FakeData.FakeAgency();
        private readonly Agency foreignAgency = FakeData.FakeAgency();

        public CompanyProfile Company { get; }
        public CompanyProfile ListCompany { get; }
        public CompanyProfile ForeignCompany { get; }

        public Data()
        {
            Company = FakeData.FakeCompanyProfileForAgency(agency.Id, "Interactions Client", companyEmail: "client@interactions.com");
            ListCompany = FakeData.FakeCompanyProfileForAgency(agency.Id, "Interactions List Client", companyEmail: "list@interactions.com");
            ForeignCompany = FakeData.FakeCompanyProfileForAgency(foreignAgency.Id, "Foreign Client", companyEmail: "foreign.client@interactions.com");
        }

        public void Seed(CovenantContext context)
        {
            context.Agencies.AddRange(agency, foreignAgency);
            context.Users.AddRange(SalesUser, OtherSalesUser, AdminUser, ForeignUser);
            context.AgencyPersonnel.AddRange(
                AgencyPersonnel.CreatePrimary(agency.Id, SalesUser.Id, "Sales Rep"),
                AgencyPersonnel.CreatePrimary(agency.Id, OtherSalesUser.Id, "Other Rep"),
                AgencyPersonnel.CreatePrimary(agency.Id, AdminUser.Id, "Admin"),
                AgencyPersonnel.CreatePrimary(foreignAgency.Id, ForeignUser.Id, "Foreign Rep"));
            context.AddRange(Company, ListCompany, ForeignCompany);
            context.SaveChanges();
        }
    }
}
