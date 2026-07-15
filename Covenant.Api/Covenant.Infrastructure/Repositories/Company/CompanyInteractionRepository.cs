using Covenant.Common.Repositories.Company;
using Covenant.Infrastructure.Contexts;

namespace Covenant.Infrastructure.Repositories.Company;

public class CompanyInteractionRepository : ICompanyInteractionRepository
{
    private readonly CovenantContext _context;
    public CompanyInteractionRepository(CovenantContext context)
    {
        _context = context;
    }
    
    public async Task Create<T>(T entity) where T : class => await _context.Set<T>().AddAsync(entity);
    

    public void Delete<T>(T entity) where T : class => _context.Set<T>().Remove(entity);

    public void Update<T>(T entity) where T : class => _context.Set<T>().Update(entity);
}