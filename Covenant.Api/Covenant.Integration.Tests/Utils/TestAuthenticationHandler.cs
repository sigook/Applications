using Covenant.Common.Constants;
using Covenant.Integration.Tests.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Covenant.Integration.Tests.Utils;

public class TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<TestAuthenticationOptions>(options, logger, encoder)
{
    public const string RoleHeader = "X-Test-Role";
    public const string SubHeader = "X-Test-Sub";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(Options.Identity.Claims, Options.Identity.AuthenticationType);
        foreach (string role in Request.Headers[RoleHeader].SelectMany(h => h.Split(',', StringSplitOptions.RemoveEmptyEntries)))
            identity.AddClaim(new Claim(ClaimTypes.Role, role.Trim()));
        foreach (string sub in Request.Headers[SubHeader])
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub));
        AddAgencyClaims(identity);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), new AuthenticationProperties(), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static void AddAgencyClaims(ClaimsIdentity identity)
    {
        var userIds = identity.FindAll(ClaimTypes.NameIdentifier)
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty);
        var existing = identity.FindAll(CovenantConstants.AgencyId).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var member in TestAgencyMembers.OfUsers(userIds).OrderBy(m => m.AgencyId))
        {
            if (existing.Add(member.AgencyId.ToString())) identity.AddClaim(new Claim(CovenantConstants.AgencyId, member.AgencyId.ToString()));
        }
    }
}