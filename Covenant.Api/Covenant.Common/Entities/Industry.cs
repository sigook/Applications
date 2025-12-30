using Covenant.Common.Entities.Company;
using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities;

public class Industry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Value { get; set; }
    public bool IsDeleted { get; private set; }
    public ICollection<JobPosition> JobPositions { get; set; } = new List<JobPosition>();
    public ICollection<CompanyProfileIndustry> CompanyProfileIndustries { get; set; }

    public static Result<Industry> Create(string value) =>
        string.IsNullOrEmpty(value)
            ? Result.Fail<Industry>(ValidationMessages.RequiredMsg(nameof(Value)))
            : Result.Ok(new Industry { Value = value });
}