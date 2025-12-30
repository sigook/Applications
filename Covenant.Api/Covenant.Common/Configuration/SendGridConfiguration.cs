namespace Covenant.Common.Configuration
{
    public class SendGridConfiguration
    {
        public string ApiKey { get; set; }
        public string From { get; set; }
        public bool SandBox { get; set; }
        public string TemplatesUrl { get; set; }
    }
}
