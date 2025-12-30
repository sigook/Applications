using Covenant.Api.Shared.RequestShift.Controllers;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.Shared.RequestShift
{
    public class RequestShiftControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<RequestShiftControllerTest.Startup>>
    {
        private readonly HttpClient _client;
        public RequestShiftControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestShiftController.RouteName.Replace("{requestId}", Data.FakeRequest.Id.ToString()));
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<ShiftModel>();
            Assert.Equal(Data.FakeRequest.Shift.Monday, model.Monday);
            Assert.Equal(Data.FakeRequest.Shift.MondayStart, model.MondayStart);
            Assert.Equal(Data.FakeRequest.Shift.MondayFinish, model.MondayFinish);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(delegate { });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<ITimeService, TimeService>();
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
                Data.Seed(context);
            }
        }

        private static class Data
        {
            public static readonly Request FakeRequest = Request.AgencyCreateRequest(Guid.NewGuid(), Guid.NewGuid(), FakeData.FakeLocation(), new DateTime(2019, 01, 01), Guid.NewGuid()).Value;
            public static void Seed(CovenantContext context)
            {
                var newShift = new Shift();
                newShift.AddMonday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
                FakeRequest.UpdateShift(newShift);
                context.Request.AddAsync(FakeRequest);
                context.SaveChanges();
            }
        }
    }
}