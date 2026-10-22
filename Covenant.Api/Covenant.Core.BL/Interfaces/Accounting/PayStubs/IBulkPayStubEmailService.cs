using Covenant.Common.Models.Accounting.PayStub;

namespace Covenant.Core.BL.Interfaces.Accounting.PayStubs;

public interface IBulkPayStubEmailService
{
    Task Send(BulkPayStubEmailJob job, CancellationToken cancellationToken);
}
