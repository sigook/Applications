using Newtonsoft.Json;

namespace Covenant.Common.Models
{
    public abstract class PaginatedList
    {
        [JsonProperty]
        public int PageIndex { get; set; }

        [JsonProperty]
        public int TotalPages { get; set; }

        [JsonProperty]
        public int TotalItems { get; set; }
    }

    public class PaginatedList<T> : PaginatedList
    {
        public List<T> Items { get; set; } = new List<T>();
    }
}
