using Covenant.Common.Functionals;

namespace Covenant.Common.Interfaces.Storage;

public interface IBaseAzureStorage
{
    Task<Result> UploadAsync(Stream file, string fileName);
    Task Upload(string sourcePath, string contentType, Dictionary<string, string> metadata = default);
    Task<string> Download(string fileName);
    Task<string> DownloadContent(string fileName);
    Task<bool> FileExist(string fileName);
    Task DeleteFileIfExists(string fileName);
    Task DeleteFilesIfExists(IEnumerable<string> fileNames);
}