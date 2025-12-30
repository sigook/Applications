namespace Covenant.Api.Common.Models
{
    public class JobSearchModel
    {
        public string JobId { get; set; }
        public string JobTitle { get; set; }
        public string Location { get; set; }
        public IEnumerable<string> Countries { get; set; }
    }
}
