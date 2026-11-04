using System.Text.Json.Serialization;
using System.Text.Json;
using ModelContextProtocol;
using Refit;

namespace Sigook.Mcp.Api;

public static class ApiCalls
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public static async Task<T> OrThrow<T>(this Task<T> call)
    {
        try
        {
            return await call;
        }
        catch (Exception ex) when (ex.InnerException is McpException inner)
        {
            throw inner;
        }
        catch (ApiException ex)
        {
            throw new McpException($"{ex.RequestMessage.Method} {ex.Uri?.PathAndQuery} failed ({(int)ex.StatusCode}): {Truncate(ex.Content)}");
        }
        catch (HttpRequestException ex)
        {
            throw new McpException($"Sigook API unreachable: {ex.Message}");
        }
    }

    public static async Task<ApiResult<T>> AsResult<T>(this Task<ApiResponse<T>> call)
    {
        try
        {
            using var response = await call;
            if (response.Error?.InnerException is McpException inner) throw inner;
            return response.IsSuccessStatusCode
                ? ApiResult<T>.Ok(response.Content!, (int)response.StatusCode)
                : ApiResult<T>.Fail((int?)response.StatusCode ?? 0, Truncate(response.Error is ApiException error ? error.Content : response.Error?.Message));
        }
        catch (Exception ex) when (ex.InnerException is McpException inner)
        {
            throw inner;
        }
        catch (ApiException ex)
        {
            return ApiResult<T>.Fail((int)ex.StatusCode, Truncate(ex.Content));
        }
        catch (HttpRequestException ex)
        {
            return ApiResult<T>.Fail(0, $"Sigook API unreachable: {ex.Message}");
        }
    }

    public static string ToJson(object model) => JsonSerializer.Serialize(model, JsonOptions);

    public static StreamPart FilePart(string filePath, string fileName) =>
        new(File.OpenRead(filePath), fileName, ContentTypeFor(filePath));

    private static string ContentTypeFor(string filePath) => Path.GetExtension(filePath).ToLowerInvariant() switch
    {
        ".pdf" => "application/pdf",
        ".doc" => "application/msword",
        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        _ => "application/octet-stream"
    };

    private static string Truncate(string? body)
    {
        if (string.IsNullOrEmpty(body)) return string.Empty;
        return body.Length <= 2000 ? body : body[..2000] + "…";
    }
}
