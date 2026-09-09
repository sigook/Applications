using System.Security.Claims;
using Covenant.IdentityServer.Controllers.Account.Models;
using Covenant.IdentityServer.Data;
using IdentityModel;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Covenant.IdentityServer.Services;

public class CustomProfileService<TUser>(
    UserManager<TUser> userManager,
    IUserClaimsPrincipalFactory<TUser> claimsFactory,
    CovenantContext covenantContext,
    IMicrosoft365AccountService accountService,
    ILogger<CustomProfileService<TUser>> logger) : ProfileService<TUser>(userManager, claimsFactory) where TUser : class
{
	/// <summary>
	/// The roles and nickname are required in the api
	/// </summary>
	/// <param name="context"></param>
	public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
	{
		string sub = context.Subject.GetSubjectId();
		TUser user = await userManager.FindByIdAsync(sub);
		ClaimsPrincipal principal = await claimsFactory.CreateAsync(user);
		IEnumerable<Claim> roles = context.Subject.FindAll(JwtClaimTypes.Role);
		if (roles != null) context.IssuedClaims.AddRange(roles);

		context.AddRequestedClaims(AddNickName(context, principal));
	}

	protected override async Task IsActiveAsync(IsActiveContext context, TUser user)
	{
		await base.IsActiveAsync(context, user);
		if (!context.IsActive) return;

		Guid userId = Guid.Parse(await userManager.GetUserIdAsync(user));
		if (await covenantContext.InactiveUsers.AnyAsync(iu => iu.UserId == userId))
		{
			logger.LogWarning("Session rejected: user is inactive. UserId={UserId} Caller={Caller}", userId, context.Caller);
			context.IsActive = false;
			return;
		}

		IList<Claim> claims = await userManager.GetClaimsAsync(user);
		string objectId = claims.FirstOrDefault(c => c.Type == Constants.MicrosoftObjectId)?.Value;
		if (string.IsNullOrEmpty(objectId)) return;

		if (!await accountService.IsAccountEnabledAsync(objectId))
		{
			logger.LogWarning("Session rejected: Microsoft 365 account is disabled. UserId={UserId} Caller={Caller}", userId, context.Caller);
			context.IsActive = false;
		}
	}

	private static IEnumerable<Claim> AddNickName(ProfileDataRequestContext context, ClaimsPrincipal principal)
	{
		Claim nickName = context.Subject.FindFirst(JwtClaimTypes.NickName);
		if (nickName is null)
		{
			string preferredUsername = principal.FindFirst("preferred_username")?.Value;
			if (string.IsNullOrEmpty(preferredUsername)) return principal.Claims;

			var claims = new List<Claim> { new Claim(JwtClaimTypes.NickName, preferredUsername) };
			claims.AddRange(principal.Claims);
			return claims;
		}
		else
		{
			var claims = new List<Claim> { nickName };
			claims.AddRange(principal.Claims.Where(c => c.Type != JwtClaimTypes.NickName));
			return claims;
		}
	}
}
