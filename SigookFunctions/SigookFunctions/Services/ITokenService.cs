namespace SigookFunctions.Services
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
    }
}
