namespace Covenant.Common.Entities.Request
{
    public class RequestNote
    {
        private RequestNote()
        {
        }
        public RequestNote(Guid requestId, CovenantNote note)
        {
            RequestId = requestId;
            Note = note ?? throw new ArgumentNullException(nameof(note));
            NoteId = note.Id;
        }

        public Guid RequestId { get; internal set; }
        public Request Request { get; internal set; }
        public Guid NoteId { get; internal set; }
        public CovenantNote Note { get; internal set; }

        public void Update(CovenantNote note, string updatedBy) => Note?.Update(note, updatedBy);
        public void Delete(string updatedBy) => Note?.Delete(updatedBy);
    }
}