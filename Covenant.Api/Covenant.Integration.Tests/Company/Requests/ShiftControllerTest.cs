using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Company.Requests;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Requests;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Requests;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.Company.Requests;

public class ShiftControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<ShiftControllerTest.Startup>>
{
    private readonly HttpClient _client;
    public ShiftControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Get()
    {
        HttpResponseMessage response = await _client.GetAsync(ShiftController.RouteName.Replace("{requestId}", Data.FakeRequest.Id.ToString()));
        response.EnsureSuccessStatusCode();
        var model = await response.Content.ReadFromJsonAsync<ShiftModel>();
        Assert.Equal(Data.FakeRequest.Shift.Monday, model.Monday);
        Assert.Equal(Data.FakeRequest.Shift.MondayStart, model.MondayStart);
        Assert.Equal(Data.FakeRequest.Shift.MondayFinish, model.MondayFinish);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o => o.AddCompanyRole());
            services.AddTestDatabase();
            services.AddSingleton<IRequestRepository, RequestRepository>();
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
            Data.Seed(context);
        }
    }

    private static class Data
    {
        public static readonly Request FakeRequest = FakeData.FakeRequest(startAt: new DateTime(2019, 01, 01));
        public static void Seed(CovenantContext context)
        {
            var newShift = new Shift();
            newShift.AddMonday(TimeSpan.Parse("08:00"), TimeSpan.Parse("16:00"));
            FakeRequest.UpdateShift(newShift);
            context.Requests.Add(FakeRequest);
            context.SaveChanges();
        }
    }
}
