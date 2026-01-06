using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Covenant.IdentityServer.Tests.Utils
{
	internal class DisableAuthorizationHandler<TRequirement> : AuthorizationHandler<TRequirement> where TRequirement : IAuthorizationRequirement
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
		{
			context.Succeed(requirement);
			return Task.CompletedTask;
		}
	}

	internal static class DisableAuthorizationHandlerExtensions
	{
		public static IServiceCollection DisableAuthorization(this IServiceCollection services)
		{
			services.AddTransient<IAuthorizationHandler, DisableAuthorizationHandler<IAuthorizationRequirement>>();
			return services;
		}
	}
}