namespace Covenant.Common.Models
{
	public class NoteModel
	{
		public NoteModel()
		{
		}
		public NoteModel(string note, string color)
		{
			Note = note;
			Color = color;
		}

        public Guid Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Note { get; set; }
		public string Color { get; set; }
	}
}