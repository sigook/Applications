using Covenant.Api.AccountingModule.Deduction;
using Covenant.Api.AccountingModule.Deduction.Cpp;
using Covenant.Common.Models;
using Covenant.Common.Utils.Extensions;
using Covenant.Deductions.Models;
using Covenant.Deductions.Repositories;
using Covenant.Deductions.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Deductions;
using Covenant.Infrastructure.Deductions.Repositories;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AccountingModule.Deduction.Cpp
{
    public class DeductionCppControllerTest :
        IClassFixture<CustomWebApplicationFactory<DeductionCppControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private static readonly string PathTestFiles = Path.Combine(Directory.GetCurrentDirectory(), "Common");

        public DeductionCppControllerTest(CustomWebApplicationFactory<DeductionCppControllerTest.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => DeductionCppController.RouteName;

        [Theory]
        [InlineData("cpp_weekly.xlsx", "Weekly")]
        [InlineData("cpp_biweekly.xlsx", "BiWeekly")]
        [InlineData("cpp_semi_monthly.xlsx", "SemiMonthly")]
        [InlineData("cpp_monthly.xlsx", "Monthly")]
        public async Task Post(string fileName, string period)
        {
            const int year = 2021;
            string imagePath = Path.Combine(PathTestFiles, fileName);
            using (var content = new MultipartFormDataContent(nameof(CreateDeductionModel)))
            {
                byte[] bytes = await File.ReadAllBytesAsync(imagePath);
                content.Add(new StreamContent(new MemoryStream(bytes)), nameof(CreateDeductionModel.File), fileName);
                content.Add(new StringContent(2021.ToString()), nameof(CreateDeductionModel.Year));
                HttpResponseMessage response = await _client.PostAsync($"{RequestUri()}/{period}/Excel", content);
                response.EnsureSuccessStatusCode();

            };
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            switch (period)
            {
                case "Weekly":
                    Assert.True(await context.CppWeekly.AnyAsync(y => y.Year == year));
                    break;
                case "BiWeekly":
                    Assert.True(await context.CppBiWeekly.AnyAsync(y => y.Year == year));
                    break;
                case "SemiMonthly":
                    Assert.True(await context.CppSemiMonthly.AnyAsync(y => y.Year == year));
                    break;
                case "Monthly":
                    Assert.True(await context.CppMonthly.AnyAsync(y => y.Year == year));
                    break;
            }
        }

        [Theory]
        [InlineData("Weekly")]
        [InlineData("BiWeekly")]
        [InlineData("SemiMonthly")]
        [InlineData("Monthly")]
        public async Task Get(string period)
        {
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{period}?Year={Startup.Year}");
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<CppModel>>();
            Assert.NotEmpty(list.Items);
        }

        public class Startup
        {
            public const int Year = 2000;
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder().AddTestAuth(o =>
                {
                    o.AddAgencyPersonnelRole();
                });
                services.AddDbContext<CovenantContext>(b
                    => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IDeductionsRepository, DeductionsRepository>();
                services.AddSingleton<ICppTablesLoader, CppTablesLoader>();
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
                context.CppWeekly.Add(new Deductions.Entities.CppWeekly(1, 1, 1, Year));
                context.CppBiWeekly.Add(new Deductions.Entities.CppBiWeekly(2, 2, 2, Year));
                context.CppSemiMonthly.Add(new Deductions.Entities.CppSemiMonthly(3, 3, 3, Year));
                context.CppMonthly.Add(new Deductions.Entities.CppMonthly(4, 4, 4, Year));
                context.SaveChanges();
            }
        }
    }
}