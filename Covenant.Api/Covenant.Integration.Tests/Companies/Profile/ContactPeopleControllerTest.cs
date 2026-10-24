using Covenant.Api.Controllers.Companies.Profile;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Models.Companies;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Companies.Profile;

public class ContactPeopleControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<ContactPeopleControllerTest.Startup, ContactPeopleControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly HttpClient _client;

    public ContactPeopleControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _ = factory.Data;
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    [Fact]
    public async Task Get_ReturnsOnlyTheCallerCompanyContacts()
    {
        var contacts = await _client.GetFromJsonAsync<List<CompanyProfileContactPersonModel>>(ContactPeopleController.RouteName);

        Assert.Contains(contacts, c => c.Email == "existing@client.com");
        Assert.DoesNotContain(contacts, c => c.Email == "other@client.com");
    }

    [Fact]
    public async Task Post_CreatesTheContactInTheCallerCompany()
    {
        var model = new CompanyProfileContactPersonModel
        {
            Title = "Ms",
            FirstName = "Carla",
            LastName = "Contact",
            Position = "HR Manager",
            MobileNumber = "4165550101",
            OfficeNumber = "4165550102",
            OfficeNumberExt = 12,
            Email = "carla@client.com"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync(ContactPeopleController.RouteName, model);

        response.EnsureSuccessStatusCode();
        CompanyProfileContactPerson contact = await Context.CompanyProfileContactPeople.AsNoTracking().SingleAsync(c => c.Email == "carla@client.com");
        Assert.Equal(Data.Company.Id, contact.CompanyProfileId);
        Assert.Equal("HR Manager", contact.Position);
        Assert.Equal(12, contact.OfficeNumberExt);
    }

    [Fact]
    public async Task Delete_RemovesTheContact()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{ContactPeopleController.RouteName}/{Data.ToDelete.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(await Context.CompanyProfileContactPeople.AnyAsync(c => c.Id == Data.ToDelete.Id));
    }

    [Fact]
    public async Task Delete_UnknownContact_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{ContactPeopleController.RouteName}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o =>
            {
                o.AddSub(Data.Company.CompanyId);
                o.AddCompanyRole();
            });
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
        public static readonly CompanyProfile Company = FakeData.FakeCompanyProfile(companyEmail: "company@contacts.com");
        public static readonly CompanyProfile OtherCompany = FakeData.FakeCompanyProfile(companyEmail: "other.company@contacts.com");
        public static readonly CompanyProfileContactPerson Existing = FakeData.FakeContactPerson(Company.Id, email: "existing@client.com");
        public static readonly CompanyProfileContactPerson ToDelete = FakeData.FakeContactPerson(Company.Id, email: "delete@client.com");
        public static readonly CompanyProfileContactPerson OtherContact = FakeData.FakeContactPerson(OtherCompany.Id, email: "other@client.com");

        public void Seed(CovenantContext context)
        {
            context.CompanyProfiles.AddRange(Company, OtherCompany);
            context.CompanyProfileContactPeople.AddRange(Existing, ToDelete, OtherContact);
            context.SaveChanges();
        }
    }
}
