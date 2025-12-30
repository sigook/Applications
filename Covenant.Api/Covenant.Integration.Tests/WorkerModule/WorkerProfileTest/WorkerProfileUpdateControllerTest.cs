using Covenant.Api;
using Covenant.Api.WorkerModule.WorkerProfile.Controllers;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Worker;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.WorkerModule.WorkerProfileTest
{
    public class WorkerProfileUpdateControllerTest : BaseTestOrder
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly User FakeAgencyUser;
        private readonly Covenant.Common.Entities.Agency.Agency FakeAgency;
        private readonly WorkerProfile FakeWorkerProfile;

        public WorkerProfileUpdateControllerTest()
        {
            _factory = new CustomWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddDefaultTestConfiguration();
                        services.AddTestAuthenticationBuilder()
                            .AddTestAuth(o =>
                            {
                                o.AddSub(FakeAgencyUser.Id);
                                o.AddAgencyPersonnelRole();
                                o.AddName(FakeAgencyUser.Email);
                            });
                        services.AddDbContext<CovenantContext>(b =>
                            b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                        services.AddSingleton<IWorkerRepository, WorkerRepository>();
                    });
                });
            FakeAgencyUser = new User(CvnEmail.Create("FakeAgencyUser@email.com").Value);
            FakeAgency = new Covenant.Common.Entities.Agency.Agency { Id = FakeAgencyUser.Id, User = FakeAgencyUser };
            FakeWorkerProfile = new WorkerProfile(new User(CvnEmail.Create("worker@update.com").Value), FakeAgency.Id)
            {
                Location = new Location { City = new City { Province = new Province { Country = new Country { Code = "USA" } } } }
            };
            _client = _factory.CreateClient();
            Seed(_factory.Services.GetService<CovenantContext>());
        }

        private string RequestUri() => WorkerProfileUpdateController.RouteName.Replace("{profileId}", FakeWorkerProfile.Id.ToString());

        [Fact]
        public async Task PostOtherDocument()
        {
            Guid id = FakeWorkerProfile.Id;
            HttpResponseMessage response = await _client.PostAsJsonAsync($"{RequestUri()}/OtherDocument",
                new CovenantFileModel("void_cheque.pdf", "Bank info"));
            response.EnsureSuccessStatusCode();
            var context = _factory.Services.GetRequiredService<CovenantContext>();
            WorkerProfile entity = await context.WorkerProfile.Where(s => s.Id == id).SingleAsync();
            Assert.NotEmpty(entity.OtherDocuments);
        }

        private void Seed(CovenantContext context)
        {
            context.Agencies.Add(FakeAgency);
            context.WorkerProfile.AddRange(FakeWorkerProfile);
            context.SaveChanges();
        }
    }
}