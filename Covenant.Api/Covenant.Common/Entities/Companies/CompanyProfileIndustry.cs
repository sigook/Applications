using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Companies;

public class CompanyProfileIndustry
{
    public CompanyProfileIndustry()
    {
    }

    public CompanyProfileIndustry(string otherIndustry) => OtherIndustry = otherIndustry;
    public CompanyProfileIndustry(Guid industryId) => IndustryId = industryId;

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid? IndustryId { get; private set; }
    public Industry Industry { get; private set; }
    public string OtherIndustry { get; private set; }

    public static Result<CompanyProfileIndustry> Create(BaseModel<Guid> industry, string otherIndustry = null)
    {
        if (!string.IsNullOrEmpty(otherIndustry))
        {
            return Result.Ok(new CompanyProfileIndustry(otherIndustry));
        }

        if (industry != null && industry.Id != Guid.Empty)
        {
            return Result.Ok(new CompanyProfileIndustry(industry.Id));
        }

        return Result.Fail<CompanyProfileIndustry>(ValidationMessages.RequiredMsg(ApiResources.Industry));
    }

    public static Result<CompanyProfileIndustry> Create(string otherIndustry) => !string.IsNullOrEmpty(otherIndustry)
            ? Result.Ok(new CompanyProfileIndustry(otherIndustry))
            : Result.Fail<CompanyProfileIndustry>(ValidationMessages.RequiredMsg(ApiResources.Industry));

    public void Update(Guid? industryId, string otherIndustry)
    {
        IndustryId = industryId;
        OtherIndustry = otherIndustry;
    }
}
