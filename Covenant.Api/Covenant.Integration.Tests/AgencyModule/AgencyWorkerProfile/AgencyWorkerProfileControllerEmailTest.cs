using Covenant.Api.AgencyModule.AgencyWorkerProfile.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Worker;
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

namespace Covenant.Integration.Tests.AgencyModule.AgencyWorkerProfile
{
    public class AgencyWorkerProfileControllerEmailTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<AgencyWorkerProfileControllerEmailTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private static string RequestUri() => AgencyWorkerProfileController.RouteName;

        public AgencyWorkerProfileControllerEmailTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Email()
        {
            CvnEmail newEmail = CvnEmail.Create("newEmail@mail.com").Value;
            Guid id = Startup.FakeWorker.Id;
            var url = $"{RequestUri()}/{id}/{nameof(AgencyWorkerProfileController.Email)}";
            HttpResponseMessage response = await _client.PutAsJsonAsync(url, new UpdateEmailModel { NewEmail = newEmail });
            response.EnsureSuccessStatusCode();

            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            User updatedUser = await context.User.FirstAsync(f => f.Id == Startup.FakeWorker.WorkerId);
            Assert.Equal(newEmail.Email, updatedUser.Email);
        }

        [Fact]
        public async Task Email_FailsIfEmailIsAlreadyTaken()
        {
            CvnEmail newEmail = Startup.OtherUserEmail;
            Guid id = Startup.FakeWorker.Id;
            var url = $"{RequestUri()}/{id}/{nameof(AgencyWorkerProfileController.Email)}";
            HttpResponseMessage response = await _client.PutAsJsonAsync(url, new UpdateEmailModel { NewEmail = newEmail });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddAgencyPersonnelRole(FakeAgencyUser.Id);
                    });
                services.AddHttpClient();
                services.AddDbContext<CovenantContext>(b =>
                    b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IWorkerRepository, WorkerRepository>();
                services.AddSingleton<IIdentityServerService, IdentityServerService>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
                var mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
                mockMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new
                        {
                            access_token = "Test",
                        }))
                    }).Verifiable();
                var client = new HttpClient(mockMessageHandler.Object)
                {
                    BaseAddress = new Uri("https://localhost:5000/UserAdministration")
                };
                var clientFactoryMock = new Mock<IHttpClientFactory>();
                clientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
                services.AddSingleton(clientFactoryMock.Object);
            }

            public static readonly User UserInStagingEnvironment = new User(CvnEmail.Create("userInStaging@sigook.com").Value, Guid.Parse("0e095ca6-02dc-46d7-b46f-7f2f9f231ac0"));
            public static readonly CvnEmail OtherUserEmail = CvnEmail.Create("other.user@mail.com").Value;
            private static readonly User FakeAgencyUser = new User(CvnEmail.Create($"agency{Guid.NewGuid():N}@email.com").Value);
            public static readonly WorkerProfile FakeWorker = new WorkerProfile(new User(CvnEmail.Create("updateMyEmail@e.com").Value), FakeAgencyUser.Id)
            {
                Location = new Location { City = new City { Province = new Province { Country = new Country { Code = "USA" } } } }
            };
            public static readonly WorkerProfile FakeWorkerInStaging = new WorkerProfile(UserInStagingEnvironment, FakeAgencyUser.Id)
            {
                Location = new Location { City = new City { Province = new Province() } }
            };

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
                context.WorkerProfile.AddRange(FakeWorker, FakeWorkerInStaging);
                context.User.AddRange(new User(OtherUserEmail), UserInStagingEnvironment);
                context.SaveChanges();
            }
        }
    }
}