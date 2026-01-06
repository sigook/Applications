using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.PayStub;

namespace Covenant.Common.Repositories.Accounting;

public interface IPayStubRepository
{
    Task Create<T>(T entity) where T : class;
    Task<List<NextNumberModel>> GetNextPayStubNumbers(int limit);
    Task<bool> IsPayStubNumberTaken(long payStubNumber);
    Task<PaginatedList<PayStubListModel>> GetPayStubs(IEnumerable<Guid> agencyId, GetPayStubsFilter filter);
    Task<List<PayStubListModel>> GetAllPayStubs(IEnumerable<Guid> agencyIds, GetPayStubsFilter filter);
    Task<PayStubDetailModel> GetPayStubDetail(Guid payStubId);
    Task<RegularWageWorker> GetWorkerRegularWages(ParamsToGetRegularWages parameters);
    Task<List<PayStubDeleteWarningListModel>> GetPayStubs(Guid invoiceId);
    Task<PaginatedList<WeeklyPayrollModel>> GetWeeklyPayrollGroupByPaymentDate(IEnumerable<Guid> agencyIds, Pagination pagination);
    Task<List<WeeklyPayStubModel>> GetWeeklyPayrollDetailByPaymentDate(DateTime paymentDate);
    Task<IReadOnlyList<string>> Delete(IEnumerable<Guid> payStubsId);
    Task<IEnumerable<PayStubT4Model>> GetPayStubsByDates(DateTime startDate, DateTime endDate);
    Task SaveChangesAsync();
}