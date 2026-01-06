using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;

namespace Covenant.IdentityServer.Services
{
	public class CustomProfileService<TUser> : ProfileService<TUser> where TUser : class
	{
		private readonly UserManager<TUser> _userManager;
		private readonly IUserClaimsPrincipalFactory<TUser> _claimsFactory;

		public CustomProfileService(UserManager<TUser> userManager, IUserClaimsPrincipalFactory<TUser> claimsFactory) 
			: base(userManager, claimsFactory)
		{
			_userManager = userManager;
			_claimsFactory = claimsFactory;
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

			context.AddRequestedClaims(AddNickName(context,principal));
		}

		private static IEnumerable<Claim> AddNickName(ProfileDataRequestContext context, ClaimsPrincipal principal)
		{
			Claim nickName = context.Subject.FindFirst(JwtClaimTypes.NickName);
			if (nickName is null)
			{
				string preferredUsername = principal.FindFirst("preferred_username")?.Value;
				if (string.IsNullOrEmpty(preferredUsername)) return principal.Claims;
				
				var claims = new List<Claim>{new Claim(JwtClaimTypes.NickName,preferredUsername)};
				claims.AddRange(principal.Claims);
				return claims;
			}
			else
			{
				var claims = new List<Claim>{nickName};
				claims.AddRange(principal.Claims.Where(c => c.Type != JwtClaimTypes.NickName));
				return claims;
			}
		}
	}
}