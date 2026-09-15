namespace Sigook.Functions.Services;

public interface IAccessTokenProvider
{
    Task<string> GetToken();
}
