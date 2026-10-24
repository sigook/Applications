using System.Net;

namespace Sigook.Functions.Tests.Fakes;

public record RecordedRequest(string Client, HttpMethod Method, string Url, string? Authorization, string Body);

public sealed class FakeHttpClientFactory : IHttpClientFactory
{
    public List<RecordedRequest> Requests { get; } = [];

    public Func<string, HttpRequestMessage, HttpResponseMessage> Respond { get; set; } = (_, _) => new HttpResponseMessage(HttpStatusCode.OK);

    public IEnumerable<RecordedRequest> To(string client) => Requests.Where(r => r.Client == client);

    public HttpClient CreateClient(string name) => new(new Handler(this, name));

    private sealed class Handler(FakeHttpClientFactory factory, string client) : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string body = request.Content is null ? string.Empty : await request.Content.ReadAsStringAsync(cancellationToken);
            factory.Requests.Add(new RecordedRequest(client, request.Method, request.RequestUri!.ToString(), request.Headers.Authorization?.ToString(), body));
            return factory.Respond(client, request);
        }
    }
}
