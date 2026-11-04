namespace Sigook.Mcp.Api;

public sealed record ApiResult<T>(bool Success, T? Value, int StatusCode, string? Error)
{
    public static ApiResult<T> Ok(T value, int statusCode) => new(true, value, statusCode, null);
    public static ApiResult<T> Fail(int statusCode, string error) => new(false, default, statusCode, error);
}
