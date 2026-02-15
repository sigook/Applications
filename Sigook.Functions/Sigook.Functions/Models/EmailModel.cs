using System.Collections.Generic;

namespace Sigook.Functions.Models
{
	public class EmailModel
	{
		public List<string> Tos { get; set; } = new List<string>();
		public string ReplyTo { get; set; }
		public dynamic Data { get; set; }
		public string Type { get; set; }
	}
	
}