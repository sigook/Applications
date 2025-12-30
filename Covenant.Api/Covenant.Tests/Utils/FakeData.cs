using Covenant.Common.Entities;
using Covenant.Common.Enums;
using System.Text;

namespace Covenant.Tests.Utils
{
    public static class FakeData
    {
        public static string GenerateString(int length)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                builder.Append("A");
            }
            return builder.ToString();
        }
        public static string GenerateStringNumber(int length)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                builder.Append(i);
            }
            return builder.ToString();
        }

        public static Location FakeLocation() => new Location { City = new City { Province = new Province { Country = new Country() } } };

        public static Covenant.Common.Entities.Request.Request FakeRequest(Guid agencyId = default,
            Guid companyId = default, Guid jobPositionRateId = default,
            Location location = default, DateTime startAt = default,
            DateTime? finishAt = null, int workersQuantity = 1,
            DurationTerm durationTerm = DurationTerm.LongTerm)
        {
            agencyId = agencyId == default ? Guid.NewGuid() : agencyId;
            companyId = companyId == default ? Guid.NewGuid() : companyId;
            jobPositionRateId = jobPositionRateId == default ? Guid.NewGuid() : jobPositionRateId;
            location = location ?? FakeLocation();
            startAt = startAt == default ? new DateTime(2019, 01, 01) : startAt;
            return Covenant.Common.Entities.Request.Request.AgencyCreateRequest(agencyId, companyId, location, startAt, jobPositionRateId, finishAt,
                workersQuantity: workersQuantity, durationTerm: durationTerm).Value;
        }
    }
}