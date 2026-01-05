using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Functionals;

namespace Covenant.PayStubs.Services;

public interface IPayStubPublicHolidays
{
    Task<Result<IReadOnlyCollection<PayStubPublicHoliday>>> GetWorkerPublicHolidays(IEnumerable<DateTime> holidaysInWeek, Guid workerProfileId);
}