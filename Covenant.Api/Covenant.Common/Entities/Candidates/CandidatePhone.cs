namespace Covenant.Common.Entities.Candidate
{
    public class CandidatePhone
    {
        public CandidatePhone()
        {
        }

        public CandidatePhone(Guid candidateId, string phoneNumber)
        {
            CandidateId = candidateId;
            PhoneNumber = phoneNumber;
        }

        public Guid Id { get; internal set; } = Guid.NewGuid();
        public string PhoneNumber { get; internal set; }
        public Candidate Candidate { get; private set; }
        public Guid CandidateId { get; internal set; }
    }
}