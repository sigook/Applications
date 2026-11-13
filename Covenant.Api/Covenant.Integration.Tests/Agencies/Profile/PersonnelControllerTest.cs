using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Agencies.Profile;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Agencies;
using Covenant.Common.Models.Identity;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Profile;

public class PersonnelControllerTest(SeededWebApplicationFactory<PersonnelControllerTest.Startup, PersonnelControllerTest.Data> factory) : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<PersonnelControllerTest.Startup, PersonnelControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<Startup, Data> _factory = factory;
    private readonly Data _data = factory.Data;
    private readonly HttpClient _client = factory.CreateClient();

    private static string RequestUri() => PersonnelController.RouteName;

    private static Guid[] AgenciesOf(Guid userId) => [.. TestAgencyMembers.OfUsers([userId]).Select(m => m.AgencyId).Order()];

    [Fact]
    public async Task Post()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
                services.AddSingleton(UserAdministrationMock.Create(createdUserId: Data.NewUserId).Object));
        });
        var client = factory.CreateClient();
        var model = new AgencyPersonnelModel
        {
            Email = Data.NewUserEmail,
            Name = "John Papas",
            Role = CovenantConstants.Role.Recruiting,
            AgencyIds = [Data.AgencyId, Data.SecondOwnAgencyId]
        };
        HttpResponseMessage response = await client.PostAsJsonAsync(RequestUri(), model);
        response.EnsureSuccessStatusCode();
        var context = factory.Services.GetRequiredService<CovenantContext>();
        var user = await context.Users.SingleAsync(u => u.Id == Data.NewUserId);
        Assert.Equal(model.Email, user.Email);
        Assert.Equal(model.Name, user.Name);
        Assert.Equal(new[] { Data.AgencyId, Data.SecondOwnAgencyId }.Order(), AgenciesOf(Data.NewUserId));
    }

    [Fact]
    public async Task PostWithoutAgencies_UsesTheActiveAgency()
    {
        var newUserId = Guid.NewGuid();
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
                services.AddSingleton(UserAdministrationMock.Create(createdUserId: newUserId).Object));
        });
        var model = new AgencyPersonnelModel
        {
            Email = $"{Guid.NewGuid():N}@sigook.com",
            Name = "Default Agency",
            Role = CovenantConstants.Role.Recruiting
        };
        HttpResponseMessage response = await factory.CreateClient().PostAsJsonAsync(RequestUri(), model);
        response.EnsureSuccessStatusCode();
        Assert.Equal([Data.AgencyId], AgenciesOf(newUserId));
    }

    [Fact]
    public async Task PostToForeignAgency()
    {
        var model = new AgencyPersonnelModel
        {
            Email = "foreign.agency@sigook.com",
            Name = "John Papas",
            Role = CovenantConstants.Role.Recruiting,
            AgencyIds = [Data.OtherAgencyId]
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync(RequestUri(), model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var context = _factory.Services.GetRequiredService<CovenantContext>();
        Assert.False(await context.Users.AnyAsync(u => u.Email == model.Email));
    }

    [Fact]
    public async Task PostRoleOutsideTheAssignableSet()
    {
        var model = new AgencyPersonnelModel
        {
            Email = "not.created@sigook.com",
            Name = "John Papas",
            Role = CovenantConstants.Role.SuperAdmin
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync(RequestUri(), model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var context = _factory.Services.GetRequiredService<CovenantContext>();
        Assert.False(await context.Users.AnyAsync(u => u.Email == model.Email));
    }

    [Fact]
    public async Task PostExistingUser()
    {
        var model = new AgencyPersonnelModel
        {
            Email = _data.Existing.Email,
            Name = _data.Existing.Name,
            Role = CovenantConstants.Role.Sales
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync(RequestUri(), model);
        response.EnsureSuccessStatusCode();
        Assert.Equal(new[] { Data.AgencyId, Data.OtherAgencyId }.Order(), AgenciesOf(_data.Existing.Id));

        response = await _client.PostAsJsonAsync(RequestUri(), model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostExistingNonStaffUser()
    {
        var model = new AgencyPersonnelModel
        {
            Email = _data.NonStaff.Email,
            Name = "Worker",
            Role = CovenantConstants.Role.Sales
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync(RequestUri(), model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private WebApplicationFactory<Startup> FactoryWithRoles(params UserRoleModel[] roles)
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
                services.AddSingleton(UserAdministrationMock.Create(roles: roles).Object));
        });
    }

    [Fact]
    public async Task Put()
    {
        User user = _data.ToUpdate;
        var factory = FactoryWithRoles(new UserRoleModel(user.Id, CovenantConstants.Role.Recruiting));
        var client = factory.CreateClient();
        var model = new AgencyPersonnelModel
        {
            Name = "Updated Name",
            Email = "updated.personnel@sigook.com",
            Role = CovenantConstants.Role.Sales,
            AgencyIds = [Data.SecondOwnAgencyId]
        };
        HttpResponseMessage response = await client.PutAsJsonAsync($"{RequestUri()}/{user.Id}", model);
        response.EnsureSuccessStatusCode();
        var context = factory.Services.GetRequiredService<CovenantContext>();
        var updated = await context.Users.AsNoTracking().SingleAsync(u => u.Id == user.Id);
        Assert.Equal(model.Name, updated.Name);
        Assert.Equal(model.Email, updated.Email);
        Assert.Equal(new[] { Data.SecondOwnAgencyId, Data.OtherAgencyId }.Order(), AgenciesOf(user.Id));
    }

    [Fact]
    public async Task PutWithoutAgencies_KeepsMemberships()
    {
        User user = _data.Personnel;
        var factory = FactoryWithRoles(new UserRoleModel(user.Id, CovenantConstants.Role.Recruiting));
        var model = new AgencyPersonnelModel
        {
            Name = user.Name,
            Email = user.Email,
            Role = CovenantConstants.Role.Recruiting
        };
        HttpResponseMessage response = await factory.CreateClient().PutAsJsonAsync($"{RequestUri()}/{user.Id}", model);
        response.EnsureSuccessStatusCode();
        Assert.Equal([Data.AgencyId], AgenciesOf(user.Id));
    }

    [Fact]
    public async Task PutRoleOutsideTheAssignableSet()
    {
        User user = _data.Personnel;
        var factory = FactoryWithRoles(new UserRoleModel(user.Id, CovenantConstants.Role.Recruiting));
        var client = factory.CreateClient();
        var model = new AgencyPersonnelModel
        {
            Name = user.Name,
            Email = user.Email,
            Role = CovenantConstants.Role.SuperAdmin
        };
        HttpResponseMessage response = await client.PutAsJsonAsync($"{RequestUri()}/{user.Id}", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutOwnRole()
    {
        User user = _data.Current;
        var factory = FactoryWithRoles(new UserRoleModel(user.Id, CovenantConstants.Role.Admin));
        var client = factory.CreateClient();
        var model = new AgencyPersonnelModel
        {
            Name = user.Name,
            Email = user.Email,
            Role = CovenantConstants.Role.Recruiting
        };
        HttpResponseMessage response = await client.PutAsJsonAsync($"{RequestUri()}/{user.Id}", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutRemovingYourselfFromAnAgency()
    {
        User user = _data.Current;
        var factory = FactoryWithRoles(new UserRoleModel(user.Id, CovenantConstants.Role.Admin));
        var model = new AgencyPersonnelModel
        {
            Name = user.Name,
            Email = user.Email,
            Role = CovenantConstants.Role.Admin,
            AgencyIds = [Data.SecondOwnAgencyId]
        };
        HttpResponseMessage response = await factory.CreateClient().PutAsJsonAsync($"{RequestUri()}/{user.Id}", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutFromAnotherAgency()
    {
        User user = _data.OtherAgency;
        var factory = FactoryWithRoles();
        var client = factory.CreateClient();
        var model = new AgencyPersonnelModel
        {
            Name = "Another Agency",
            Email = user.Email,
            Role = CovenantConstants.Role.Recruiting
        };
        HttpResponseMessage response = await client.PutAsJsonAsync($"{RequestUri()}/{user.Id}", model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetIncludesRoleAndAgencies()
    {
        User user = _data.Personnel;
        var factory = FactoryWithRoles(new UserRoleModel(user.Id, CovenantConstants.Role.Recruiting));
        var client = factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync(RequestUri());
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<IEnumerable<AgencyPersonnelModel>>();
        var model = list.Single(c => c.Id == user.Id);
        Assert.Equal(CovenantConstants.Role.Recruiting, model.Role);
        Assert.Equal([Data.AgencyId], model.AgencyIds);
        Assert.Equal(["Own Agency"], model.Agencies);
    }

    [Fact]
    public async Task Get_ListsMembersOfTheCallerAgencies()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri());
        response.EnsureSuccessStatusCode();
        var list = (await response.Content.ReadFromJsonAsync<IEnumerable<AgencyPersonnelModel>>()).ToList();
        var model = list.Single(c => c.Id == _data.Personnel.Id);
        Assert.Equal(_data.Personnel.Name, model.Name);
        Assert.Equal(_data.Personnel.Email, model.Email);
        Assert.DoesNotContain(list, c => c.Id == _data.OtherAgency.Id);
        Assert.DoesNotContain(list, c => c.Id == _data.NonStaff.Id);
        Assert.Contains(list, c => c.Id == _data.SecondAgencyOnly.Id);
    }

    [Fact]
    public async Task Get_FiltersBySelectedAgencies()
    {
        var list = await _client.GetFromJsonAsync<List<AgencyPersonnelModel>>($"{RequestUri()}?agencyIds={Data.AgencyId}");
        Assert.Contains(list, c => c.Id == _data.Personnel.Id);
        Assert.DoesNotContain(list, c => c.Id == _data.SecondAgencyOnly.Id);
    }

    [Fact]
    public async Task Get_HidesAgenciesTheCallerCannotSee()
    {
        var list = await _client.GetFromJsonAsync<List<AgencyPersonnelModel>>(RequestUri());
        var model = list.Single(c => c.Id == _data.InSeveralAgencies.Id);
        Assert.Equal([Data.AgencyId], model.AgencyIds);
    }

    [Fact]
    public async Task GetById()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{_data.Personnel.Id}");
        response.EnsureSuccessStatusCode();
        var model = await response.Content.ReadFromJsonAsync<AgencyPersonnelModel>();
        Assert.Equal(_data.Personnel.Name, model.Name);
        Assert.Equal(_data.Personnel.Email, model.Email);
    }

    [Fact]
    public async Task GetById_FromAnotherAgency_NotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{_data.OtherAgency.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
                services.AddSingleton(UserAdministrationMock.Create(userDeleted: true).Object));
        });
        var client = factory.CreateClient();
        HttpResponseMessage response = await client.DeleteAsync($"{RequestUri()}/{_data.ToDelete.Id}");
        response.EnsureSuccessStatusCode();
        var ctx = factory.Services.GetRequiredService<CovenantContext>();
        Assert.False(await ctx.Users.AnyAsync(a => a.Id == _data.ToDelete.Id));
        Assert.Empty(AgenciesOf(_data.ToDelete.Id));
    }

    [Fact]
    public async Task Delete_KeepsMembershipsOutsideTheCallerAgencies()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{_data.SharedToDelete.Id}");
        response.EnsureSuccessStatusCode();
        var ctx = _factory.Services.GetRequiredService<CovenantContext>();
        Assert.True(await ctx.Users.AnyAsync(a => a.Id == _data.SharedToDelete.Id));
        Assert.Equal([Data.OtherAgencyId], AgenciesOf(_data.SharedToDelete.Id));
    }

    [Fact]
    public async Task Delete_FromAnotherAgency_NotFound()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{_data.OtherAgency.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public class Data : ITestData
    {
        public static readonly Guid AgencyId = Guid.NewGuid();
        public static readonly Guid SecondOwnAgencyId = Guid.NewGuid();
        public static readonly Guid OtherAgencyId = Guid.NewGuid();
        public static readonly CvnEmail NewUserEmail = CvnEmail.Create($"{Guid.NewGuid():N}@company.com").Value;
        public static readonly Guid NewUserId = Guid.NewGuid();
        public static readonly Guid CurrentUserId = Guid.NewGuid();

        public User Personnel { get; } = Staff("Recruiter", AgencyId);
        public User ToUpdate { get; } = Staff("To Update", AgencyId, OtherAgencyId);
        public User Current { get; } = StaffWithId("Current Admin", CurrentUserId, AgencyId, SecondOwnAgencyId);
        public User ToDelete { get; } = Staff("To Delete", AgencyId);
        public User Existing { get; } = Staff("Existing", OtherAgencyId);
        public User OtherAgency { get; } = Staff("Other Agency", OtherAgencyId);
        public User InSeveralAgencies { get; } = Staff("Several", AgencyId, OtherAgencyId);
        public User SecondAgencyOnly { get; } = Staff("Second Only", SecondOwnAgencyId);
        public User SharedToDelete { get; } = Staff("Shared To Delete", AgencyId, SecondOwnAgencyId, OtherAgencyId);
        public User NonStaff { get; } = new(CvnEmail.Create($"{Guid.NewGuid():N}@sigook.com").Value);

        private static User Staff(string name, params Guid[] agencyIds) => StaffWithId(name, Guid.NewGuid(), agencyIds);

        private static User StaffWithId(string name, Guid userId, params Guid[] agencyIds)
        {
            var user = new User(CvnEmail.Create($"{Guid.NewGuid():N}@sigook.com").Value, userId).With(u => u.UpdateName(name));
            foreach (var agencyId in agencyIds) TestAgencyMembers.Add(agencyId, user.Id);
            return user;
        }

        public void Seed(CovenantContext context)
        {
            context.Agencies.AddRange(
                FakeData.FakeAgency(AgencyId).With(a => a.UpdateProfile("Own Agency", null, null, null, null, null)),
                FakeData.FakeAgency(SecondOwnAgencyId),
                FakeData.FakeAgency(OtherAgencyId));
            context.Users.AddRange(Personnel, ToUpdate, Current, ToDelete, Existing, OtherAgency, InSeveralAgencies, SecondAgencyOnly, SharedToDelete, NonStaff);
            context.SaveChanges();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o =>
                {
                    o.AddAdminRole(Data.AgencyId);
                    o.Identity.AddClaim(new Claim(CovenantConstants.AgencyId, Data.SecondOwnAgencyId.ToString()));
                    o.AddSub(Data.CurrentUserId);
                });
            services.AddTestDatabase();
            services.AddSingleton<AgencyIdFilter>();
            services.AddSingleton<IUserAccountService, UserAccountService>();
        }

        public void Configure(IApplicationBuilder app)
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
        }
    }
}
