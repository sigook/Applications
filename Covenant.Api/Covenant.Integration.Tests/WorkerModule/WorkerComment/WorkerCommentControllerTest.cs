using Covenant.Api.WorkerModule.WorkerComment.Controllers;
using Covenant.Common.Entities;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.WorkerModule.WorkerComment
{
    public class WorkerCommentControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<WorkerCommentControllerTest.Startup>>
    {
        private readonly HttpClient _client;

        public WorkerCommentControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetList()
        {
            HttpResponseMessage response = await _client.GetAsync(WorkerCommentController.RouteName);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerCommentModel>>();
            Assert.Single(list.Items);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(o =>
                {
                    o.AddSub(Data.WorkerProfile.WorkerId);
                    o.AddWorkerRole();
                });
                services.AddTestDatabase();
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
                context.WorkerProfiles.Add(Data.WorkerProfile);
                context.WorkerComments.Add(Data.AgencyComment);
                context.SaveChanges();
            }
        }

        internal static class Data
        {
            public static readonly Common.Entities.Worker.WorkerProfile WorkerProfile =
                new Common.Entities.Worker.WorkerProfile(new User(CvnEmail.Create("worker_me@mail.com").Value))
                {
                    Agency = FakeData.FakeAgency(),
                    Location = FakeData.FakeLocation()
                };

            public static readonly Common.Entities.Worker.WorkerComment AgencyComment =
                Common.Entities.Worker.WorkerComment.CommentPostByAgency(WorkerProfile.Id, "Ok", 1);
        }
    }
}
