namespace Covenant.IdentityServer.Configuration;

public static class ConfigurationExtensions
{
    public static string GetAppUrl(this IConfiguration configuration) => configuration.GetValue<string>("AppUrl");
}
