using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;

namespace Covenant.Core.BL.Interfaces;

public interface ICompanyInteractionService
{
    Task<PaginatedList<CompanyInteractionListModel>> GetInteractions(GetCompanyInteractionsFilter filter);
    Task<Result<Guid>> Create(CreateCompanyInteractionModel model);
    Task<Result> Update(Guid id, UpdateCompanyInteractionModel model);
    Task<Result> Delete(Guid id);
}
