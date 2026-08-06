using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Resources;
using Covenant.Core.BL.Extensions;
using Covenant.Core.BL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Covenant.Core.BL.Services;

public class UploadedFilesService(
    IHttpContextAccessor httpContextAccessor,
    IFilesContainer filesContainer,
    IOptions<FilesConfiguration> options) : IUploadedFilesService
{
    private readonly FilesConfiguration filesConfiguration = options.Value;

    private IFormCollection Form => httpContextAccessor.HttpContext.Request.Form;

    public T GetModel<T>() => Form.DeserializeData<T>();

    public Result Validate()
    {
        foreach (var file in Form.Files)
        {
            if (file.Length > filesConfiguration.MaximumFileSize)
            {
                return Result.Fail(ApiResources.MaxSizeFileError);
            }
            var extension = Path.GetExtension(file.FileName);
            if (!filesConfiguration.DocumentFormats.Any(f => extension.Equals($".{f}", StringComparison.OrdinalIgnoreCase)))
            {
                return Result.Fail($"Invalid format {extension}");
            }
        }
        return Result.Ok();
    }

    public async Task Upload(IEnumerable<string> fileNames)
    {
        foreach (var fileName in fileNames.Where(n => !string.IsNullOrEmpty(n)))
        {
            var file = Form.Files[fileName] ?? Form.Files.FirstOrDefault(f => f.FileName == fileName);
            if (file is not null)
            {
                await filesContainer.UploadAsync(file.OpenReadStream(), fileName);
            }
        }
    }
}
