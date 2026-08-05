using Covenant.Api.Controllers.Sigook;
using Covenant.Common.Entities;
using Covenant.Common.Models;
using Covenant.Common.Repositories;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories;
using Covenant.Integration.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Xunit;
using System.Net.Http.Json;

namespace Covenant.Integration.Tests.Common;

public class CatalogControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<CatalogControllerTest.Startup>>
{
    private readonly HttpClient _client;
    private const string Url = CatalogController.RouteName;

    public CatalogControllerTest(CustomWebApplicationFactory<CatalogControllerTest.Startup> factory)
    {
        _client = factory.CreateClient();
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddDbContext<CovenantContext>(c =>
                c.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
            services.AddSingleton<ICatalogRepository, CatalogRepository>();
            services.AddResponseCaching();
        }

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
            context.Countries.AddRange(Canada, UnitedStates);
            context.Provinces.AddRange(Ontario, Florida);
            context.SaveChanges();
        }
    }
}
