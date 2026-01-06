using Microsoft.Extensions.Configuration;

namespace Covenant.Common.Utils.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetAppUrl(this IConfiguration configuration) => configuration.GetValue<string>("AppUrl");

        public static string GetWebClientUrl(this IConfiguration configuration) => configuration.GetValue<string>("WebClientUrl");

        public static string GetFilesBaseUrl(this IConfiguration configuration) => configuration["FilesConfiguration:FilesPath"];
    }
}
