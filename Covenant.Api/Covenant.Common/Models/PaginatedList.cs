namespace Covenant.Common.Models
{
    public abstract class PaginatedList
    {
        public int PageIndex { get; set; }

        public int TotalPages { get; set; }

        public int TotalItems { get; set; }
    }

    public class PaginatedList<T> : PaginatedList
    {
        public List<T> Items { get; set; } = new List<T>();
    }
}
