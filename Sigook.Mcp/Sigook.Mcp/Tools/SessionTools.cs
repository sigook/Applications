using System.ComponentModel;
using ModelContextProtocol.Server;
using Sigook.Mcp.Api;
using Sigook.Mcp.Auth;

namespace Sigook.Mcp.Tools;

[McpServerToolType]
public static class SessionTools
{
    [McpServerTool(Name = "sigook_login"), Description("Signs in to Sigook with the Microsoft 365 account of the person running this harness. Opens the browser once; the session is cached for 30 days. Call this when any other tool reports 'Not signed in'.")]
    public static async Task<object> Login(SigookSession session, ISigookIdentityApi identity, CancellationToken cancellationToken)
    {
        await session.LoginAsync(cancellationToken);
        var user = await identity.GetUserInfo(cancellationToken).OrThrow();
        return new { environment = session.Environment.Name, apiUrl = session.Environment.ApiUrl.ToString(), user };
    }

    [McpServerTool(Name = "sigook_logout"), Description("Signs out of Sigook and forgets the cached session for the current environment.")]
    public static async Task<object> Logout(SigookSession session, CancellationToken cancellationToken)
    {
        await session.LogoutAsync(cancellationToken);
        return new { environment = session.Environment.Name, signedIn = false };
    }

    [McpServerTool(Name = "sigook_whoami"), Description("Shows the target Sigook environment (local, staging or production) and who is signed in. Use it before writing anything to confirm you are pointing at the right environment.")]
    public static async Task<object> WhoAmI(SigookSession session, ISigookIdentityApi identity, CancellationToken cancellationToken)
    {
        if (!session.IsSignedIn)
            return new { environment = session.Environment.Name, apiUrl = session.Environment.ApiUrl.ToString(), signedIn = false };
        var user = await identity.GetUserInfo(cancellationToken).OrThrow();
        return new { environment = session.Environment.Name, apiUrl = session.Environment.ApiUrl.ToString(), signedIn = true, user };
    }
}
