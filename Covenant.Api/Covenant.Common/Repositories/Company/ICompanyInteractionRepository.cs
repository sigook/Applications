using Covenant.Common.Entities.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Company;

public interface ICompanyInteractionRepository
{
    Task<PaginatedList<CompanyInteractionListModel>> GetInteractions(Guid agencyId, GetCompanyInteractionsFilter filter);
    Task<CompanyInteraction> GetInteraction(Expression<Func<CompanyInteraction, bool>> expression);
    Task<bool> CompanyProfileBelongsToAgency(Guid companyProfileId, Guid agencyId);
    Task Create(CompanyInteraction interaction);
    Task Delete(CompanyInteraction interaction);
    Task Update(CompanyInteraction interaction);
    Task SaveChangesAsync();
}
