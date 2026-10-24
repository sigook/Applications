using Covenant.Common.Entities.Candidates;
namespace Covenant.Common.Entities
{
    public class Source : ICatalog<Guid>
    {
        public Guid Id { get; internal set; }
        public string Value { get; internal set; }
        public bool IsAvailableForRequests { get; private set; }
        public ICollection<Candidate> Candidates { get; private set; } = new List<Candidate>();
    }
}
