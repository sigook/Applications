using Covenant.Common.Entities.Accounting.Subcontractor;

namespace Covenant.Subcontractor.Services;

public interface ISubContractorPublicHolidays
{
    Task<IReadOnlyCollection<ReportSubcontractorPublicHoliday>> GetSubcontractorWorkerHolidays(IEnumerable<DateTime> holidaysInWeek, Guid workerProfileId);
}