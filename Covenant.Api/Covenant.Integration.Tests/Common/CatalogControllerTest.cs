using Covenant.Api.Controllers.Sigook;
using Covenant.Common.Entities;
using Covenant.Common.Models;
using Covenant.Common.Repositories;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories;
using Covenant.Integration.Tests.Configuration;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Xunit;

namespace Covenant.Integration.Tests.Common;

public class CatalogControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<CatalogControllerTest.Startup>>
{
    private readonly HttpClient _client;
    private const string Url = CatalogController.RouteName;

    public CatalogControllerTest(CustomWebApplicationFactory<CatalogControllerTest.Startup> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task JobPosition()
    {
        HttpResponseMessage response = await _client.GetAsync($"{Url}/jobPosition");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadAsJsonAsync<List<JobPositionDetailModel>>();
        Assert.NotEmpty(list);
        Assert.Single(list, l =>
            l.Id == Startup.FaeJobPosition.Id &&
            l.Value == Startup.FaeJobPosition.Value &&
            l.Industry == Startup.FaeJobPosition.Industry.Value);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddDbContext<CovenantContext>(c =>
                c.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
            services.AddSingleton<ICatalogRepository, CatalogRepository>();
            services.AddAutoMapper(Assembly.Load("Covenant.Api"));
            services.AddResponseCaching();
        }

        public static readonly JobPosition FaeJobPosition = new JobPosition { Value = "Labour", Industry = new Industry { Value = "General Labour" } };
        public static readonly Country Canada = Country.Canada;
        public static readonly Country UnitedStates = Country.UnitedStates;
        public static readonly Province Ontario = new Province { Code = "ON", Value = "Ontario", Country = Canada };
        public static readonly Province Florida = new Province { Code = "FL", Value = "Florida", Country = UnitedStates };
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
            context.JobPosition.Add(FaeJobPosition);
            context.Country.AddRange(Canada, UnitedStates);
            context.Province.AddRange(Ontario, Florida);
            context.SaveChanges();
        }
    }
}