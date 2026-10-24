using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Interfaces.Shared;

namespace Covenant.Core.BL.Services.Shared;

public class CatalogService(ICatalogRepository catalogRepository) : ICatalogService
{
    public async Task<Result<Industry>> AddIndustry(string value)
    {
        var industry = Industry.Create(value);
        if (!industry) return industry;
        await catalogRepository.Create(industry.Value);
        await catalogRepository.SaveChangesAsync();
        return industry;
    }
}
