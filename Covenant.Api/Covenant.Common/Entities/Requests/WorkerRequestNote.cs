namespace Covenant.Common.Entities.Request
{
    public class WorkerRequestNote
    {
        private WorkerRequestNote()
        {
        }
        public WorkerRequestNote(Guid workerRequestId, CovenantNote note)
        {
            WorkerRequestId = workerRequestId;
            Note = note ?? throw new ArgumentNullException(nameof(note));
            NoteId = note.Id;
        }

        public Guid WorkerRequestId { get; internal set; }
        public WorkerRequest WorkerRequest { get; internal set; }
        public Guid NoteId { get; internal set; }
        public CovenantNote Note { get; internal set; }

        public void Update(CovenantNote note, string updatedBy) => Note?.Update(note, updatedBy);
        public void Delete(string updatedBy) => Note?.Delete(updatedBy);
    }
}