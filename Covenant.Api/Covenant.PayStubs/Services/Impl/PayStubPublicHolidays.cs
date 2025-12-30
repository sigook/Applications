using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Repositories.Accounting;

namespace Covenant.PayStubs.Services.Impl
{
    public class PayStubPublicHolidays : IPayStubPublicHolidays
    {
        private readonly IPayStubRepository _payStubRepository;
        public PayStubPublicHolidays(IPayStubRepository payStubRepository) => _payStubRepository = payStubRepository;

        public async Task<Result<IReadOnlyCollection<PayStubPublicHoliday>>> GetWorkerPublicHolidays(IEnumerable<DateTime> holidaysInWeek, Guid workerProfileId)
        {
            var list = new List<PayStubPublicHoliday>();
            if (holidaysInWeek is null) return Result.Ok<IReadOnlyCollection<PayStubPublicHoliday>>(list);
            foreach (var holiday in holidaysInWeek)
            {
                var regularWages = await _payStubRepository.GetWorkerRegularWages(new ParamsToGetRegularWages(workerProfileId, holiday));
                if (regularWages is null) continue;
                var result = PayStubPublicHoliday.Create(holiday, regularWages.AmountToPay, regularWages.Description);
                if (!result) return Result.Fail<IReadOnlyCollection<PayStubPublicHoliday>>(result.Errors);
                list.Add(result.Value);
            }
            return Result.Ok<IReadOnlyCollection<PayStubPublicHoliday>>(list);
        }
    }
}