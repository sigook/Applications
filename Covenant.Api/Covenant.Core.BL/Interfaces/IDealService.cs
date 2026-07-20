using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;

namespace Covenant.Core.BL.Interfaces;

public interface IDealService
{
    Task<PaginatedList<DealListModel>> GetDeals(GetDealsFilter filter);
    Task<Result<Guid>> Create(CreateDealModel model);
    Task<Result> Update(Guid id, UpdateDealModel model);
    Task<Result> Delete(Guid id);
}
