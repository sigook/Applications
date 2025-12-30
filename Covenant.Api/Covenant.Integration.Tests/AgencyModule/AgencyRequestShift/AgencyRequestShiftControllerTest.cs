using Covenant.Api.AgencyModule.AgencyRequestShift.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyRequestShift
{
    public class AgencyRequestShiftControllerTest :
        IClassFixture<CustomWebApplicationFactory<AgencyRequestShiftControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyRequestShiftControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyRequestShiftController.RouteName.Replace("{requestId}",
            Data.FakeRequest.Id.ToString());

        [Fact]
        public async Task Put()
        {
            var model = new ShiftModel
            {
                Monday = true,
                MondayStart = TimeSpan.Parse("01:00"),
                MondayFinish = TimeSpan.Parse("02:00"),
                Tuesday = true,
                TuesdayStart = TimeSpan.Parse("03:00"),
                TuesdayFinish = TimeSpan.Parse("04:00"),
                Wednesday = true,
                WednesdayStart = TimeSpan.Parse("05:00"),
                WednesdayFinish = TimeSpan.Parse("06:00"),
                Thursday = true,
                ThursdayStart = TimeSpan.Parse("07:00"),
                ThursdayFinish = TimeSpan.Parse("08:00"),
                Friday = true,
                FridayStart = TimeSpan.Parse("09:00"),
                FridayFinish = TimeSpan.Parse("10:00"),
                Saturday = true,
                SaturdayStart = TimeSpan.Parse("11:00"),
                SaturdayFinish = TimeSpan.Parse("12:00"),
                Sunday = true,
                SundayStart = TimeSpan.Parse("13:00"),
                SundayFinish = TimeSpan.Parse("14:00"),
                Comments = new string('C', 10)
            };
            HttpResponseMessage response = await _client.PutAsJsonAsync(RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Request entity = await context.Request.SingleAsync(c => c.Id == Data.FakeRequest.Id);
            Assert.Equal(model.Monday, entity.Shift?.Monday);
            Assert.Equal(model.MondayStart, entity.Shift?.MondayStart);
            Assert.Equal(model.MondayFinish, entity.Shift?.MondayFinish);
            Assert.Equal(model.Tuesday, entity.Shift?.Tuesday);
            Assert.Equal(model.TuesdayStart, entity.Shift?.TuesdayStart);
            Assert.Equal(model.TuesdayFinish, entity.Shift?.TuesdayFinish);
            Assert.Equal(model.Wednesday, entity.Shift?.Wednesday);
            Assert.Equal(model.WednesdayStart, entity.Shift?.WednesdayStart);
            Assert.Equal(model.WednesdayFinish, entity.Shift?.WednesdayFinish);
            Assert.Equal(model.Thursday, entity.Shift?.Thursday);
            Assert.Equal(model.ThursdayStart, entity.Shift?.ThursdayStart);
            Assert.Equal(model.ThursdayFinish, entity.Shift?.ThursdayFinish);
            Assert.Equal(model.Friday, entity.Shift?.Friday);
            Assert.Equal(model.FridayStart, entity.Shift?.FridayStart);
            Assert.Equal(model.FridayFinish, entity.Shift?.FridayFinish);
            Assert.Equal(model.Saturday, entity.Shift?.Saturday);
            Assert.Equal(model.SaturdayStart, entity.Shift?.SaturdayStart);
            Assert.Equal(model.SaturdayFinish, entity.Shift?.SaturdayFinish);
            Assert.Equal(model.Sunday, entity.Shift?.Monday);
            Assert.Equal(model.SundayStart, entity.Shift?.SundayStart);
            Assert.Equal(model.SundayFinish, entity.Shift?.SundayFinish);
            Assert.Equal(model.Comments, entity.Shift?.Comments);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o => o.AddAgencyPersonnelRole());
                services.AddDbContext<CovenantContext>(b
                    => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<IShiftRepository, ShiftRepository>();
                services.AddSingleton<AgencyIdFilter>();
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
                context.CompanyProfile.Add(Data.FakeCompany);
                context.Request.Add(Data.FakeRequest);
                context.SaveChanges();
            }
        }

        private static class Data
        {
            private static readonly Guid AgencyId = Guid.NewGuid();

            internal static readonly CompanyProfile FakeCompany = CompanyProfile.AgencyCreateCompany(
                new User(CvnEmail.Create("c@m.com").Value),
                AgencyId,
                CompanyName.Create("ABX").Value,
                CompanyName.Create("ABX").Value,
                default,
                default,
                default,
                default,
                default,
                new CompanyProfileIndustry("Drivers"),
                default,
                default,
                default,
                default,
                "juan@covenantgroupl.com",
                CompanyStatus.Lead,
                null).Value;

            public static readonly Request FakeRequest;

            static Data()
            {
                FakeCompany.AddJobPositionRate(CompanyProfileJobPositionRate.Create(FakeCompany.Id, "Labor", 1, 1).Value);
                FakeRequest = FakeData.FakeRequest(AgencyId, FakeCompany.CompanyId, FakeCompany.JobPositionRates.First().Id);
            }
        }
    }
}