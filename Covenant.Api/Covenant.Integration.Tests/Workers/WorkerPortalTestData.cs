using Covenant.Common.Constants;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;

namespace Covenant.Integration.Tests.Workers;

public class WorkerPortalTestData : ITestData
{
    public City City { get; } = FakeData.FakeCity();
    public City OtherCity { get; } = FakeData.FakeCity();
    public Agency Agency { get; }
    public WorkerProfile Profile { get; }
    public Availability FullTime { get; } = new("Full time");
    public Availability PartTime { get; } = new("Part time");
    public AvailabilityTime Morning { get; } = new("Morning");
    public Day Monday { get; } = new("Monday");
    public Language English { get; } = new("English");
    public Language French { get; } = new("French");
    public Lift Lift { get; } = new("50 lbs");
    public Request CancelledRequest { get; }
    public Request OpenRequest { get; }

    public WorkerPortalTestData()
    {
        Agency = FakeData.FakeAgency(city: City);
        Profile = FakeData.FakeWorkerProfile(Agency, $"worker.{Guid.NewGuid():N}@portal.com", City)
            .With(w => { w.FirstName = "Wendy"; w.LastName = "Worker"; });
        CancelledRequest = FakeData.FakeRequest(agencyId: Agency.Id, startAt: new DateTime(2019, 01, 01));
        OpenRequest = FakeData.FakeRequest(agencyId: Agency.Id, startAt: new DateTime(2019, 01, 01));
        CancelledRequest.Cancel(new DateTime(2019, 01, 02));
    }

    public HttpClient AsWorker(Func<HttpClient> create)
    {
        HttpClient client = create();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, CovenantConstants.Role.Worker);
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.SubHeader, Profile.WorkerId.ToString());
        return client;
    }

    public virtual void Seed(CovenantContext context)
    {
        context.Agencies.Add(Agency);
        context.Cities.Add(OtherCity);
        context.WorkerProfiles.Add(Profile);
        context.Availabilities.AddRange(FullTime, PartTime);
        context.AvailabilityTimes.Add(Morning);
        context.Days.Add(Monday);
        context.Languages.AddRange(English, French);
        context.Lifts.Add(Lift);
        context.Requests.AddRange(CancelledRequest, OpenRequest);
        context.WorkerRequests.AddRange(WorkerRequest.AgencyBook(Profile.Id, CancelledRequest.Id), WorkerRequest.AgencyBook(Profile.Id, OpenRequest.Id));
        context.SaveChanges();
    }
}

public class WorkerPortalTestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDefaultTestConfiguration();
        services.AddTestAuthenticationBuilder().AddTestAuth(_ => { });
        services.AddTestDatabase();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
