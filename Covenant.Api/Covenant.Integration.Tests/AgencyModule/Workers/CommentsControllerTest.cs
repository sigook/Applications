using Covenant.Api.Controllers.Sigook.Agency.Workers;
using Covenant.Api.Authorization;
using Covenant.Common.Models.Worker;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.Workers
{
    public class CommentsControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<CommentsControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private const string Url = CommentsController.RouteName;
        private readonly HttpClient _client;
        public CommentsControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post()
        {
            Guid workerProfileId = Startup.FakeWorker.Id;
            string url = Url.Replace("{workerProfileId:guid}", workerProfileId.ToString());
            var model = new CreateCommentModel { Comment = "Bad worker", Rate = 5 };
            HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            WorkerComment entity = await context.WorkerComments.SingleAsync();
            Assert.Equal(model.Comment, entity.Comment);
            Assert.Equal(model.Rate, entity.Rate);
            Assert.Equal(workerProfileId, entity.WorkerProfileId);
            Assert.Null(entity.CompanyProfileId);
        }

        public class Startup
        {
            public static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency { FullName = "Covenant" };
            public static readonly WorkerProfile FakeWorker = new WorkerProfile(new User(CvnEmail.Create("w@s.com").Value), FakeAgency.Id);

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(o =>
                {
                    o.AddAgencyPersonnelRole(FakeAgency.Id);
                });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
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
                context.Agencies.Add(FakeAgency);
                context.WorkerProfiles.Add(FakeWorker);
                context.SaveChanges();
            }
        }
    }
}
