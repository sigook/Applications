namespace Covenant.Common.Entities.Candidate
{
    public class CandidateDocument
    {
        private CandidateDocument()
        {
        }

        public CandidateDocument(Guid candidateId, CovenantFile document)
        {
            CandidateId = candidateId;
            Document = document ?? throw new ArgumentNullException(nameof(document));
            DocumentId = document.Id;
        }

        public Guid CandidateId { get; internal set; }
        public Candidate Candidate { get; internal set; }
        public Guid DocumentId { get; internal set; }
        public CovenantFile Document { get; internal set; }
    }
}