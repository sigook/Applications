namespace Covenant.Common.Interfaces.Identity;

public interface IMicrosoft365AccountService
{
    Task<bool> IsAccountEnabled(string objectId);
}
