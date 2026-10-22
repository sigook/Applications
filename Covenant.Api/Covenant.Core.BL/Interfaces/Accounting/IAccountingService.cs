using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Accounting.Subcontractors;
using Covenant.Common.Models;

namespace Covenant.Core.BL.Interfaces.Accounting;

public interface IAccountingService
{
    Task<PaginatedList<WeeklyPayrollModel>> GetWeeklyPayrollGroupByPaymentDate(Pagination pagination);
    Task<Result<ResultGenerateDocument<byte[]>>> GetWeeklyPayrollGroupByPaymentDateFile(string weekEnding);
    Task<PaginatedList<PayrollSubContractorListModel>> GetSubcontractors(Pagination filter);
    Task<Result<ResultGenerateDocument<byte[]>>> GetSubcontractorFile(string weekEnding);
    Task<Result> DeleteSubcontractorReport(string weekEnding);
}
