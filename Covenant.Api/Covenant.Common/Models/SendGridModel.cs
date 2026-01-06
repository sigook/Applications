namespace Covenant.Common.Models
{
    public class SendGridModel
    {
        public object Data { get; set; }
        public string Template { get; set; }
        public IEnumerable<string> Tos { get; set; }
    }
}