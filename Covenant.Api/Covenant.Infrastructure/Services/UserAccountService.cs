using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Models;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Covenant.Infrastructure.Services;

public class UserAccountService(
    IUserRepository userRepository,
    IUserAdministrationService userAdministration,
    IConfiguration configuration,
    ICurrentUserService currentUserService,
    IValidator<ChangeEmailModel> changeEmailValidator,
    ILogger<UserAccountService> logger) : IUserAccountService
{
    public const string UserNotFound = "User not found";
    public const string EmailEqualToCurrent = "Your new email is equal to your current email";

    public async Task<Result<User>> CreateUser(CreateUserModel model)
    {
        try
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                model.Password = RandomPassword();
            }
            var created = await userAdministration.CreateUser(model);
            if (!created) return Result.Fail<User>(created.Errors);

            var newUser = new User(model.Email, created.Value);
            await userRepository.Create(newUser);
            await userRepository.SaveChangesAsync();
            return Result.Ok(newUser);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user");
            return Result.Fail<User>("There was an error creating the user please try again later");
        }
    }

    public async Task<Result> UpdateAgencyUser(Guid userId, IdModel agency)
    {
        try
        {
            return await userAdministration.AddAgencyClaim(userId, agency.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding the user to the agency");
            return Result.Fail("There was an error creating the user please try again later");
        }
    }

    public async Task<Result> DeleteUserOrClaim(Guid userId, IdModel claim)
    {
        try
        {
            var user = await userRepository.GetUserById(userId);
            if (user == null)
            {
                return Result.Fail("User not exists");
            }
            var removed = await userAdministration.RemoveClaimOrDeleteUser(userId, claim.Id);
            if (!removed) return Result.Fail(removed.Errors);
            if (removed.Value)
            {
                userRepository.Delete(user);
                await userRepository.SaveChangesAsync();
            }
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting agency personnel");
            return Result.Fail("There was an error deleting the user please try again later");
        }
    }

    public async Task<Result> InactiveUser(Guid id)
    {
        try
        {
            var deactivated = await userAdministration.Deactivate(id);
            if (!deactivated) return deactivated;

            var user = await userRepository.GetUserById(id);
            user.InactiveUser();
            await userRepository.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inactivating user");
            return Result.Fail("There was an error inactiving the user please try again later");
        }
    }

    public async Task<Result> UpdateUserEmail(UpdateEmailModel model)
    {
        try
        {
            var email = CvnEmail.Create(model.NewEmail);
            if (!email) return email;
            if (await userRepository.UserExists(email.Value.Email)) return Result.Fail(ApiResources.EmailAlreadyTaken);

            var updated = await userAdministration.UpdateEmail(model);
            if (!updated) return updated;

            var user = await userRepository.GetUserById(model.Id);
            user.UpdateEmail(model.NewEmail);
            await userRepository.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user's email");
            return Result.Fail("There was an error updating the email please try again later");
        }
    }

    public async Task<Result> ChangeEmail(ChangeEmailModel model)
    {
        var validation = await changeEmailValidator.ValidateAsync(model);
        if (!validation.IsValid) return validation.ToResultFailure();
        var email = CvnEmail.Create(model.NewEmail);
        if (!email) return email;
        var user = await userRepository.GetUserById(currentUserService.GetUserId());
        if (user is null) return Result.Fail(UserNotFound);
        if (user.Email == email.Value.Email) return Result.Fail(EmailEqualToCurrent);
        return await UpdateUserEmail(new UpdateEmailModel(user.Id) { NewEmail = email.Value.Email });
    }

    public async Task<Result> UpdateUserRole(UpdateRoleModel model)
    {
        try
        {
            return await userAdministration.UpdateRole(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user's role");
            return Result.Fail("There was an error updating the role please try again later");
        }
    }

    public async Task<Result<IEnumerable<UserRoleModel>>> GetUsersRoles(IEnumerable<Guid> userIds)
    {
        var ids = userIds?.Distinct().ToList() ?? [];
        if (ids.Count == 0) return Result.Ok(Enumerable.Empty<UserRoleModel>());
        try
        {
            var roles = await userAdministration.GetUsersRoles(ids);
            return Result.Ok(roles.AsEnumerable());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting users roles");
            return Result.Fail<IEnumerable<UserRoleModel>>("There was an error getting the roles please try again later");
        }
    }

    private string RandomPassword()
    {
        var defaultPassword = configuration.GetValue<string>("DefaultPassword");
        if (string.IsNullOrEmpty(defaultPassword))
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(8));
        }
        return defaultPassword;
    }
}
