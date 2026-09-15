using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Repositories.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Covenant.Core.BL.Services.Identity;

public class UserSessionValidator(
    UserManager<CovenantUser> userManager,
    IIdentityRepository identityRepository,
    IMicrosoft365AccountService accountService,
    ILogger<UserSessionValidator> logger) : IUserSessionValidator
{
    public async Task<bool> IsActive(CovenantUser user)
    {
        if (await identityRepository.IsInactive(user.Id))
        {
            logger.LogWarning("Session rejected: user is inactive. UserId={UserId}", user.Id);
            return false;
        }

        var claims = await userManager.GetClaimsAsync(user);
        var objectId = claims.FirstOrDefault(c => c.Type == IdentityClaims.MicrosoftObjectId)?.Value;
        if (string.IsNullOrEmpty(objectId)) return true;

        if (await accountService.IsAccountEnabled(objectId)) return true;

        logger.LogWarning("Session rejected: Microsoft 365 account is disabled. UserId={UserId}", user.Id);
        return false;
    }
}
