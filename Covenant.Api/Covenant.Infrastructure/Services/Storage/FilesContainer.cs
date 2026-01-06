using Covenant.Common.Configuration;
using Covenant.Common.Interfaces.Storage;

namespace Covenant.Infrastructure.Services.Storage
{
	public class FilesContainer : BaseAzureStorage, IFilesContainer
	{
		public const string ContainerName = "files";

		public FilesContainer(AzureStorageConfiguration configuration) 
			: base(configuration, ContainerName) 
		{ 
		}
	}
}