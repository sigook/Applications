using System.Collections.Generic;

namespace Sigook.Functions.Models
{
	public class PaginatedList<T>
	{
		public int PageIndex { get; set; }
		public int TotalPages { get; set; }
		public int TotalItems { get; set; }
		public List<T> Items { get; set; } = new List<T>();
		public bool HasPreviousPage => PageIndex > 1;
		public bool HasNextPage => PageIndex < TotalPages;
	}
}