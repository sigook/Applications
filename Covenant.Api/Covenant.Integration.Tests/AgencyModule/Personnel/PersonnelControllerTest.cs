using Covenant.Api.Controllers.Sigook.Agency.Personnel;
using Covenant.Api.Authorization;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Security;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Xunit;
using System.Net.Http.Json;

namespace Covenant.Integration.Tests.AgencyModule.Personnel
{
    public class PersonnelControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<PersonnelControllerTest.Startup, PersonnelControllerTest.Data>>
    {
        private readonly SeededWebApplicationFactory<Startup, Data> _factory;
        private readonly Data _data;
        private readonly HttpClient _client;

        public PersonnelControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _data = factory.Data;
        }

        private static string RequestUri() => PersonnelController.RouteName;

        [Fact]
        public async Task Post()
        {
            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
                    mockMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonSerializer.Serialize(new IdModel(Data.NewUserId)))
                    }).Verifiable();
                    var client = new HttpClient(mockMessageHandler.Object)
                    {
                        BaseAddress = new Uri("https://localhost:5000/UserAdministration")
                    };
                    var clientFactoryMock = new Mock<IHttpClientFactory>();
                    clientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
                    services.AddSingleton(clientFactoryMock.Object);
                });
            });
            var client = factory.CreateClient();
            var model = new AgencyPersonnelModel
            {
                Email = Data.NewUserEmail,
                Name = "John Papas",
                Role = CovenantConstants.Role.Recruiting
            };
            HttpResponseMessage response = await client.PostAsJsonAsync(RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var context = factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.AgencyPersonnel.FirstOrDefaultAsync(ap => ap.User.Email == model.Email);
            Assert.Equal(Data.NewUserId, entity.UserId);
            Assert.Equal(model.Email, entity.User.Email);
            Assert.Equal(model.Name, entity.Name);
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

            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.False(await context.AgencyPersonnel.AnyAsync(ap => ap.User.Email == model.Email));
        }

        [Fact]
        public async Task PostExistingUser()
        {
            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
                    mockMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)).Verifiable();
                    var client = new HttpClient(mockMessageHandler.Object)
                    {
                        BaseAddress = new Uri("https://localhost:5000/UserAdministration")
                    };
                    var clientFactoryMock = new Mock<IHttpClientFactory>();
                    clientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
                    services.AddSingleton(clientFactoryMock.Object);
                });
            });
            var client = factory.CreateClient();
            var model = new AgencyPersonnelModel
            {
                Email = _data.PersonnelExisting.User.Email,
                Name = _data.PersonnelExisting.Name,
                Role = CovenantConstants.Role.Sales
            };
            HttpResponseMessage response = await client.PostAsJsonAsync(RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var context = factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var list = await context.AgencyPersonnel.Where(w => w.UserId == _data.PersonnelExisting.UserId).ToListAsync();
            Assert.Equal(2, list.Count);

            response = await client.PostAsJsonAsync(RequestUri(), model);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private WebApplicationFactory<Startup> FactoryWithRoles(params UserRoleModel[] roles)
        {
            return _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
                    mockMessageHandler.Protected()
                        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                        .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK));
                    mockMessageHandler.Protected()
                        .Setup<Task<HttpResponseMessage>>("SendAsync",
                            ItExpr.Is<HttpRequestMessage>(r => r.RequestUri.ToString().Contains("UsersRoles")),
                            ItExpr.IsAny<CancellationToken>())
                        .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(JsonSerializer.Serialize(roles))
                        });
                    var client = new HttpClient(mockMessageHandler.Object)
                    {
                        BaseAddress = new Uri("https://localhost:5000/UserAdministration")
                    };
                    var clientFactoryMock = new Mock<IHttpClientFactory>();
                    clientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
                    services.AddSingleton(clientFactoryMock.Object);
                });
            });
        }

        [Fact]
        public async Task Put()
        {
            AgencyPersonnel entity = _data.PersonnelToUpdate;
            var factory = FactoryWithRoles(new UserRoleModel(entity.UserId, CovenantConstants.Role.Recruiting));
            var client = factory.CreateClient();
            var model = new AgencyPersonnelModel
            {
                Name = "Updated Name",
                Email = "updated.personnel@sigook.com",
                Role = CovenantConstants.Role.Sales
            };
            HttpResponseMessage response = await client.PutAsJsonAsync($"{RequestUri()}/{entity.Id}", model);
            response.EnsureSuccessStatusCode();
            var context = factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var updated = await context.AgencyPersonnel.Include(ap => ap.User)
                .SingleAsync(ap => ap.Id == entity.Id);
            Assert.Equal(model.Name, updated.Name);
            Assert.Equal(model.Email, updated.User.Email);
        }

        [Fact]
        public async Task PutRoleOutsideTheAssignableSet()
        {
            AgencyPersonnel entity = _data.Personnel;
            var factory = FactoryWithRoles(new UserRoleModel(entity.UserId, CovenantConstants.Role.Recruiting));
            var client = factory.CreateClient();
            var model = new AgencyPersonnelModel
            {
                Name = entity.Name,
                Email = entity.User.Email,
                Role = CovenantConstants.Role.SuperAdmin
            };
            HttpResponseMessage response = await client.PutAsJsonAsync($"{RequestUri()}/{entity.Id}", model);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutOwnRole()
        {
            AgencyPersonnel entity = _data.CurrentPersonnel;
            var factory = FactoryWithRoles(new UserRoleModel(entity.UserId, CovenantConstants.Role.Admin));
            var client = factory.CreateClient();
            var model = new AgencyPersonnelModel
            {
                Name = entity.Name,
                Email = entity.User.Email,
                Role = CovenantConstants.Role.Recruiting
            };
            HttpResponseMessage response = await client.PutAsJsonAsync($"{RequestUri()}/{entity.Id}", model);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutFromAnotherAgency()
        {
            AgencyPersonnel entity = _data.PersonnelExisting;
            var factory = FactoryWithRoles();
            var client = factory.CreateClient();
            var model = new AgencyPersonnelModel
            {
                Name = "Another Agency",
                Email = entity.User.Email,
                Role = CovenantConstants.Role.Recruiting
            };
            HttpResponseMessage response = await client.PutAsJsonAsync($"{RequestUri()}/{entity.Id}", model);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetIncludesRole()
        {
            AgencyPersonnel entity = _data.Personnel;
            var factory = FactoryWithRoles(new UserRoleModel(entity.UserId, CovenantConstants.Role.Recruiting));
            var client = factory.CreateClient();
            HttpResponseMessage response = await client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<IEnumerable<AgencyPersonnelModel>>();
            var model = list.Single(c => c.Id == entity.Id);
            Assert.Equal(CovenantConstants.Role.Recruiting, model.Role);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<IEnumerable<AgencyPersonnelModel>>();
            AgencyPersonnel entity = _data.Personnel;
            var model = list.Single(c => c.Id == entity.Id);
            Assert.Equal(entity.Name, model.Name);
            Assert.Equal(entity.User.Email, model.Email);
        }

        [Fact]
        public async Task GetById()
        {
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{_data.Personnel.Id}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<AgencyPersonnelModel>();
            AgencyPersonnel entity = _data.Personnel;
            Assert.Equal(entity.Name, model.Name);
            Assert.Equal(entity.User.Email, model.Email);
        }

        [Fact]
        public async Task Delete()
        {
            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
                    mockMessageHandler.Protected()
                        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(JsonSerializer.Serialize(true))
                        }).Verifiable();
                    var client = new HttpClient(mockMessageHandler.Object)
                    {
                        BaseAddress = new Uri("https://localhost:5000/UserAdministration")
                    };
                    var clientFactoryMock = new Mock<IHttpClientFactory>();
                    clientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
                    services.AddSingleton(clientFactoryMock.Object);
                });
            });
            var client = factory.CreateClient();
            Guid id = _data.PersonnelToDelete.Id;
            HttpResponseMessage response = await client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var ctx = factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.False(await ctx.AgencyPersonnel.AnyAsync(a => a.Id == id));
            Assert.False(await ctx.Users.AnyAsync(a => a.Id == _data.PersonnelToDelete.UserId));
        }


        public class Data : ITestData
        {
            public static readonly Guid AgencyId = Guid.NewGuid();
            public static readonly Guid OtherAgencyId = Guid.NewGuid();
            public static readonly Guid ThirdAgencyId = Guid.NewGuid();
            public static readonly CvnEmail NewUserEmail = CvnEmail.Create("pepe.payroll@company.com").Value;
            public static readonly Guid NewUserId = Guid.NewGuid();
            public static readonly Guid CurrentUserId = Guid.NewGuid();

            public AgencyPersonnel Personnel { get; } = AgencyPersonnel.CreatePrimary(AgencyId,
                new User(CvnEmail.Create("recruiter@sigook.com").Value), "Recruiter");
            public AgencyPersonnel PersonnelToUpdate { get; } = AgencyPersonnel.CreatePrimary(AgencyId,
                new User(CvnEmail.Create("update@sigook.com").Value), "To Update");
            public AgencyPersonnel CurrentPersonnel { get; } = AgencyPersonnel.CreatePrimary(AgencyId,
                new User(CvnEmail.Create("current.admin@sigook.com").Value, CurrentUserId), "Current Admin");
            public AgencyPersonnel PersonnelToDelete { get; } = AgencyPersonnel.CreatePrimary(AgencyId,
                new User(CvnEmail.Create("delete@sigook.com").Value));
            public AgencyPersonnel PersonnelExisting { get; } = AgencyPersonnel.CreatePrimary(OtherAgencyId,
                new User(CvnEmail.Create("existing@sigook.com").Value));
            public User UserInMultipleAgencies { get; } = new(CvnEmail.Create("multiple.agency@sigook.com").Value);
            public AgencyPersonnel PersonnelToDeleteAgency1 { get; }
            public AgencyPersonnel PersonnelToDeleteAgency2 { get; }
            public User NewUser { get; } = new(NewUserEmail);

            public Data()
            {
                PersonnelToDeleteAgency1 = AgencyPersonnel.CreatePrimary(ThirdAgencyId, UserInMultipleAgencies);
                PersonnelToDeleteAgency2 = AgencyPersonnel.Create(AgencyId, UserInMultipleAgencies, false);
            }

            public void Seed(CovenantContext context)
            {
                context.Agencies.AddRange(FakeData.FakeAgency(AgencyId), FakeData.FakeAgency(OtherAgencyId), FakeData.FakeAgency(ThirdAgencyId));
                context.AgencyPersonnel.AddRange(Personnel, PersonnelToUpdate, CurrentPersonnel, PersonnelToDelete,
                    PersonnelExisting, PersonnelToDeleteAgency1, PersonnelToDeleteAgency2);
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
                        o.AddSub(Data.CurrentUserId);
                    });
                services.AddTestDatabase();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
                services.AddSingleton<IIdentityServerService, IdentityServerService>();
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
}
