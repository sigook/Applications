using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Covenant.Integration.Tests.Utils
{
	public class TestAuthenticationOptions : AuthenticationSchemeOptions
	{
		public static string Sub { get; set; }
		public static string Rol { get; set; }

		public virtual ClaimsIdentity Identity { get; } = new ClaimsIdentity(new[]
		{
			new Claim("http://schemas.microsoft.com/identity/claims/tenantid", "test"),
			new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.NewGuid().ToString()),
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "test"),
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "test"),
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "test"),
		}, "test");
	}
}