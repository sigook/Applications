using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;

namespace Covenant.Core.BL.Interfaces;

public interface IPayStubService
{
    Task<Result<ResultGenerateDocument<byte[]>>> GenerateT4(DateTime from, DateTime to);
    Task<Result<ResultGenerateDocument<byte[]>>> GenerateCraPayroll(DateTime from, DateTime to);
    Task<Result> Generate(IEnumerable<Guid> agencyIds, IEnumerable<Guid> workerIds);
    Task<Result<PayStub>> CreateManualPayStub(CreatePayStubModel model);
}
