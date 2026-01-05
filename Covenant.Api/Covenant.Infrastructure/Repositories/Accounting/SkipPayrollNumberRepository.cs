using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Repositories.Accounting;
using Covenant.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories.Accounting;

public class SkipPayrollNumberRepository : ISkipPayrollNumberRepository
{
    private readonly CovenantContext _context;

    public SkipPayrollNumberRepository(CovenantContext context) => _context = context;

    public Task<List<BaseModel<Guid>>> Get(string searchTerm)
    {
        var skipPayrollNumbers = _context.SkipPayrollNumbers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            skipPayrollNumbers = skipPayrollNumbers.Where(spn => spn.PayrollNumber.Contains(searchTerm));
        }
        var result = skipPayrollNumbers.Select(spn => new BaseModel<Guid>(spn.Id, spn.PayrollNumber));
        return result.ToListAsync();
    }

    public async Task<Result> Create(BaseModel<Guid> model)
    {
        if (await _context.PayStub.AnyAsync(p => p.PayStubNumberId.ToString() == model.Value) ||
            await _context.SkipPayrollNumbers.AnyAsync(p => p.PayrollNumber == model.Value))
        {
            return Result.Fail("The number is in a pay stub or in the list");
        }
        await _context.SkipPayrollNumbers.AddAsync(new SkipPayrollNumber(Guid.NewGuid(), model.Value));
        await _context.SaveChangesAsync();
        return Result.Ok();
    }
}