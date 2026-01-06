using Covenant.Common.Entities;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models.Worker;
using Covenant.Documents;
using Covenant.Infrastructure.Context;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.Shared.WorkersReportDocument
{
    public class WorkersReportDocumentControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;

        public WorkersReportDocumentControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            client = factory.CreateClient();
        }

        [Fact]
        public async void Should_Get_ExcelFile_Of_Workers()
        {
            var response = await client.GetAsync($"api/WorkersReportDocument/057d28b0-7d09-4e8e-aca4-a22a97943770/Document");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync();
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"doc_{Guid.NewGuid():N}.xlsx");
            using (var file = File.Create(path))
            {
                file.Seek(0, SeekOrigin.Begin);
                await content.CopyToAsync(file);
            }
            Assert.True(File.Exists(path));
            File.Delete(path);
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ServicesConfiguration).Assembly));
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o => { });
            services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
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
            context.Seed();
        }
    }

    public static class Data
    {
        public static void Seed(this CovenantContext context)
        {
            var basicInformation = new Mock<IWorkerBasicInformation<ICatalog<Guid>>>();
            basicInformation.SetupGet(i => i.FirstName).Returns("Juan");
            basicInformation.SetupGet(i => i.MiddleName).Returns("Sebastian");
            basicInformation.SetupGet(i => i.LastName).Returns("Gonzalez");
            basicInformation.SetupGet(i => i.SecondLastName).Returns("Mejia");
            var sinInformation = new Mock<ISinInformation<ICovenantFile>>();
            sinInformation.SetupGet(s => s.SocialInsurance).Returns("4561516587");
            sinInformation.SetupGet(s => s.DueDate).Returns(DateTime.Now);
            sinInformation.SetupGet(s => s.SocialInsuranceExpire).Returns(true);
            var city = new Mock<ICatalog<Guid>>();
            city.SetupGet(c => c.Id).Returns(Guid.NewGuid());
            var location = new Mock<ILocation<ICatalog<Guid>>>();
            location.SetupGet(l => l.City).Returns(city.Object);
            location.SetupGet(l => l.Address).Returns("Street False 123");
            location.SetupGet(l => l.PostalCode).Returns("K1A0B1");
            var contactInformation = new Mock<IWorkerContactInformation<ILocation<ICatalog<Guid>>, ICatalog<Guid>>>();
            contactInformation.SetupGet(i => i.Location).Returns(location.Object);
            contactInformation.SetupGet(c => c.MobileNumber).Returns("3102793995");
            var workerProfile = new WorkerProfile(new User(CvnEmail.Create("some@hotmail.com").Value, Guid.NewGuid()))
            {
                Id = Guid.NewGuid(),
                NumberId = 1,
                ApprovedToWork = true,
                AgencyId = Guid.Empty,
            };
            workerProfile.PatchBasicInformation(basicInformation.Object);
            workerProfile.PatchSinInformation(sinInformation.Object);
            workerProfile.PatchContactInformation(contactInformation.Object);
            var workerRequest = Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(workerProfile.WorkerId, Guid.Parse("057d28b0-7d09-4e8e-aca4-a22a97943770"));
            workerRequest.UpdateStartWorking(DateTime.Now);
            context.Add(workerProfile);
            context.Add(workerRequest);
            context.SaveChanges();
        }
    }
}
