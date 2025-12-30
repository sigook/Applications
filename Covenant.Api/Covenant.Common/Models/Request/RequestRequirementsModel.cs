using Covenant.Common.Constants;
using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Models.Request
{
    public class RequestRequirementsModel
    {
        private RequestRequirementsModel()
        {
        }

        public string Value { get; private set; }

        public static Result<RequestRequirementsModel> Create(string requirements)
        {
            if (string.IsNullOrEmpty(requirements))
                return Result.Fail<RequestRequirementsModel>(ValidationMessages.RequiredMsg(ApiResources.Requirements));
            if (requirements.Length > CovenantConstants.Validation.MaximumLengthRequirements)
            {
                return Result.Fail<RequestRequirementsModel>(
                    ValidationMessages.LessThanOrEqualMsg(ApiResources.Requirements, CovenantConstants.Validation.MaximumLengthRequirements));
            }
            return Result.Ok(new RequestRequirementsModel { Value = requirements });
        }
    }
}