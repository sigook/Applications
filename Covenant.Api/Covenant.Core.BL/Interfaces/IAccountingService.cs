using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Accounting.Subcontractor;

namespace Covenant.Core.BL.Interfaces;

public interface IAccountingService
{
    Task<PaginatedList<WeeklyPayrollModel>> GetWeeklyPayrollGroupByPaymentDate(Pagination pagination);
    Task<Result<ResultGenerateDocument<byte[]>>> GetWeeklyPayrollGroupByPaymentDateFile(string weekEnding);
    Task<PaginatedList<PayrollSubContractorListModel>> GetSubcontractors(Pagination filter);
    Task<Result<ResultGenerateDocument<byte[]>>> GetSubcontractorFile(string weekEnding);
}
