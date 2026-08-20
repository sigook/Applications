using System.Security.Claims;
using Covenant.IdentityServer.Data;
using Covenant.IdentityServer.Entities;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Covenant.IdentityServer.Security;

public class CovenantResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    public const string InvalidCredentials = "invalid_credentials";
    public const string InactiveUser = "inactive_user";
    public const string EmailNotConfirmed = "email_not_confirmed";
    public const string LockedOut = "locked_out";

    private readonly UserManager<CovenantUser> _userManager;
    private readonly SignInManager<CovenantUser> _signInManager;
    private readonly CovenantContext _context;
    private readonly ILogger<CovenantResourceOwnerPasswordValidator> _logger;

    public CovenantResourceOwnerPasswordValidator(
        UserManager<CovenantUser> userManager,
        SignInManager<CovenantUser> signInManager,
        CovenantContext context,
        ILogger<CovenantResourceOwnerPasswordValidator> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _logger = logger;
    }

    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        CovenantUser user = await _userManager.FindByEmailAsync(context.UserName);
        if (user is null)
        {
            _logger.LogWarning("Native login failed: user not found for {Email}", context.UserName);
            context.Result = Invalid(InvalidCredentials);
            return;
        }

        bool isInactive = await _context.InactiveUsers.AnyAsync(iu => iu.UserId == user.Id);
        if (isInactive)
        {
            _logger.LogWarning("Native login failed: user is inactive. UserId={UserId}", user.Id);
            context.Result = Invalid(InactiveUser);
            return;
        }

        SignInResult passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, context.Password, true);
        if (passwordCheck.IsLockedOut)
        {
            _logger.LogWarning("Native login failed: user is locked out. UserId={UserId}", user.Id);
            context.Result = Invalid(LockedOut);
            return;
        }

        if (!passwordCheck.Succeeded)
        {
            _logger.LogWarning("Native login failed: invalid password for {Email}", context.UserName);
            context.Result = Invalid(InvalidCredentials);
            return;
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            _logger.LogWarning("Native login failed: email not confirmed for {Email}", context.UserName);
            context.Result = Invalid(EmailNotConfirmed);
            return;
        }

        var claims = new List<Claim>();
        IList<string> roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
        claims.AddRange(await _userManager.GetClaimsAsync(user));

        _logger.LogInformation("Native login succeeded for UserId={UserId}", user.Id);
        context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.Password, claims);
    }

    private static GrantValidationResult Invalid(string error) =>
        new(TokenRequestErrors.InvalidGrant, error);
}
