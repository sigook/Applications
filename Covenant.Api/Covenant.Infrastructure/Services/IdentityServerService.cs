using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Security;
using Covenant.Common.Repositories;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;

namespace Covenant.Infrastructure.Services;

public class IdentityServerService : IIdentityServerService
{
    public const string IdentityClient = "Identity";

    private readonly IUserRepository userRepository;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogger<IdentityServerService> _logger;

    public IdentityServerService(
        IUserRepository userRepository,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<IdentityServerService> logger)
    {
        this.userRepository = userRepository;
        this.httpClientFactory = httpClientFactory;
        this.configuration = configuration;
        this.httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<Result<User>> CreateUser(CreateUserModel model)
    {
        try
        {
            var client = httpClientFactory.CreateClient(IdentityClient);
            if (string.IsNullOrEmpty(model.Password))
            {
                model.Password = RandomPassword();
            }
            var content = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync("CreateUser", stringContent);
            var stringResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<IdModel>(stringResponse);
                var newUser = new User(model.Email, result.Id);
                await userRepository.Create(newUser);
                await userRepository.SaveChangesAsync();
                return Result.Ok(newUser);
            }
            return Result.Fail<User>(stringResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating worker: {Error}", ex.Message);
            return Result.Fail<User>("There was an error creating the user please try again later");
        }
    }

    public async Task<Result> UpdateAgencyUser(Guid userId, IdModel agency)
    {
        try
        {
            var client = httpClientFactory.CreateClient(IdentityClient);
            var content = JsonConvert.SerializeObject(agency);
            var stringContent = new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PutAsync($"UpdateAgencyUser/{userId}", stringContent);
            var stringResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return Result.Ok();
            }
            return Result.Fail<User>(stringResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating worker: {Error}", ex.Message);
            return Result.Fail<User>("There was an error creating the user please try again later");
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
            var client = httpClientFactory.CreateClient(IdentityClient);
            var content = JsonConvert.SerializeObject(claim);
            var stringContent = new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PutAsync($"DeleteUserOrClaim/{userId}", stringContent);
            var stringResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var userDeleted = JsonConvert.DeserializeObject<bool>(stringResponse);
                if (userDeleted)
                {
                    userRepository.Delete(user);
                    await userRepository.SaveChangesAsync();
                }
                return Result.Ok();
            }
            return Result.Fail(stringResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error deleting agency personnel: {Error}", ex.Message);
            return Result.Fail("There was an error deleting the user please try again later");
        }
    }

    public Result<string> HashPassword(string password)
    {
        var user = new PasswordHasher<CovenantUser>();
        var passwordHashed = user.HashPassword(new CovenantUser(), password);
        return Result.Ok(passwordHashed);
    }

    public async Task<Result> InactiveUser(Guid id)
    {
        try
        {
            var client = httpClientFactory.CreateClient(IdentityClient);
            var content = JsonConvert.SerializeObject(new IdModel(id));
            var response = await client.PostAsync("CreateInactiveUser", new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json));
            var stringResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var user = await userRepository.GetUserById(id);
                user.InactiveUser();
                await userRepository.SaveChangesAsync();
                return Result.Ok();
            }
            return Result.Fail(stringResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error inactiving user {Error}", ex.Message);
            return Result.Fail("There was an error inactiving the user please try again later");
        }
    }

    public async Task<Result> UpdateUserEmail(UpdateEmailModel model)
    {
        try
        {
            var email = CvnEmail.Create(model.NewEmail);
            if (email)
            {
                if (!await userRepository.UserExists(email.Value.Email))
                {
                    var client = httpClientFactory.CreateClient(IdentityClient);
                    var content = JsonConvert.SerializeObject(model);
                    var stringContent = new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json);
                    var response = await client.PutAsync("UpdateEmail", stringContent);
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var user = await userRepository.GetUserById(model.Id);
                        user.UpdateEmail(model.NewEmail);
                        await userRepository.SaveChangesAsync();
                        return Result.Ok();
                    }
                    return Result.Fail(stringResponse);
                }
                return Result.Fail(ApiResources.EmailAlreadyTaken);
            }
            return email;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error updating user's email {Error}", ex.Message);
            return Result.Fail("There was an error updating the email please try again later");
        }
    }

    public async Task<Result> UpdateUserRole(UpdateRoleModel model)
    {
        try
        {
            var client = httpClientFactory.CreateClient(IdentityClient);
            var content = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PutAsync("UpdateRole", stringContent);
            var stringResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return Result.Ok();
            }
            return Result.Fail(stringResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error updating user's email {Error}", ex.Message);
            return Result.Fail("There was an error updating the email please try again later");
        }
    }

    public string GetNickname() => httpContextAccessor.HttpContext?.User?.GetNickname();

    public Guid GetCompanyId() => httpContextAccessor.HttpContext.User.GetCompanyId();

    public Guid GetAgencyId() => httpContextAccessor.HttpContext.User.GetAgencyId();

    public Guid GetUserId() => httpContextAccessor.HttpContext.User.GetUserId();

    public bool IsAdmin() => httpContextAccessor.HttpContext.User.IsPayrollManager();

    public IEnumerable<Guid> GetAgencyIds() => httpContextAccessor.HttpContext.User.GetAgencyIds();

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