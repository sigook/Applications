using Covenant.Common.Functionals;
using Covenant.Common.Models;

namespace Covenant.Common.Repositories.Accounting;

public interface ISkipPayrollNumberRepository
{
    Task<List<BaseModel<Guid>>> Get(string searchTerm);
    Task<Result> Create(BaseModel<Guid> model);
}