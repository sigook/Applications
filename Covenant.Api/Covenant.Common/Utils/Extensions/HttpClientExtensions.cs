using System.Net.Http.Headers;
using System.Text.Json;

namespace Covenant.Common.Utils.Extensions;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
    {
        string dataAsString = JsonSerializer.Serialize(data);
        var content = new StringContent(dataAsString);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return httpClient.PostAsync(url, content);
    }

    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
    {
        string dataAsString = JsonSerializer.Serialize(data);
        var content = new StringContent(dataAsString);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return httpClient.PutAsync(url, content);
    }

    public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
    {
        string dataAsString = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(dataAsString);
    }
}
