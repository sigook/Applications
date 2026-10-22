using Covenant.Common.Models;

namespace Covenant.Common.Configuration;

public class AzureStorageConfiguration(IEnumerable<AccessKey> keys)
{
    public IReadOnlyCollection<AccessKey> AccessKeys { get; } = [.. keys];
}
