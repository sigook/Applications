using ModelContextProtocol;
using Sigook.Mcp.Configuration;

namespace Sigook.Mcp.Auth;

public sealed class SigookSession(TokenStore store, PkceLoginFlow loginFlow, SigookEnvironment environment)
{
    private static readonly TimeSpan ExpiryMargin = TimeSpan.FromMinutes(1);
    private readonly SemaphoreSlim gate = new(1, 1);

    public SigookEnvironment Environment => environment;

    public async Task<StoredTokens> LoginAsync(CancellationToken cancellationToken)
    {
        var response = await loginFlow.LoginAsync(cancellationToken);
        var tokens = ToStored(response);
        store.Save(tokens);
        return tokens;
    }

    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        var tokens = store.Load();
        if (tokens?.RefreshToken is not null) await loginFlow.RevokeAsync(tokens.RefreshToken, cancellationToken);
        store.Clear();
    }

    public bool IsSignedIn => store.Load() is not null;

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            var tokens = store.Load() ?? throw NotSignedIn();
            if (tokens.ExpiresAt - ExpiryMargin > DateTimeOffset.UtcNow) return tokens.AccessToken;
            if (string.IsNullOrEmpty(tokens.RefreshToken))
            {
                store.Clear();
                throw NotSignedIn();
            }
            try
            {
                var refreshed = ToStored(await loginFlow.RefreshAsync(tokens.RefreshToken, cancellationToken));
                store.Save(refreshed);
                return refreshed.AccessToken;
            }
            catch (McpException)
            {
                store.Clear();
                throw NotSignedIn();
            }
        }
        finally
        {
            gate.Release();
        }
    }

    private StoredTokens ToStored(TokenResponse response) =>
        new(response.AccessToken, response.RefreshToken, DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn), environment.Name);

    private McpException NotSignedIn() =>
        new($"Not signed in to Sigook ({environment.Name}). Call the sigook_login tool first.");
}
