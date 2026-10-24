using Covenant.Api.Controllers.Agencies.Sales.Clients;
using Covenant.Common.Constants;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities.Sales;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Sales;
using Covenant.Common.Models;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using System.Text.Json;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Sales.Clients;

public class DealsControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<DealsControllerTest.Startup, DealsControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;

    public DealsControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
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

    private static string Uri(CompanyProfile company) => DealsController.RouteName.Replace("{profileId}", company.Id.ToString());

    private static MultipartFormDataContent Form(object model, string fileName = null)
    {
        var form = new MultipartFormDataContent { { new StringContent(JsonSerializer.Serialize(model)), "data" } };
        if (fileName is not null) form.Add(new ByteArrayContent([1, 2, 3]), fileName, fileName);
        return form;
    }

    private static CreateDealModel Model(string title = "Warehouse staffing", decimal value = 5000m) => new()
    {
        Title = title,
        Date = new DateTime(2026, 10, 1),
        Value = value,
        Type = DealType.Temporal,
        Status = DealStatus.Sent
    };

    private async Task<Guid> Create(HttpClient client, CompanyProfile company, CreateDealModel model, string fileName = null)
    {
        HttpResponseMessage response = await client.PostAsync(Uri(company), Form(model, fileName));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    [Fact]
    public async Task Post_CreatesDealOwnedByCurrentUser()
    {
        Guid id = await Create(AsSales(), _data.Company, Model("Owned deal"));

        Deal deal = await Context.Deals.AsNoTracking().SingleAsync(d => d.Id == id);
        Assert.Equal(Data.SalesUser.Id, deal.UserId);
        Assert.Equal(_data.Company.Id, deal.CompanyProfileId);
        Assert.Equal("Owned deal", deal.Title);
        Assert.Equal(DealStatus.Sent, deal.Status);
        Assert.Null(deal.DocumentId);
    }

    [Fact]
    public async Task Post_WithFile_AttachesDocumentAndUploadsIt()
    {
        Guid id = await Create(AsSales(), _data.Company, new CreateDealModel
        {
            Title = "Deal with contract",
            Date = new DateTime(2026, 10, 2),
            Value = 1,
            FileName = "contract.pdf"
        }, "contract.pdf");

        Deal deal = await Context.Deals.AsNoTracking().SingleAsync(d => d.Id == id);
        Assert.NotNull(deal.DocumentId);
        Startup.Files.Verify(f => f.UploadAsync(It.IsAny<Stream>(), "contract.pdf"), Times.Once);
    }

    [Fact]
    public async Task Post_InvalidFileFormat_ReturnsBadRequest()
    {
        HttpResponseMessage response = await AsSales().PostAsync(Uri(_data.Company), Form(Model("Bad file"), "virus.exe"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(await Context.Deals.AnyAsync(d => d.Title == "Bad file"));
    }

    [Fact]
    public async Task Post_WithoutTitle_ReturnsBadRequest()
    {
        HttpResponseMessage response = await AsSales().PostAsync(Uri(_data.Company), Form(Model(title: "")));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_SalesSeesOnlyOwnDeals_AdminSeesAll()
    {
        Guid own = await Create(AsSales(), _data.ListCompany, Model("Mine"));
        Guid other = await Create(AsOtherSales(), _data.ListCompany, Model("Theirs"));

        var salesList = await AsSales().GetFromJsonAsync<PaginatedList<DealListModel>>(Uri(_data.ListCompany));
        Assert.Equal([own], salesList.Items.Select(d => d.Id));

        var adminList = await AsAdmin().GetFromJsonAsync<PaginatedList<DealListModel>>(Uri(_data.ListCompany));
        Assert.Equal(2, adminList.TotalItems);
        Assert.Contains(adminList.Items, d => d.Id == other);
    }

    [Fact]
    public async Task Put_OwnDeal_UpdatesIt()
    {
        Guid id = await Create(AsSales(), _data.Company, Model("Before"));

        HttpResponseMessage response = await AsSales().PutAsync($"{Uri(_data.Company)}/{id}", Form(new UpdateDealModel
        {
            Title = "After",
            Date = new DateTime(2026, 10, 5),
            Value = 9000m,
            Type = DealType.Permanent,
            Status = DealStatus.Accepted
        }));

        response.EnsureSuccessStatusCode();
        Deal deal = await Context.Deals.AsNoTracking().SingleAsync(d => d.Id == id);
        Assert.Equal("After", deal.Title);
        Assert.Equal(9000m, deal.Value);
        Assert.Equal(DealStatus.Accepted, deal.Status);
    }

    [Fact]
    public async Task PutAndDelete_DealOfAnotherRep_ReturnsBadRequest_ButAdminCan()
    {
        Guid id = await Create(AsOtherSales(), _data.Company, Model("Someone else's"));

        HttpResponseMessage put = await AsSales().PutAsync($"{Uri(_data.Company)}/{id}", Form(Model("Hijacked")));
        HttpResponseMessage delete = await AsSales().DeleteAsync($"{Uri(_data.Company)}/{id}");

        Assert.Equal(HttpStatusCode.BadRequest, put.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, delete.StatusCode);
        Assert.Equal("Someone else's", (await Context.Deals.AsNoTracking().SingleAsync(d => d.Id == id)).Title);

        (await AsAdmin().DeleteAsync($"{Uri(_data.Company)}/{id}")).EnsureSuccessStatusCode();
        Assert.False(await Context.Deals.AnyAsync(d => d.Id == id));
    }

    [Fact]
    public async Task Delete_UnknownDeal_ReturnsBadRequest()
    {
        HttpResponseMessage response = await AsSales().DeleteAsync($"{Uri(_data.Company)}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public class Startup
    {
        public static readonly Mock<IFilesContainer> Files = new();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(_ => { });
            services.AddTestDatabase();
            services.AddSingleton(Files.Object);
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
        public static readonly User SalesUser = FakeData.FakeUser("sales@deals.com");
        public static readonly User OtherSalesUser = FakeData.FakeUser("other.sales@deals.com");
        public static readonly User AdminUser = FakeData.FakeUser("admin@deals.com");
        public static readonly User ForeignUser = FakeData.FakeUser("foreign@deals.com");

        private readonly Agency agency = FakeData.FakeAgency();
        private readonly Agency foreignAgency = FakeData.FakeAgency();

        public CompanyProfile Company { get; }
        public CompanyProfile ListCompany { get; }
        public CompanyProfile ForeignCompany { get; }

        public Data()
        {
            Company = FakeData.FakeCompanyProfileForAgency(agency.Id, "Deals Client", companyEmail: "client@deals.com");
            ListCompany = FakeData.FakeCompanyProfileForAgency(agency.Id, "Deals List Client", companyEmail: "list@deals.com");
            ForeignCompany = FakeData.FakeCompanyProfileForAgency(foreignAgency.Id, "Foreign Client", companyEmail: "foreign.client@deals.com");
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
