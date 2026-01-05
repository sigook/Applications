namespace Covenant.Common.Entities.Company
{
    public class CompanyProfileNote
    {
        private CompanyProfileNote()
        {
        }
        public CompanyProfileNote(Guid companyProfileId, CovenantNote note)
        {
            CompanyProfileId = companyProfileId;
            Note = note ?? throw new ArgumentNullException(nameof(note));
            NoteId = note.Id;
        }

        public Guid CompanyProfileId { get; internal set; }
        public CompanyProfile CompanyProfile { get; internal set; }
        public Guid NoteId { get; internal set; }
        public CovenantNote Note { get; internal set; }

        public void Update(CovenantNote note, string updatedBy) => Note?.Update(note, updatedBy);
        public void Delete(string updatedBy) => Note?.Delete(updatedBy);
    }
}