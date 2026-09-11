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

public class CustomProfileService<TUser> : ProfileService<TUser> where TUser : class
{
	private readonly UserManager<TUser> _userManager;
	private readonly IUserClaimsPrincipalFactory<TUser> _claimsFactory;
	private readonly CovenantContext _covenantContext;
	private readonly IMicrosoft365AccountService _accountService;
	private readonly ILogger<CustomProfileService<TUser>> _logger;

	public CustomProfileService(
		UserManager<TUser> userManager,
		IUserClaimsPrincipalFactory<TUser> claimsFactory,
		CovenantContext covenantContext,
		IMicrosoft365AccountService accountService,
		ILogger<CustomProfileService<TUser>> logger)
		: base(userManager, claimsFactory)
	{
		_userManager = userManager;
		_claimsFactory = claimsFactory;
		_covenantContext = covenantContext;
		_accountService = accountService;
		_logger = logger;
	}

	/// <summary>
	/// The roles and nickname are required in the api
	/// </summary>
	/// <param name="context"></param>
	public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
	{
		string sub = context.Subject.GetSubjectId();
		TUser user = await _userManager.FindByIdAsync(sub);
		ClaimsPrincipal principal = await _claimsFactory.CreateAsync(user);
		IEnumerable<Claim> roles = context.Subject.FindAll(JwtClaimTypes.Role);
		if (roles != null) context.IssuedClaims.AddRange(roles);

		context.AddRequestedClaims(AddNickName(context, principal));
	}

	protected override async Task IsActiveAsync(IsActiveContext context, TUser user)
	{
		await base.IsActiveAsync(context, user);
		if (!context.IsActive) return;

		Guid userId = Guid.Parse(await _userManager.GetUserIdAsync(user));
		if (await _covenantContext.InactiveUsers.AnyAsync(iu => iu.UserId == userId))
		{
			_logger.LogWarning("Session rejected: user is inactive. UserId={UserId} Caller={Caller}", userId, context.Caller);
			context.IsActive = false;
			return;
		}

		IList<Claim> claims = await _userManager.GetClaimsAsync(user);
		string objectId = claims.FirstOrDefault(c => c.Type == Constants.MicrosoftObjectId)?.Value;
		if (string.IsNullOrEmpty(objectId)) return;

		if (!await _accountService.IsAccountEnabledAsync(objectId))
		{
			_logger.LogWarning("Session rejected: Microsoft 365 account is disabled. UserId={UserId} Caller={Caller}", userId, context.Caller);
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
