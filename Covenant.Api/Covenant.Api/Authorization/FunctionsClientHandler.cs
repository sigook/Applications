using Covenant.Common.Configuration;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Covenant.Api.Authorization;

public class FunctionsClientHandler(IOptions<IdentityConfiguration> options) : AuthorizationHandler<FunctionsClientRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FunctionsClientRequirement requirement)
    {
        var clientId = options.Value.FunctionsClientId;
        if (!string.IsNullOrEmpty(clientId) && string.Equals(context.User.GetSubject(), clientId, StringComparison.Ordinal))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
