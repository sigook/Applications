using Covenant.Common.Entities.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Company;

public interface IDealRepository
{
    Task<PaginatedList<DealListModel>> GetDeals(Guid agencyId, GetDealsFilter filter);
    Task<Deal> GetDeal(Expression<Func<Deal, bool>> expression);
    Task<bool> CompanyProfileBelongsToAgency(Guid companyProfileId, Guid agencyId);
    Task Create(Deal deal);
    Task Delete(Deal deal);
    Task Update(Deal deal);
    Task SaveChangesAsync();
}
