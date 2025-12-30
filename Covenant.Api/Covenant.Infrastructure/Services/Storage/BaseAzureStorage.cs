using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Storage;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace Covenant.Infrastructure.Services.Storage;

public class BaseAzureStorage : IBaseAzureStorage
{
    private readonly ILogger logger;
    private readonly string _containerName;
    private readonly string _connectionString;
    private readonly BlobContainerClient container;

    protected BaseAzureStorage(AzureStorageConfiguration configuration, string containerName)
    {
        logger = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
            builder.AddConsole();
        }).CreateLogger<BaseAzureStorage>();
        _containerName = containerName ?? throw new ArgumentNullException(nameof(containerName));
        _connectionString = configuration.AccessKeys.First(key => key.ContainerName == containerName).ConnectionString
            ?? throw new ArgumentNullException(nameof(_connectionString));
        container = new BlobContainerClient(_connectionString, _containerName);
    }

    public async Task Upload(string sourcePath, string contentType, Dictionary<string, string> metadata = default)
    {
        if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath)) throw new ArgumentNullException();
        var blob = container.GetBlobClient(Path.GetFileName(sourcePath));
        if (await blob.ExistsAsync()) return;
        using var stream = new FileStream(sourcePath, FileMode.Open);
        var info = await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, metadata ?? new Dictionary<string, string>());
    }

    public async Task<string> Download(string fileName)
    {
        try
        {
            var blob = container.GetBlobClient(fileName);
            var response = await blob.DownloadAsync();
            if (response is null) return string.Empty;
            string path = Path.Combine(Path.GetTempPath(), fileName);
            using (FileStream file = File.OpenWrite(path)) await response.Value.Content.CopyToAsync(file);
            return path;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public async Task<string> DownloadContent(string fileName)
    {
        try
        {
            var blob = container.GetBlobClient(fileName);
            var response = await blob.DownloadContentAsync();
            if (response is null) return string.Empty;
            return response.Value.Content.ToString();
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public async Task<bool> FileExist(string fileName)
    {
        try
        {
            var client = container.GetBlobClient(fileName);
            return await client.ExistsAsync();
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task DeleteFileIfExists(string fileName)
    {
        try
        {
            var result = await container.DeleteBlobIfExistsAsync(fileName);
            logger.LogInformation("Was file {0} deleted? {1}", fileName, result.Value);
        }
        catch (Exception)
        {
            logger.LogInformation("File {0} was not deleted", fileName);
            return;
        }
    }

    public async Task DeleteFilesIfExists(IEnumerable<string> fileNames)
    {
        if (fileNames?.Any() == true)
        {
            foreach (var file in fileNames)
            {
                await DeleteFileIfExists(file);
            }
        }
    }

    public async Task<Result> UploadAsync(Stream file, string fileName)
    {
        var blob = container.GetBlobClient(fileName);
        if (await blob.ExistsAsync())
        {
            return Result.Fail($"{fileName} already exists");
        }
        var contentTypeProvider = new FileExtensionContentTypeProvider();
        if (!contentTypeProvider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };
        await blob.UploadAsync(file, options);
        return Result.Ok();
    }
}