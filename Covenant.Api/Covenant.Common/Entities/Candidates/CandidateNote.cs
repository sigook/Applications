namespace Covenant.Common.Entities.Candidate
{
    public class CandidateNote
    {
        private CandidateNote()
        {
        }

        public CandidateNote(Guid candidateId, CovenantNote note)
        {
            CandidateId = candidateId;
            Note = note ?? throw new ArgumentNullException(nameof(note));
            NoteId = note.Id;
        }

        public Guid CandidateId { get; internal set; }
        public Candidate Candidate { get; internal set; }
        public Guid NoteId { get; internal set; }
        public CovenantNote Note { get; internal set; }

        public void Delete(string updateBy) => Note?.Delete(updateBy);
        public void Update(CovenantNote note, string updatedBy) => Note?.Update(note, updatedBy);
    }
}