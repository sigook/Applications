namespace SigookFunctions.Options
{
    public class SendGridOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ReplyToEmail { get; set; } = "it@covenantgroupl.com";
        public string FromEmail { get; set; } = "it@covenantgroupl.com";
        public string TestingEmail { get; set; } = "it@covenantgroupl.com";
        public string NewJobTemplateId { get; set; } = "d-4bf3a224275f4d6fb24f4f2a7e326daa";
        public bool SandBoxMode { get; set; } = true;
        public string BCCEmail { get; set; } = string.Empty;
    }
}
