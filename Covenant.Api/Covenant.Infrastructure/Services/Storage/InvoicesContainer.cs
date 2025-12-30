using Covenant.Common.Configuration;
using Covenant.Common.Interfaces.Storage;

namespace Covenant.Infrastructure.Services.Storage
{
    public class InvoicesContainer : BaseAzureStorage, IInvoicesContainer
    {
        public const string ContainerName = "invoices";
        public InvoicesContainer(AzureStorageConfiguration configuration)
            : base(configuration, ContainerName)
        {
        }
    }
}