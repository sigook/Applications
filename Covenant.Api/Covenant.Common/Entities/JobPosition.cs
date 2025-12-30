using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities
{
    public class JobPosition
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Value { get; set; }
        public Industry Industry { get; set; }
        public Guid IndustryId { get; set; }
        public bool IsDeleted { get; private set; }

        public static Result<JobPosition> Create(Guid industryId, string value) =>
            string.IsNullOrEmpty(value)
                ? Result.Fail<JobPosition>(ValidationMessages.RequiredMsg(nameof(Value)))
                : Result.Ok(new JobPosition { IndustryId = industryId, Value = value });

        public void Update(JobPosition value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            Value = value.Value;
        }

        public void Delete() => IsDeleted = true;
    }
}