using Covenant.Common.Configuration;
using Covenant.Common.Interfaces.Storage;

namespace Covenant.Infrastructure.Services.Storage;

public class CraTablesContainer(AzureStorageConfiguration configuration)
    : BaseAzureStorage(configuration, ContainerName), ICraTablesContainer
{
    public const string ContainerName = "cra-tables";
}
