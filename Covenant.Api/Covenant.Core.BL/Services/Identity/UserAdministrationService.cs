using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Identity;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories.Identity;
using Microsoft.AspNetCore.Identity;

namespace Covenant.Core.BL.Services.Identity;

public class UserAdministrationService(
    UserManager<CovenantUser> userManager,
    RoleManager<CovenantRole> roleManager,
    IIdentityRepository identityRepository,
    IAccountNotificationService notifications) : IUserAdministrationService
{
    public async Task<Result<Guid>> CreateUser(CreateUserModel model)
    {
        if (!await roleManager.RoleExistsAsync(model.Role))
        {
            return Result.Fail<Guid>(ResultError.Create(nameof(model.Role), $"Unknown role '{model.Role}'"));
        }

        var existing = await userManager.FindByEmailAsync(model.Email);
        if (existing is not null) return Result.Fail<Guid>(AccountMessages.UserAlreadyExists);

        var user = new CovenantUser
        {
            Id = Guid.NewGuid(),
            Email = model.Email,
            UserName = model.Email,
            EmailConfirmed = model.UserType switch
            {
                UserType.Agency or UserType.AgencyPersonnel => true,
                UserType.Company => string.IsNullOrEmpty(model.ConfirmPassword),
                _ => false
            }
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return Fail<Guid>(result);

        switch (model.UserType)
        {
            case UserType.Agency:
                result = await userManager.AddClaimAsync(user, IdentityClaims.AgencyId(model.AgencyId.Value));
                break;
            case UserType.AgencyPersonnel:
                result = await userManager.AddClaimAsync(user, IdentityClaims.AgencyId(model.AgencyId.Value));
                await notifications.SendConfirmAndSetPassword(user);
                break;
            case UserType.CompanyUser:
                result = await userManager.AddClaimAsync(user, IdentityClaims.CompanyId(model.CompanyId.Value));
                await notifications.SendConfirmAndSetPassword(user);
                break;
            case UserType.Company:
                if (!user.EmailConfirmed) await notifications.SendConfirmAccount(user, AccountMessages.ConfirmAccountWorker);
                break;
            case UserType.Worker:
                if (!string.IsNullOrEmpty(model.ConfirmPassword))
                    await notifications.SendConfirmAccount(user, AccountMessages.ConfirmAccountWorker);
                else
                    await notifications.SendConfirmAndSetPassword(user);
                break;
        }
        if (!result.Succeeded) return Fail<Guid>(result);

        result = await userManager.AddToRolesAsync(user, [model.Role]);
        if (!result.Succeeded) return Fail<Guid>(result);

        return Result.Ok(user.Id);
    }

    public async Task<Result> AddAgencyClaim(Guid userId, Guid agencyId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result.Fail(AccountMessages.UserNotFound);

        var claims = await userManager.GetClaimsAsync(user);
        if (claims.Any(c => c.Value.Equals(agencyId.ToString(), StringComparison.InvariantCultureIgnoreCase)))
        {
            return Result.Fail(ResultError.Create(nameof(agencyId), "The user already exists in this agency"));
        }

        return ToResult(await userManager.AddClaimAsync(user, IdentityClaims.AgencyId(agencyId)));
    }

    public async Task<Result<bool>> RemoveClaimOrDeleteUser(Guid userId, Guid claimValue)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result.Fail<bool>(AccountMessages.UserNotFound);

        var claims = await userManager.GetClaimsAsync(user);
        if (claims.Count > 1)
        {
            var claim = claims.FirstOrDefault(c => c.Value == claimValue.ToString());
            if (claim is null) return Result.Fail<bool>("Claim not found");
            var removed = await userManager.RemoveClaimAsync(user, claim);
            return removed.Succeeded ? Result.Ok(false) : Fail<bool>(removed);
        }

        var deleted = await userManager.DeleteAsync(user);
        return deleted.Succeeded ? Result.Ok(true) : Fail<bool>(deleted);
    }

    public async Task<Result> Deactivate(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result.Fail(AccountMessages.UserNotFound);

        await identityRepository.AddInactiveUser(user.Id);
        await identityRepository.SaveChangesAsync();
        await identityRepository.RevokeTokens(user.Id);
        return Result.Ok();
    }

    public async Task<Result> UpdateEmail(UpdateEmailModel model)
    {
        var user = await userManager.FindByIdAsync(model.Id.ToString());
        if (user is null) return Result.Fail(AccountMessages.UserNotFound);

        user.Email = model.NewEmail;
        user.UserName = model.NewEmail;
        return ToResult(await userManager.UpdateAsync(user));
    }

    public async Task<Result> UpdateRole(UpdateRoleModel model)
    {
        if (!await roleManager.RoleExistsAsync(model.Role))
        {
            return Result.Fail(ResultError.Create(nameof(model.Role), $"Unknown role '{model.Role}'"));
        }

        var user = await userManager.FindByIdAsync(model.Id.ToString());
        if (user is null) return Result.Fail(AccountMessages.UserNotFound);

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Count == 1 && currentRoles[0].Equals(model.Role, StringComparison.OrdinalIgnoreCase)) return Result.Ok();

        if (currentRoles.Count > 0)
        {
            var removed = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removed.Succeeded) return ToResult(removed);
        }

        return ToResult(await userManager.AddToRoleAsync(user, model.Role));
    }

    public Task<IReadOnlyList<UserRoleModel>> GetUsersRoles(IEnumerable<Guid> userIds) =>
        identityRepository.GetUsersRoles(userIds.Distinct().ToList());

    public string HashPassword(string password) =>
        new PasswordHasher<CovenantUser>().HashPassword(new CovenantUser(), password);

    private static Result ToResult(IdentityResult result) =>
        result.Succeeded ? Result.Ok() : Result.Fail(Errors(result));

    private static Result<T> Fail<T>(IdentityResult result) => Result.Fail<T>(Errors(result));

    private static IEnumerable<ResultError> Errors(IdentityResult result) =>
        result.Errors.Select(e => ResultError.Create(e.Code, e.Description));
}
