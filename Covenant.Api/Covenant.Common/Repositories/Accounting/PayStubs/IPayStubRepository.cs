using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models;

namespace Covenant.Common.Repositories.Accounting.PayStubs;

public interface IPayStubRepository : IBaseRepository
{
    Task<List<NextNumberModel>> GetNextPayStubNumbers(int limit);
    Task<bool> IsPayStubNumberTaken(long payStubNumber);
    Task<PaginatedList<PayStubListModel>> GetPayStubs(IEnumerable<Guid> agencyId, GetPayStubsFilter filter);
    Task<List<PayStubListModel>> GetAllPayStubs(IEnumerable<Guid> agencyIds, GetPayStubsFilter filter);
    Task<PayStubDetailModel> GetPayStubDetail(Guid payStubId);
    Task<RegularWageWorker> GetWorkerRegularWages(Guid workerProfileId, DateTime holiday, IEnumerable<DateTime> qualifyingDays);
    Task<List<PayStubDeleteWarningListModel>> GetPayStubs(Guid invoiceId);
    Task<PaginatedList<WeeklyPayrollModel>> GetWeeklyPayrollGroupByPaymentDate(IEnumerable<Guid> agencyIds, Pagination pagination);
    Task<List<WeeklyPayStubModel>> GetWeeklyPayrollDetailByPaymentDate(DateTime paymentDate);
    Task<IReadOnlyList<string>> DeletePayStubs(IEnumerable<Guid> payStubsId);
    Task<IEnumerable<PayStubT4Model>> GetPayStubsByDates(DateTime startDate, DateTime endDate);
    Task<PayStubYtdModel> GetYtdSummary(Guid workerProfileId, int year);
}