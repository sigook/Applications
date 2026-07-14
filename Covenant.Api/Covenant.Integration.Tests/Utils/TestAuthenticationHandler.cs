using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Covenant.Integration.Tests.Utils;

public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
{
    public const string RoleHeader = "X-Test-Role";
    public const string SubHeader = "X-Test-Sub";

    public TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(Options.Identity.Claims, Options.Identity.AuthenticationType);
        foreach (string role in Request.Headers[RoleHeader].SelectMany(h => h.Split(',', StringSplitOptions.RemoveEmptyEntries)))
            identity.AddClaim(new Claim(ClaimTypes.Role, role.Trim()));
        foreach (string sub in Request.Headers[SubHeader])
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), new AuthenticationProperties(), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}