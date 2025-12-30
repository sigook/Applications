using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Candidate
{
    public class CandidateSkill
    {
        private CandidateSkill()
        {
        }

        public Guid Id { get; internal set; } = Guid.NewGuid();
        public string Skill { get; internal set; }

        public Candidate Candidate { get; private set; }
        public Guid CandidateId { get; internal set; }

        public static Result<CandidateSkill> Create(Guid candidateId, string skill)
        {
            return string.IsNullOrEmpty(skill)
                ? Result.Fail<CandidateSkill>(ValidationMessages.RequiredMsg(nameof(Skill)))
                : Result.Ok(new CandidateSkill { CandidateId = candidateId, Skill = skill.Trim() });
        }
    }
}