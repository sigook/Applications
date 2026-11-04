using Covenant.Common.Configuration;
using Covenant.Common.Interfaces.Storage;

namespace Covenant.Infrastructure.Services.Storage;

public class InvoicesContainer(AzureStorageConfiguration configuration) : BaseAzureStorage(configuration, ContainerName), IInvoicesContainer
{
    public const string ContainerName = "invoices";
}