using Covenant.Common.Entities;
using Covenant.Common.Repositories;
using Covenant.Infrastructure.Context;

namespace Covenant.Infrastructure.Repositories
{
    public class ShiftRepository : BaseRepository<Shift>, IShiftRepository
    {
        public ShiftRepository(CovenantContext context)
            : base(context)
        {
        }
    }
}
