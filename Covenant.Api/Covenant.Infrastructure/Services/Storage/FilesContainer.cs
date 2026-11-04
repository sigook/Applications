using Covenant.Common.Configuration;
using Covenant.Common.Interfaces.Storage;

namespace Covenant.Infrastructure.Services.Storage;

	public class FilesContainer(AzureStorageConfiguration configuration) : BaseAzureStorage(configuration, ContainerName), IFilesContainer
	{
		public const string ContainerName = "files";
}