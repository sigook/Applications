using Covenant.Common.Models.Accounting.PayStub;

namespace Covenant.Reports.Repositories
{
    public interface ISummaryReportRepository
    {
        Task SaveChangesAsync();
        Task Create<T>(T entity) where T : class;
        Task<List<CompanyRegularChargesByWorker>> GetCompanyRegularCharges(ParamsToGetRegularWages parameters);
    }
}