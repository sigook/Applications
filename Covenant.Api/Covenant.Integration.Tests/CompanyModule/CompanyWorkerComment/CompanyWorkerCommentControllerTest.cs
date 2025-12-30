using Covenant.Api.Authorization;
using Covenant.Api.CompanyModule.CompanyWorkerComment.Controllers;
using Covenant.Api.Shared.WorkerComment.Models;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Repositories.Worker;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
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
            Guid workerId = Startup.FakeWorker.Worker.Id;
            string url = Url.Replace("{workerId:guid}", workerId.ToString());
            var model = new CreateCommentModel { Comment = "Good worker", Rate = 5 };
            HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            WorkerComment entity = await context.WorkerComment.SingleAsync();
            Assert.Equal(model.Comment, entity.Comment);
            Assert.Equal(model.Rate, entity.Rate);
            Assert.Equal(workerId, entity.WorkerId);
            Assert.Equal(Startup.FakeCompany.Company.Id, entity.CompanyId);
            Assert.Null(entity.AgencyId);
            response = await _client.GetAsync(response.Headers.Location);
            response.EnsureSuccessStatusCode();
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
                services.AddSingleton<IWorkerCommentsRepository, WorkerCommentsRepository>();
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
                context.WorkerProfile.Add(FakeWorker);
                context.CompanyProfile.Add(FakeCompany);
                context.SaveChanges();
            }
        }
    }
}