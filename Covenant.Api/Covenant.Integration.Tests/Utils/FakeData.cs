using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;

namespace Covenant.Integration.Tests.Utils;

public static class FakeData
{
    public static User FakeUser(string email = null) =>
        new(CvnEmail.Create(email ?? $"user.{Guid.NewGuid():N}@test.com").Value);

    public static Country FakeCountry(string code = "CA") => new() { Id = Guid.NewGuid(), Code = code };

    public static Province FakeProvince(Country country = null) => new() { Country = country ?? FakeCountry() };

    public static City FakeCity(Province province = null) => new() { Province = province ?? FakeProvince() };

    public static Location FakeLocation(City city = null)
    {
        var location = new Location
        {
            Address = "424 Dundas",
            PostalCode = "M3P1M7",
            City = city ?? FakeCity()
        };
        location.UpdateCoordinates(Location.DefaultLatitude, Location.DefaultLongitude);
        return location;
    }

    public static Covenant.Common.Entities.Agency.Agency FakeAgency(Guid id = default, City city = null)
    {
        id = id == default ? Guid.NewGuid() : id;
        var agency = new Covenant.Common.Entities.Agency.Agency
        {
            Id = id,
            FullName = "Test Agency",
            RecruitmentEmail = "recruit@test.com",
            User = FakeUser($"agency.{id:N}@test.com")
        };
        agency.AddLocation(FakeLocation(city), true);
        return agency;
    }

    public static CompanyProfile FakeCompanyProfile(Covenant.Common.Entities.Agency.Agency agency = null,
        string fullName = "Test Company", City city = null, Guid id = default, string companyEmail = null)
    {
        agency ??= FakeAgency(city: city);
        var companyProfile = new CompanyProfile(FakeUser(companyEmail), agency, fullName, "4165551234",
            new CompanyProfileIndustry("Test"));
        if (id != default) companyProfile.Id = id;
        companyProfile.AgencyId = agency.Id;
        companyProfile.AddLocation(FakeLocation(city), true);
        return companyProfile;
    }

    public static CompanyProfile FakeCompanyProfileForAgency(Guid agencyId, string fullName = "Test Company",
        City city = null, Guid id = default, string companyEmail = null)
    {
        var companyProfile = new CompanyProfile
        {
            Company = FakeUser(companyEmail),
            AgencyId = agencyId,
            FullName = fullName,
            Phone = "4165551234",
            Industry = new CompanyProfileIndustry("Test")
        };
        if (id != default) companyProfile.Id = id;
        companyProfile.AddLocation(FakeLocation(city), true);
        return companyProfile;
    }

    public static CompanyProfileJobPositionRate FakeJobPositionRate(CompanyProfile companyProfile,
        string jobPosition = "General Labour", decimal rate = 2, decimal workerRate = 1) =>
        new()
        {
            CompanyProfile = companyProfile,
            CompanyProfileId = companyProfile.Id,
            JobPosition = jobPosition,
            Rate = rate,
            WorkerRate = workerRate
        };

    public static WorkerProfile FakeWorkerProfile(Covenant.Common.Entities.Agency.Agency agency = null,
        string email = null, City city = null, Guid id = default)
    {
        agency ??= FakeAgency(city: city);
        var worker = new WorkerProfile(FakeUser(email), agency.Id)
        {
            Agency = agency,
            ApprovedToWork = true,
            Location = FakeLocation(city)
        };
        if (id != default) worker.Id = id;
        return worker;
    }

    public static Request FakeRequest(Guid agencyId = default,
        Guid companyProfileId = default, Guid jobPositionRateId = default,
        Location location = default, DateTime startAt = default,
        int workersQuantity = 1,
        DurationTerm durationTerm = DurationTerm.LongTerm)
    {
        bool ownsAgency = agencyId == default;
        agencyId = ownsAgency ? Guid.NewGuid() : agencyId;
        bool ownsCompanyProfile = companyProfileId == default;
        CompanyProfile companyProfile = null;
        CompanyProfileJobPositionRate rate = null;
        if (ownsCompanyProfile)
        {
            companyProfile = ownsAgency
                ? FakeCompanyProfile(FakeAgency(agencyId))
                : FakeCompanyProfileForAgency(agencyId);
            companyProfileId = companyProfile.Id;
            if (jobPositionRateId == default)
            {
                rate = FakeJobPositionRate(companyProfile);
                jobPositionRateId = rate.Id;
            }
        }
        jobPositionRateId = jobPositionRateId == default ? Guid.NewGuid() : jobPositionRateId;
        location ??= FakeLocation();
        startAt = startAt == default ? new DateTime(2019, 01, 01) : startAt;
        var request = Request.AgencyCreateRequest(companyProfileId, location, startAt, jobPositionRateId, workersQuantity: workersQuantity,
            durationTerm: durationTerm).Value;
        if (companyProfile is not null) request.CompanyProfile = companyProfile;
        if (rate is not null) request.JobPositionRate = rate;
        return request;
    }
}
