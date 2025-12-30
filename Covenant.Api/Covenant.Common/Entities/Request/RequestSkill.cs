using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Request
{
    public class RequestSkill
    {
        private RequestSkill()
        {
        }

        public Guid Id { get; internal set; } = Guid.NewGuid();
        public string Skill { get; internal set; }
        public Request Request { get; private set; }
        public Guid RequestId { get; private set; }

        public static Result<RequestSkill> Create(Guid requestId, string skill)
        {
            return string.IsNullOrEmpty(skill)
                ? Result.Fail<RequestSkill>(ValidationMessages.RequiredMsg(nameof(Skill)))
                : Result.Ok(new RequestSkill { RequestId = requestId, Skill = skill.Trim() });
        }
    }
}