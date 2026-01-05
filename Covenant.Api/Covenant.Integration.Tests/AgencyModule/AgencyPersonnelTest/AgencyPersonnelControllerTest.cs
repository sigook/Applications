using Covenant.Api.AgencyModule.AgencyPersonnel.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyPersonnelTest
{
    public class AgencyPersonnelControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<AgencyPersonnelControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyPersonnelControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyPersonnelController.RouteName;

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
                        Content = new StringContent(JsonConvert.SerializeObject(new IdModel(Startup.NewUserId)))
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
                Email = Startup.NewUserEmail,
                Name = "John Papas"
            };
            HttpResponseMessage response = await client.PostAsJsonAsync(RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var context = factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.AgencyPersonnel.FirstOrDefaultAsync(ap => ap.User.Email == model.Email);
            Assert.Equal(Startup.NewUserId, entity.UserId);
            Assert.Equal(model.Email, entity.User.Email);
            Assert.Equal(model.Name, entity.Name);
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
                Email = Startup.FakePersonnelExisting.User.Email,
                Name = Startup.FakePersonnelExisting.Name
            };
            HttpResponseMessage response = await client.PostAsJsonAsync(RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var context = factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var list = await context.AgencyPersonnel.Where(w => w.UserId == Startup.FakePersonnelExisting.UserId).ToListAsync();
            Assert.Equal(2, list.Count);

            response = await client.PostAsJsonAsync(RequestUri(), model);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<IEnumerable<AgencyPersonnelModel>>();
            AgencyPersonnel entity = Startup.FakePersonnel;
            var model = list.Single(c => c.Id == entity.Id);
            Assert.Equal(entity.Name, model.Name);
            Assert.Equal(entity.User.Email, model.Email);
        }

        [Fact]
        public async Task GetById()
        {
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{Startup.FakePersonnel.Id}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<AgencyPersonnelModel>();
            AgencyPersonnel entity = Startup.FakePersonnel;
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
                            Content = new StringContent(JsonConvert.SerializeObject(true))
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
            Guid id = Startup.FakePersonnelToDelete.Id;
            HttpResponseMessage response = await client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var ctx = factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.False(await ctx.AgencyPersonnel.AnyAsync(a => a.Id == id));
            Assert.False(await ctx.User.AnyAsync(a => a.Id == Startup.FakePersonnelToDelete.UserId));
        }

        public class Startup
        {
            private static readonly Guid AgencyId = Guid.NewGuid();
            public static readonly CvnEmail NewUserEmail = CvnEmail.Create("pepe.payroll@company.com").Value;
            public static readonly Guid NewUserId = Guid.NewGuid();

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddAgencyPersonnelRole(AgencyId);
                    });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
                services.AddSingleton<IIdentityServerService, IdentityServerService>();
            }

            public static readonly AgencyPersonnel FakePersonnel = AgencyPersonnel.CreatePrimary(AgencyId,
                new User(CvnEmail.Create("recruiter@sigook.com").Value), "Recruiter");

            public static readonly AgencyPersonnel FakePersonnelToDelete = AgencyPersonnel.CreatePrimary(AgencyId,
                new User(CvnEmail.Create("delete@sigook.com").Value));

            public static readonly AgencyPersonnel FakePersonnelExisting = AgencyPersonnel.CreatePrimary(Guid.NewGuid(),
                new User(CvnEmail.Create("existing@sigook.com").Value));

            public static readonly User UserInMultipleAgencies = new User(CvnEmail.Create("multiple.agency@sigook.com").Value);
            public static readonly AgencyPersonnel FakePersonnelToDeleteAgency1 = AgencyPersonnel.CreatePrimary(Guid.NewGuid(), UserInMultipleAgencies);
            public static readonly AgencyPersonnel FakePersonnelToDeleteAgency2 = AgencyPersonnel.Create(AgencyId, UserInMultipleAgencies, false);
            public static readonly User FakeNewUser = new User(NewUserEmail);

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
                context.AgencyPersonnel.AddRange(FakePersonnel, FakePersonnelToDelete,
                    FakePersonnelExisting, FakePersonnelToDeleteAgency1, FakePersonnelToDeleteAgency2);
                context.SaveChanges();
            }
        }
    }
}