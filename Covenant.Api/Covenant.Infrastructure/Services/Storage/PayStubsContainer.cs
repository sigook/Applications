using Covenant.Common.Configuration;
using Covenant.Common.Interfaces.Storage;

namespace Covenant.Infrastructure.Services.Storage;

public class PayStubsContainer(AzureStorageConfiguration configuration) : BaseAzureStorage(configuration, ContainerName), IPayStubsContainer
{
    public const string ContainerName = "paystubs";
}