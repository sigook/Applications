using Covenant.Common.Entities.Accounting.Subcontractor;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Repositories.Accounting;
using Covenant.Subcontractor.Utils;

namespace Covenant.Subcontractor.Services;

public class SubContractorPublicHolidays : ISubContractorPublicHolidays
{
    private readonly ISubcontractorRepository _subcontractorRepository;

    public SubContractorPublicHolidays(ISubcontractorRepository subcontractorRepository) => _subcontractorRepository = subcontractorRepository;

    public async Task<IReadOnlyCollection<ReportSubcontractorPublicHoliday>> GetSubcontractorWorkerHolidays(IEnumerable<DateTime> holidaysInWeek, Guid workerProfileId)
    {
        return await holidaysInWeek.ForEachHoliday(async holiday =>
        {
            var regularWages = await _subcontractorRepository.GetSubcontractorRegularWages(new ParamsToGetRegularWages(workerProfileId, holiday));
            return regularWages is null ? null : new ReportSubcontractorPublicHoliday(holiday, regularWages.AmountToPay) { Description = regularWages.Description };
        });
    }
}