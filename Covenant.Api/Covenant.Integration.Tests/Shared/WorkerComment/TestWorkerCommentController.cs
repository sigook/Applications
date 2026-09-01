using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Net.Http.Json;

namespace Covenant.Integration.Tests.Shared.WorkerComment
{
    public class TestWorkerCommentController : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<TestWorkerCommentController.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        private static string Uri(Guid workerId) => $"api/worker/{workerId}/comment";
        private readonly HttpClient _client;

        public TestWorkerCommentController(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetList()
        {
            HttpResponseMessage response = await _client.GetAsync(Uri(Data.WorkerProfile.Worker.Id));
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerCommentModel>>();
            Assert.NotEmpty(list.Items);
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
                    o.AddCompanyRole();
                });
                services.AddTestDatabase();
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
                context.WorkerComments.AddRange(Data.Comment, Data.CompanyComment);
                context.SaveChanges();
            }
        }

        private static class Data
        {
            public static readonly User LoginUser = new User(CvnEmail.Create("login_user@mail.com").Value);
            public static readonly WorkerProfile WorkerProfile = new WorkerProfile(new User(CvnEmail.Create("worker_worker@mail.com").Value))
            {
                Agency = new Covenant.Common.Entities.Agency.Agency { Id = LoginUser.Id, User = LoginUser }
            , Location = FakeData.FakeLocation(),};

            public static readonly CompanyProfile CompanyProfile = new CompanyProfile { Company = LoginUser, Logo = new CovenantFile("logo.png") , Industry = new CompanyProfileIndustry("Test") , Agency = FakeData.FakeAgency() };
            public static readonly Covenant.Common.Entities.Worker.WorkerComment Comment =
                Covenant.Common.Entities.Worker.WorkerComment.CommentPostByAgency(WorkerProfile.Id, "Ok", 1);
            public static readonly Covenant.Common.Entities.Worker.WorkerComment CompanyComment =
                Covenant.Common.Entities.Worker.WorkerComment.CommentPostByCompany(WorkerProfile.Id, CompanyProfile.Id, "Posted by the company", 3);
        }
    }
}
