using Covenant.Common.Repositories;
using Covenant.Infrastructure.Contexts;

namespace Covenant.Infrastructure.Repositories;

public class ShiftRepository(CovenantContext context) : BaseRepository<CovenantContext>(context), IShiftRepository
{
}
