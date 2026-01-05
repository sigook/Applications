using Covenant.Api.Shared.WorkerComment.Models;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Worker;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.Shared.WorkerComment
{
    public class TestWorkerCommentController : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<TestWorkerCommentController.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        private static string Uri(Guid workerId, Guid? id = null)
            => $"api/worker/{workerId}/comment{(id is null ? string.Empty : $"/{id}")}";
        private readonly HttpClient _client;

        public TestWorkerCommentController(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AgencyPostComment()
        {
            var url = $"{Uri(Data.WorkerProfile.Worker.Id)}/Agency";
            var model = new CreateCommentModel { Comment = "Good worker says agency", Rate = 4 };
            HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
            response.EnsureSuccessStatusCode();
            response = await _client.GetAsync(response.Headers.Location);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<WorkerCommentModel>();
            Assert.Equal(model.Comment, detail.Comment);
            Assert.Equal(model.Rate, detail.Rate);
        }
        [Fact]
        public async Task CompanyPostComment()
        {
            var url = $"{Uri(Data.WorkerProfile.Worker.Id)}/Company";
            var model = new CreateCommentModel { Comment = "Bad worker says company", Rate = 4 };
            HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
            response.EnsureSuccessStatusCode();
            response = await _client.GetAsync(response.Headers.Location);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<WorkerCommentModel>();
            Assert.Equal(model.Comment, detail.Comment);
            Assert.Equal(model.Rate, detail.Rate);
        }

        [Fact]
        public async Task Put()
        {
            var model = new CreateCommentModel { Comment = "Good worker", Rate = 5 };
            var url = Uri(Data.WorkerProfile.Worker.Id, Data.Comment.Id);
            HttpResponseMessage response = await _client.PutAsJsonAsync(url, model);
            response.EnsureSuccessStatusCode();
            response = await _client.GetAsync(url);
            var detail = await response.Content.ReadAsJsonAsync<WorkerCommentModel>();
            Assert.Equal(model.Comment, detail.Comment);
            Assert.Equal(model.Rate, detail.Rate);
        }

        [Fact]
        public async Task GetList()
        {
            HttpResponseMessage response = await _client.GetAsync(Uri(Data.WorkerProfile.Worker.Id));
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<WorkerCommentModel>>();
            Assert.NotEmpty(list.Items);
            Assert.All(list.Items, m => Assert.NotNull(m.Logo));
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(o =>
                {
                    o.AddSub(Data.LoginUser.Id);
                    o.AddAgencyPersonnelRole();
                    o.AddCompanyRole();
                });
                services.AddDbContext<CovenantContext>(
                    b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IWorkerCommentsRepository, WorkerCommentsRepository>();
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
                context.WorkerProfile.Add(Data.WorkerProfile);
                context.CompanyProfile.Add(Data.CompanyProfile);
                context.WorkerComment.Add(Data.Comment);
                context.SaveChanges();
            }
        }

        private static class Data
        {
            public static readonly User LoginUser = new User(CvnEmail.Create("login_user@mail.com").Value);
            public static readonly WorkerProfile WorkerProfile = new WorkerProfile(new User(CvnEmail.Create("worker_worker@mail.com").Value))
            {
                Agency = new Covenant.Common.Entities.Agency.Agency { Id = LoginUser.Id, User = LoginUser }
            };

            public static readonly CompanyProfile CompanyProfile = new CompanyProfile { Company = LoginUser, Logo = new CovenantFile("logo.png") };
            public static readonly Covenant.Common.Entities.Worker.WorkerComment Comment = Covenant.Common.Entities.Worker.WorkerComment.CommentPostByCompany(WorkerProfile.Worker.Id, CompanyProfile.Company.Id, "Ok", 1);
        }
    }
}