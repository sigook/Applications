using Covenant.Common.Entities.Companies;
using Covenant.Common.Models.Companies.Agencies;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models.Locations;
using Covenant.Common.Models;

namespace Covenant.Common.Interfaces.Adapters;

public interface ICompanyAdapter
{
    Task<BulkCompany> ConvertCompanyCsvToCompanyBulk(CompanyCsvModel model, Guid agencyId, BaseModel<Guid> industry, CityModel city);
}
