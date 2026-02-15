using System.Collections.Generic;

namespace Sigook.Functions.Models
{
	public class EmailMessage
	{
		public string Subject { get; set; }
		public string Content { get; set; }
		public IEnumerable<string> To { get; set; } = new string[0];
	}
}