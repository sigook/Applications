using Covenant.Common.Models.Accounting.PayStub;

namespace Covenant.Common.Interfaces;

public interface IBulkPayStubEmailQueue
{
    ValueTask Enqueue(BulkPayStubEmailJob job);
    ValueTask<BulkPayStubEmailJob> Dequeue(CancellationToken cancellationToken);
}
