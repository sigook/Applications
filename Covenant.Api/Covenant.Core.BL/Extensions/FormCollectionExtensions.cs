using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Covenant.Core.BL.Extensions;

public static class FormCollectionExtensions
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static T DeserializeData<T>(this IFormCollection form, string key = "data") =>
        JsonSerializer.Deserialize<T>(form[key], Options);
}
