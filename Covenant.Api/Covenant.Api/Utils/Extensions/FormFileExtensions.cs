using Covenant.Common.Models;
using Microsoft.AspNetCore.StaticFiles;

namespace Covenant.Api.Utils.Extensions;

public static class FormFileExtensions
{
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

    public static List<EmailAttachment> ToEmailAttachments(this IFormFileCollection files) =>
        files?.Select(f => new EmailAttachment(f.FileName, GetContentType(f), f.OpenReadStream())).ToList() ?? [];

    private static string GetContentType(IFormFile file) =>
        string.IsNullOrEmpty(file.ContentType) && ContentTypeProvider.TryGetContentType(file.FileName, out string contentType)
            ? contentType
            : file.ContentType;
}
