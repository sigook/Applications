using Covenant.Common.Constants;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;

namespace Covenant.Integration.Tests.Agencies.Recruiting.Clients;

public class ClientsTestData : ITestData
{
    public User Recruiter { get; } = FakeData.FakeUser($"recruiter.{Guid.NewGuid():N}@clients.com");
    public User Sales { get; } = FakeData.FakeUser($"sales.{Guid.NewGuid():N}@clients.com");
    public User OtherSales { get; } = FakeData.FakeUser($"other.sales.{Guid.NewGuid():N}@clients.com");
    public Agency Agency { get; } = FakeData.FakeAgency();
    public Agency ForeignAgency { get; } = FakeData.FakeAgency();
    public AgencyPersonnel SalesPersonnel { get; }
    public AgencyPersonnel OtherSalesPersonnel { get; }
    public CompanyProfile Company { get; }
    public CompanyProfile OtherCompany { get; }
    public CompanyProfile SalesCompany { get; }
    public CompanyProfile OtherSalesCompany { get; }
    public CompanyProfile ForeignCompany { get; }

    public ClientsTestData()
    {
        SalesPersonnel = AgencyPersonnel.CreatePrimary(Agency.Id, Sales.Id, "Sales Rep");
        OtherSalesPersonnel = AgencyPersonnel.CreatePrimary(Agency.Id, OtherSales.Id, "Other Rep");
        Company = Client("Alpha Client");
        OtherCompany = Client("Beta Client");
        SalesCompany = Client("Gamma Sales Client").With(c => c.UpdateDetails(null, null, SalesPersonnel.Id));
        OtherSalesCompany = Client("Delta Other Sales Client").With(c => c.UpdateDetails(null, null, OtherSalesPersonnel.Id));
        ForeignCompany = FakeData.FakeCompanyProfileForAgency(ForeignAgency.Id, "Foreign Client", companyEmail: $"foreign.{Guid.NewGuid():N}@clients.com");
    }

    private CompanyProfile Client(string name) =>
        FakeData.FakeCompanyProfileForAgency(Agency.Id, name, companyEmail: $"{Guid.NewGuid():N}@clients.com");

    public HttpClient ClientAs(Func<HttpClient> create, string role, User user)
    {
        HttpClient client = create();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, role);
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.SubHeader, user.Id.ToString());
        return client;
    }

    public HttpClient AsRecruiter(Func<HttpClient> create) => ClientAs(create, CovenantConstants.Role.Recruiting, Recruiter);

    public virtual void Seed(CovenantContext context)
    {
        context.Agencies.AddRange(Agency, ForeignAgency);
        context.Users.AddRange(Recruiter, Sales, OtherSales);
        context.AgencyPersonnel.AddRange(AgencyPersonnel.CreatePrimary(Agency.Id, Recruiter.Id, "Recruiter"), SalesPersonnel, OtherSalesPersonnel);
        context.AddRange(Company, OtherCompany, SalesCompany, OtherSalesCompany, ForeignCompany);
        context.SaveChanges();
    }
}

public class ClientsTestStartup
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
