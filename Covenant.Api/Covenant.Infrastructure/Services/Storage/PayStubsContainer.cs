using Covenant.Common.Configuration;
using Covenant.Common.Interfaces.Storage;

namespace Covenant.Infrastructure.Services.Storage
{
    public class PayStubsContainer : BaseAzureStorage, IPayStubsContainer
    {
        public const string ContainerName = "paystubs";

        public PayStubsContainer(AzureStorageConfiguration configuration)
            : base(configuration, ContainerName)
        {
        }
    }
}