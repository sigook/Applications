using Covenant.Common.Models.Accounting.PayStubs;

namespace Covenant.Core.BL.Interfaces.Accounting.PayStubs;

public interface IBulkPayStubEmailService
{
    Task Send(BulkPayStubEmailJob job, CancellationToken cancellationToken);
}
