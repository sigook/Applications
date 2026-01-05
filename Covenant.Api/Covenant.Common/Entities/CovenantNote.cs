using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities
{
    public class CovenantNote
    {
        public const string RedColor = "#fefefe";
        public Guid Id { get; internal set; }
        public string Note { get; internal set; }
        public string Color { get; internal set; }
        public string CreatedBy { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        private CovenantNote()
        {
        }

        public static Result<CovenantNote> Create(string note, string color, string createdBy, Guid id = default)
        {
            if (string.IsNullOrEmpty(note)) return Result.Fail<CovenantNote>(ValidationMessages.RequiredMsg(nameof(Note)));
            return Result.Ok(new CovenantNote
            {
                Id = id == default ? Guid.NewGuid() : id,
                Note = note,
                Color = color,
                CreatedBy = createdBy,
                CreatedAt = DateTime.Now
            });
        }

        public void Update(CovenantNote note, string updatedBy)
        {
            if (note is null) throw new ArgumentNullException(nameof(note));
            Note = note.Note;
            Color = note.Color;
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.Now;
        }

        public void Delete(string updatedBy)
        {
            IsDeleted = true;
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.Now;
        }
    }
}