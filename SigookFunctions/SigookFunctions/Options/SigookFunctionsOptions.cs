namespace SigookFunctions.Options
{
    public class SigookFunctionsOptions
    {
        public const string SectionName = "SigookFunctions";

        public bool IsProduction { get; set; }
        public SendGridOptions SendGrid { get; set; } = new();
        public UrlsOptions Urls { get; set; } = new();
        public AuthOptions Auth { get; set; } = new();
        public string TeamsWebhook { get; set; } = string.Empty;
    }
}
