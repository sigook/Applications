using Covenant.Api.Shared.WorkerAvailableToInvite.Controllers;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Worker;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.Shared.WorkerAvailableToInvite
{
    public class WorkerAvailableToInviteControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<WorkerAvailableToInviteControllerTest.Startup>>
    {
        private readonly HttpClient _client;

        public WorkerAvailableToInviteControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

        private static string RequestUri() => WorkerAvailableToInviteController.RouteName;

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o => { });
                services.AddDbContext<CovenantContext>(b =>
                    b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IWorkerRepository, WorkerRepository>();
            }

            private static readonly User FakeAgencyUser = new User(CvnEmail.Create("FakeAgencyUser@email.com").Value);
            public static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency { Id = FakeAgencyUser.Id, User = FakeAgencyUser };

            public static readonly WorkerProfile FakeWorker = new WorkerProfile(new User(CvnEmail.Create("some@e.com").Value)) { Agency = FakeAgency };

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
                context.WorkerProfile.AddRange(FakeWorker);
                context.NotificationType.Add(NotificationType.NewRequestNotifyWorker);
                context.UserNotificationType.Add(new UserNotificationType(FakeWorker.WorkerId, NotificationType.NewRequestNotifyWorker.Id) { EmailNotification = true });
                context.SaveChanges();
            }
        }
    }
}