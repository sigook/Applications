using Covenant.Api.Controllers.Agencies.Recruiting.Clients;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models.Identity;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Recruiting.Clients;

public class UsersControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<ClientsTestStartup, UsersControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<ClientsTestStartup, Data> _factory;
    private readonly Data _data;
    private readonly HttpClient _client;

    public UsersControllerTest(SeededWebApplicationFactory<ClientsTestStartup, Data> factory)
    {
        _factory = factory;
        _data = factory.Data;
        _client = _data.AsRecruiter(factory.CreateClient);
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    private static string Uri(CompanyProfile company) => UsersController.RouteName.Replace("{profileId}", company.Id.ToString());

    [Fact]
    public async Task Get_ListsOnlyUsersOfThatClient_SortedByName()
    {
        var users = await _client.GetFromJsonAsync<List<CompanyUserModel>>(Uri(_data.Company));

        Assert.Equal(["Ann", "Zed"], users.Select(u => u.Name));
    }

    [Fact]
    public async Task Post_CreatesCompanyUser()
    {
        var model = new CompanyUserModel { Email = "new.user@client.com", Name = "Nia", Lastname = "New", Position = "HR", MobileNumber = "4165550001" };

        HttpResponseMessage response = await _client.PostAsJsonAsync(Uri(_data.OtherCompany), model);

        response.EnsureSuccessStatusCode();
        CompanyUser created = await Context.CompanyUsers.AsNoTracking().Include(u => u.User).SingleAsync(u => u.User.Email == "new.user@client.com");
        Assert.Equal(_data.OtherCompany.Id, created.CompanyProfileId);
        Assert.Equal("Nia", created.Name);
        Assert.Equal("HR", created.Position);
    }

    [Fact]
    public async Task Post_EmailAlreadyTaken_ReturnsBadRequest()
    {
        var model = new CompanyUserModel { Email = _data.Ann.User.Email, Name = "Dup" };

        HttpResponseMessage response = await _client.PostAsJsonAsync(Uri(_data.Company), model);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_InvalidEmail_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync(Uri(_data.Company), new CompanyUserModel { Email = "not-an-email" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesUserOfThatClient()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{Uri(_data.OtherCompany)}/{_data.ToDelete.UserId}");

        response.EnsureSuccessStatusCode();
        Assert.False(await Context.CompanyUsers.AnyAsync(u => u.UserId == _data.ToDelete.UserId));
    }

    [Fact]
    public async Task Delete_UnknownUser_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{Uri(_data.Company)}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesDetailsOfCompanyUser()
    {
        var model = new CompanyUserModel { Name = "Edna", Lastname = "Edited", Position = "Manager", MobileNumber = "4165550002" };

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{Uri(_data.OtherCompany)}/{_data.ToEdit.UserId}", model);

        response.EnsureSuccessStatusCode();
        CompanyUser edited = await Context.CompanyUsers.AsNoTracking().SingleAsync(u => u.UserId == _data.ToEdit.UserId);
        Assert.Equal("Edna", edited.Name);
        Assert.Equal("Edited", edited.Lastname);
        Assert.Equal("Manager", edited.Position);
        Assert.Equal("4165550002", edited.MobileNumber);
    }

    [Fact]
    public async Task Put_UnknownUser_ReturnsBadRequest()
    {
        var model = new CompanyUserModel { Name = "Nobody", Lastname = "Ghost" };

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{Uri(_data.Company)}/{Guid.NewGuid()}", model);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Email_UpdatesLoginEmailOfCompanyUser()
    {
        var model = new UpdateEmailModel { NewEmail = "renamed.user@client.com" };

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{Uri(_data.OtherCompany)}/{_data.ToRename.UserId}/Email", model);

        response.EnsureSuccessStatusCode();
        User user = await Context.Users.AsNoTracking().SingleAsync(u => u.Id == _data.ToRename.UserId);
        Assert.Equal("renamed.user@client.com", user.Email);
    }

    [Fact]
    public async Task Email_AlreadyTaken_ReturnsBadRequest()
    {
        var model = new UpdateEmailModel { NewEmail = _data.Zed.User.Email };

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{Uri(_data.Company)}/{_data.Ann.UserId}/Email", model);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Email_UnknownUser_ReturnsBadRequest()
    {
        var model = new UpdateEmailModel { NewEmail = "ghost@client.com" };

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{Uri(_data.Company)}/{Guid.NewGuid()}/Email", model);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public class Data : ClientsTestData
    {
        public CompanyUser Ann { get; }
        public CompanyUser Zed { get; }
        public CompanyUser ToDelete { get; }
        public CompanyUser ToRename { get; }
        public CompanyUser ToEdit { get; }

        public Data()
        {
            Zed = User(Company, "Zed");
            Ann = User(Company, "Ann");
            ToDelete = User(OtherCompany, "Del");
            ToRename = User(OtherCompany, "Ren");
            ToEdit = User(OtherCompany, "Edi");
        }

        private static CompanyUser User(CompanyProfile company, string name) =>
            new CompanyUser(company.Id, FakeData.FakeUser($"{name.ToLower()}.{Guid.NewGuid():N}@client.com"))
                .With(u => u.UpdateDetails(name, "User", "Staff", "4165550000"));

        public override void Seed(CovenantContext context)
        {
            base.Seed(context);
            context.CompanyUsers.AddRange(Zed, Ann, ToDelete, ToRename, ToEdit);
            context.SaveChanges();
        }
    }
}
