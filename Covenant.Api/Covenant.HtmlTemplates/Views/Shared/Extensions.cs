using System;
using Microsoft.Extensions.Configuration;

namespace Covenant.HtmlTemplates.Views.Shared
{
    public static class Extensions
    {
        public static string GetAppUrl(this IConfiguration configuration)
        {
            return configuration.GetSection("AppUrl")?.Value;
        }

        public static string GetWebClientUrl(this IConfiguration configuration) => configuration?.GetSection("WebClientUrl")?.Value;

        public static string GetFilesBaseUrl(this IConfiguration configuration) =>  configuration["FilesConfiguration:FilesPath"];

        public static string ToUsMoney(this decimal value) => string.Format(new System.Globalization.CultureInfo("en-US"),"{0:C}",value);
        
        public static string InvoiceDateDisplay(this DateTime dateTime) => dateTime.ToString("dd MMMM yyyy");
        
        public static string PayStubDateDisplay(this DateTime dateTime) => dateTime.ToString("dd MMMM yyyy");
    }
}