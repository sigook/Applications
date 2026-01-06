using Covenant.Api.WorkerModule.WorkerRequest.Controllers;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Integration.Tests.Common;
using Covenant.Integration.Tests.Configuration;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.WorkerModule.WorkerRequest
{
    public class WorkerRequestControllerApplyAnonymousTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<WorkerRequestControllerApplyAnonymousTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        public WorkerRequestControllerApplyAnonymousTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => WorkerRequestController.RouteName;

        [Fact]
        public async Task Apply()
        {
            var model = new WorkerRequestApplyModel { Comments = "Hard Worker" };
            var url = $"{RequestUri()}/{WorkerRequestControllerTest.Data.WorkerProfile.Worker.Id}/{WorkerRequestControllerTest.Data.FakeRequest.Id}/Apply";
            HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<RequestApplicantDetailModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.RequestApplicant.SingleAsync(s => s.Id == detail.Id);
            Assert.Equal(detail.WorkerProfileId, entity.WorkerProfileId);
            Assert.Equal(model.Comments, entity.Comments);
            Assert.Null(entity.CandidateId);
            response = await _client.PostAsJsonAsync(url, model);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                var timeService = new Mock<ITimeService>();
                timeService.Setup(s => s.GetCurrentDateTime()).Returns(WorkerRequestControllerTest.Data.Now);
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
                context.Request.Add(WorkerRequestControllerTest.Data.FakeRequest);
                context.CompanyProfile.Add(WorkerRequestControllerTest.Data.CompanyProfile);
                context.CompanyProfileJobPositionRate.Add(WorkerRequestControllerTest.Data.FakeRate);
                context.WorkerProfile.Add(WorkerRequestControllerTest.Data.WorkerProfile);
                context.SaveChanges();
            }
        }
    }
}