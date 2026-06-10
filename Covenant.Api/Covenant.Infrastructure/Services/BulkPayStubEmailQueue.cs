using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.PayStub;
using System.Threading.Channels;

namespace Covenant.Infrastructure.Services;

public class BulkPayStubEmailQueue : IBulkPayStubEmailQueue
{
    private readonly Channel<BulkPayStubEmailJob> _channel =
        Channel.CreateUnbounded<BulkPayStubEmailJob>(new UnboundedChannelOptions { SingleReader = true });

    public ValueTask Enqueue(BulkPayStubEmailJob job) => _channel.Writer.WriteAsync(job);

    public ValueTask<BulkPayStubEmailJob> Dequeue(CancellationToken cancellationToken) =>
        _channel.Reader.ReadAsync(cancellationToken);
}
