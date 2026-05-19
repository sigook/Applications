namespace Covenant.Common.Entities
{
    public class Source : ICatalog<Guid>
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public ICollection<Candidate.Candidate> Candidates { get; set; } = new List<Candidate.Candidate>();
    }
}
