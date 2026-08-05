using Covenant.Api.Authorization;
using Covenant.Api.CompanyModule.CompanyWorkerComment.Controllers;
using Covenant.Common.Models.Worker;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.CompanyModule.CompanyWorkerComment
{
    public class CompanyWorkerCommentControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<CompanyWorkerCommentControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private const string Url = CompanyWorkerCommentController.RouteName;
        private readonly HttpClient _client;
        public CompanyWorkerCommentControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post()
        {
            Guid workerProfileId = Startup.FakeWorker.Id;
            string url = Url.Replace("{workerProfileId:guid}", workerProfileId.ToString());
            var model = new CreateCommentModel { Comment = "Good worker", Rate = 5 };
            HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            WorkerComment entity = await context.WorkerComments.SingleAsync();
            Assert.Equal(model.Comment, entity.Comment);
            Assert.Equal(model.Rate, entity.Rate);
            Assert.Equal(workerProfileId, entity.WorkerProfileId);
            Assert.Equal(Startup.FakeCompany.Id, entity.CompanyProfileId);
        }

        public class Startup
        {
            private static readonly Covenant.Common.Entities.Agency.Agency Agency = new Covenant.Common.Entities.Agency.Agency { FullName = "Covenant" };
            public static readonly CompanyProfile FakeCompany = new CompanyProfile { Company = new User(CvnEmail.Create("company@company.com").Value), Agency = Agency };
            public static readonly WorkerProfile FakeWorker = new WorkerProfile(new User(CvnEmail.Create("w@s.com").Value), Agency.Id);

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(o =>
                {
                    o.AddSub(FakeCompany.Company.Id);
                    o.AddCompanyRole();
                });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<CompanyIdFilter>();
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
                context.WorkerProfiles.Add(FakeWorker);
                context.CompanyProfiles.Add(FakeCompany);
                context.SaveChanges();
            }
        }
    }
}
