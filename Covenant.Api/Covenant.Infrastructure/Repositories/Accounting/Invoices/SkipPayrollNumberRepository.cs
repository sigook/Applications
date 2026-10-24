using Covenant.Common.Entities.Accounting.Invoices;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Accounting.Invoices;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Repositories.Accounting.Invoices;

public class SkipPayrollNumberRepository(CovenantContext context) : BaseRepository<CovenantContext>(context), ISkipPayrollNumberRepository
{
    public Task<List<BaseModel<Guid>>> Get(string searchTerm)
    {
        var skipPayrollNumbers = Context.SkipPayrollNumbers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            skipPayrollNumbers = skipPayrollNumbers.Where(spn => spn.PayrollNumber.Contains(searchTerm));
        }
        var result = skipPayrollNumbers.Select(spn => new BaseModel<Guid>(spn.Id, spn.PayrollNumber));
        return result.ToListAsync();
    }

    public async Task<Result> Create(BaseModel<Guid> model)
    {
        if (await Context.PayStubs.AnyAsync(p => p.PayStubNumberId.ToString() == model.Value) ||
            await Context.SkipPayrollNumbers.AnyAsync(p => p.PayrollNumber == model.Value))
        {
            return Result.Fail("The number is in a pay stub or in the list");
        }
        await Context.SkipPayrollNumbers.AddAsync(new SkipPayrollNumber(Guid.NewGuid(), model.Value));
        await Context.SaveChangesAsync();
        return Result.Ok();
    }
}
