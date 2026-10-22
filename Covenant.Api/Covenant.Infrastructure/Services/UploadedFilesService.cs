using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Covenant.Infrastructure.Services;

public class UploadedFilesService(
    IHttpContextAccessor httpContextAccessor,
    IFilesContainer filesContainer,
    IOptions<FilesConfiguration> options) : IUploadedFilesService
{
    private const string DataKey = "data";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly FilesConfiguration filesConfiguration = options.Value;

    private IFormCollection Form => httpContextAccessor.HttpContext.Request.Form;

    public T GetModel<T>() => JsonSerializer.Deserialize<T>(Form[DataKey], JsonOptions);

    public IReadOnlyList<string> GetFileNames() => Form.Files.Select(f => f.FileName).ToList();

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
