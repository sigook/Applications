using Covenant.Common.Entities.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;

namespace Covenant.Common.Interfaces.Adapters;

public interface ICompanyAdapter
{
    Task<BulkCompany> ConvertCompanyCsvToCompanyBulk(CompanyCsvModel model, Guid agencyId, BaseModel<Guid> industry, CityModel city);
}
