namespace Covenant.IdentityServer.Services;

public interface IMicrosoft365AccountService
{
    Task<bool> IsAccountEnabledAsync(string objectId);
}
