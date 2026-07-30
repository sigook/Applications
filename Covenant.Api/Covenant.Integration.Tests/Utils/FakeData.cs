using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;

namespace Covenant.Integration.Tests.Utils
{
    public static class FakeData
    {
        public static Location FakeLocation()
        {
            var location = new Location
            {
                City = new City
                {
                    Province = new Province
                    {
                        Country = new Country
                        {
                            Code = "CA"
                        }
                    }
                },
            };
            location.UpdateCoordinates(43.6020909, -79.7335027);
            return location;
        }

        public static Request FakeRequest(Guid agencyId = default,
            Guid companyProfileId = default, Guid jobPositionRateId = default,
            Location location = default, DateTime startAt = default,
            int workersQuantity = 1,
            DurationTerm durationTerm = DurationTerm.LongTerm)
        {
            agencyId = agencyId == default ? Guid.NewGuid() : agencyId;
            bool ownsCompanyProfile = companyProfileId == default;
            companyProfileId = ownsCompanyProfile ? Guid.NewGuid() : companyProfileId;
            jobPositionRateId = jobPositionRateId == default ? Guid.NewGuid() : jobPositionRateId;
            location = location ?? FakeLocation();
            startAt = startAt == default ? new DateTime(2019, 01, 01) : startAt;
            var request = Request.AgencyCreateRequest(companyProfileId, location, startAt, jobPositionRateId, workersQuantity: workersQuantity,
                durationTerm: durationTerm).Value;
            if (ownsCompanyProfile)
                request.CompanyProfile = new CompanyProfile { Id = companyProfileId, AgencyId = agencyId };
            return request;
        }

        public static Covenant.Common.Entities.Agency.Agency FakeAgency(Guid id) =>
            new Covenant.Common.Entities.Agency.Agency { Id = id, RecruitmentEmail = "recruit@test.com" };
    }
}