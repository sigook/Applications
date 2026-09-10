using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Sigook.Agency.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.Workers
{
    public class CommentsControllerGetTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<CommentsControllerGetTest.Startup>>
    {
        private readonly HttpClient _client;

        public CommentsControllerGetTest(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetList()
        {
            string url = CommentsController.RouteName.Replace("{workerProfileId:guid}", Data.WorkerProfile.Id.ToString());
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerCommentModel>>();
            Assert.Equal(2, list.Items.Count);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(o =>
                {
                    o.AddSub(Data.LoginUser.Id);
                    o.AddAgencyPersonnelRole(Data.LoginUser.Id);
                });
                services.AddTestDatabase();
                services.AddSingleton<AgencyIdFilter>();
                var timeService = new Mock<ITimeService>();
                timeService.Setup(s => s.GetCurrentDateTime()).Returns(new DateTime(2019, 01, 01));
                services.AddSingleton(timeService.Object);
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
                context.CompanyProfiles.Add(Data.CompanyProfile);
                context.WorkerComments.AddRange(Data.AgencyComment, Data.CompanyComment);
                context.SaveChanges();
            }
        }

        internal static class Data
        {
            public static readonly User LoginUser = new User(CvnEmail.Create("login_user@mail.com").Value);

            public static readonly Common.Entities.Worker.WorkerProfile WorkerProfile =
                new Common.Entities.Worker.WorkerProfile(new User(CvnEmail.Create("worker_worker@mail.com").Value))
                {
                    Agency = new Common.Entities.Agency.Agency { Id = LoginUser.Id, User = LoginUser },
                    Location = FakeData.FakeLocation()
                };

            public static readonly CompanyProfile CompanyProfile = new CompanyProfile
            {
                Company = LoginUser,
                Logo = new CovenantFile("logo.png"),
                Industry = new CompanyProfileIndustry("Test"),
                Agency = FakeData.FakeAgency()
            };

            public static readonly Common.Entities.Worker.WorkerComment AgencyComment =
                Common.Entities.Worker.WorkerComment.CommentPostByAgency(WorkerProfile.Id, "Ok", 1);

            public static readonly Common.Entities.Worker.WorkerComment CompanyComment =
                Common.Entities.Worker.WorkerComment.CommentPostByCompany(WorkerProfile.Id, CompanyProfile.Id, "Posted by the company", 3);
        }
    }
}
