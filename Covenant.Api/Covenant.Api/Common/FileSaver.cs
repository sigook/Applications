using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Microsoft.AspNetCore.StaticFiles;

namespace Covenant.Api.Common;

public class FileSaver
{
    private readonly FilesConfiguration filesConfiguration;

    public FileSaver(string filesUrl)
    {
        filesConfiguration = new FilesConfiguration
        {
            FilesUrl = filesUrl,
            FilesPath = filesUrl,
        };
    }

    public FileSaver(FilesConfiguration filesConfiguration) => this.filesConfiguration = filesConfiguration;

    public async Task<Result<FilesResult>> SaveImageProfile(string sourcePath, Action<string, string> upload)
    {
        if (!Directory.Exists(filesConfiguration.FilesUrl)) Directory.CreateDirectory(filesConfiguration.FilesUrl);
        string fileName = $"image_profile_{Path.GetFileName(sourcePath)}";
        string path = Path.Combine(filesConfiguration.FilesUrl, fileName);
        using (FileStream file = File.OpenRead(sourcePath))
        {
            using (var stream = new FileStream(path, FileMode.Create)) await file.CopyToAsync(stream);
        }
        new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);
        upload.Invoke(path, contentType);
        return Result.Ok(new FilesResult { Path = fileName });
    }
}
