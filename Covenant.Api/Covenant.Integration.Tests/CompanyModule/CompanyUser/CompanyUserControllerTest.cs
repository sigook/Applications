using Covenant.Api.Authorization;
using Covenant.Api.CompanyModule.CompanyUser.Controllers;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.CompanyModule.CompanyUser
{
    public class CompanyUserControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<CompanyUserControllerTest.Startup>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;

        public CompanyUserControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private static string RequestUri() => CompanyUserController.RouteName;

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
            var model = new CompanyUserModel
            {
                Email = Startup.NewUserEmail,
                Name = "Pepe",
                Lastname = "Johns",
                MobileNumber = "6478769065",
                Position = "Supervisor"
            };
            var response = await client.PostAsJsonAsync(RequestUri(), model);
            response.EnsureSuccessStatusCode();

            var entity = await factory.Server.Host.Services.GetRequiredService<CovenantContext>().CompanyUser
                .FirstOrDefaultAsync(cu => cu.User.Email == Startup.NewUserEmail);
            Assert.Equal(Startup.NewUserId, entity.Id);
            Assert.Equal(model.Email, entity.User.Email);
            Assert.Equal(model.Name, entity.Name);
            Assert.Equal(model.Lastname, entity.Lastname);
            Assert.Equal(model.MobileNumber, entity.MobileNumber);
            Assert.Equal(model.Position, entity.Position);
        }

        [Fact]
        public async Task Put()
        {
            var model = new CompanyUserModel
            {
                Name = "Sandra",
                Lastname = "Mills",
                MobileNumber = "6477658970",
                Position = "Manager"
            };
            Guid id = Startup.FakeCompanyUserToUpdate.Id;
            var response = await _client.PutAsJsonAsync($"{RequestUri()}/{id}", model);
            response.EnsureSuccessStatusCode();

            var entity = await _factory.Server.Host.Services.GetRequiredService<CovenantContext>().CompanyUser.FindAsync(id);
            Assert.Equal(model.Name, entity.Name);
            Assert.Equal(model.Lastname, entity.Lastname);
            Assert.Equal(model.MobileNumber, entity.MobileNumber);
            Assert.Equal(model.Position, entity.Position);
        }

        [Fact]
        public async Task Get()
        {
            var response = await _client.GetAsync(RequestUri());
            var list = await response.Content.ReadAsJsonAsync<IEnumerable<CompanyUserModel>>();
            Assert.NotEmpty(list);
            var entity = Startup.FakeCompanyUser;
            var detail = list.Single(s => s.Id == entity.Id);
            Assert.Equal(entity.User.Email, detail.Email);
            Assert.Equal(entity.Name, detail.Name);
            Assert.Equal(entity.Lastname, detail.Lastname);
            Assert.Equal(entity.MobileNumber, detail.MobileNumber);
            Assert.Equal(entity.Position, detail.Position);
            Assert.Equal(entity.CreatedAt, detail.CreatedAt);
        }

        [Fact]
        public async Task GetDetail()
        {
            var response = await _client.GetAsync($"{RequestUri()}/detail");
            var detail = await response.Content.ReadAsJsonAsync<CompanyUserModel>();
            var entity = Startup.FakeCompanyUser;
            Assert.Equal(entity.Id, detail.Id);
            Assert.Equal(entity.User.Email, detail.Email);
            Assert.Equal(entity.Name, detail.Name);
            Assert.Equal(entity.Lastname, detail.Lastname);
            Assert.Equal(entity.MobileNumber, detail.MobileNumber);
            Assert.Equal(entity.Position, detail.Position);
            Assert.Equal(entity.CreatedAt, detail.CreatedAt);
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
            Guid id = Startup.FakeCompanyUserToDelete.Id;
            var response = await client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var ctx = factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.False(await ctx.CompanyUser.AnyAsync(a => a.Id == id));
            Assert.False(await ctx.User.AnyAsync(a => a.Id == id));
        }

        public class Startup
        {
            private static readonly Guid CompanyId = Guid.NewGuid();
            public static readonly CvnEmail NewUserEmail = CvnEmail.Create("pepe.supervisor@company.com").Value;
            public static readonly Guid NewUserId = Guid.NewGuid();
            public static readonly Covenant.Common.Entities.Company.CompanyUser FakeCompanyUser = new Covenant.Common.Entities.Company.CompanyUser(CompanyId, new User(CvnEmail.Create("get@mail.com").Value))
            {
                Name = "George",
                Lastname = "Perez",
                MobileNumber = "6574568970",
                Position = "Manager",
                CreatedAt = new DateTime(2019, 01, 01),
                CreatedBy = "abc@mail.com"
            };
            public static readonly Covenant.Common.Entities.Company.CompanyUser FakeCompanyUserToUpdate = new Covenant.Common.Entities.Company.CompanyUser(CompanyId, new User(CvnEmail.Create("update@sigook.com").Value));
            public static readonly Covenant.Common.Entities.Company.CompanyUser FakeCompanyUserToDelete = new Covenant.Common.Entities.Company.CompanyUser(CompanyId, new User(CvnEmail.Create("delete@sigook.com").Value));

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(FakeCompanyUser.Id);
                        o.AddCompanyUserRole();
                    });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<CompanyIdFilter>();
                services.AddSingleton<IIdentityServerService, IdentityServerService>();
            }

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
                context.CompanyUser.AddRange(FakeCompanyUser,
                    FakeCompanyUserToUpdate, FakeCompanyUserToDelete);
                context.SaveChanges();
            }
        }
    }
}