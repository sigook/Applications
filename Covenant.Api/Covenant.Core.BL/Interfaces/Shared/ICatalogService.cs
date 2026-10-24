using Covenant.Common.Entities;
using Covenant.Common.Functionals;

namespace Covenant.Core.BL.Interfaces.Shared;

public interface ICatalogService
{
    Task<Result<Industry>> AddIndustry(string value);
}
