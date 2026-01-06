using Covenant.Api.Shared.EmailPreferences.Controllers;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Models.Notification;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories;
using Covenant.Infrastructure.Repositories.Notification;
using Covenant.Integration.Tests.Configuration;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.Shared.BillDocument
{
    public class EmailPreferencesControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<EmailPreferencesControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        public EmailPreferencesControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post()
        {
            var requestUri = $"{EmailPreferencesController.RouteName}/{nameof(EmailPreferencesController.Unsubscribe)}";
            HttpResponseMessage response = await _client.PostAsJsonAsync(requestUri, new UnsubscribeModel
            {
                UserId = Startup.FakeUser.Id,
                TypeId = NotificationType.NewRequestNotifyWorker.Id.ToString()
            });
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.True(await context.UserNotificationType.AnyAsync());
        }

        public class Startup
        {
            public static readonly User FakeUser = new User(CvnEmail.Create("user@mail.com").Value);
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<INotificationRepository, NotificationRepository>();
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
                context.User.Add(FakeUser);
                context.NotificationType.Add(NotificationType.NewRequestNotifyWorker);
                context.SaveChanges();
            }
        }
    }
}