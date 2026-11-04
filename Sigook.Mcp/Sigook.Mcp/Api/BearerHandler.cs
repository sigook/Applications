using System.Net.Http.Headers;
using Sigook.Mcp.Auth;

namespace Sigook.Mcp.Api;

public sealed class BearerHandler(SigookSession session) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await session.GetAccessTokenAsync(cancellationToken));
        return await base.SendAsync(request, cancellationToken);
    }
}
