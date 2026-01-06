using Covenant.Common.Constants;
using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Models.Request
{
    public class RequestDescriptionModel
    {
        private RequestDescriptionModel() 
        { 
        }

        public string Value { get; private set; }
        
        public static Result<RequestDescriptionModel> Create(string description)
        {
            if (string.IsNullOrEmpty(description))
                return Result.Fail<RequestDescriptionModel>(ValidationMessages.RequiredMsg(ApiResources.Description));
            if (description.Length > CovenantConstants.Validation.MaximumLengthDescription)
            {
                return Result.Fail<RequestDescriptionModel>(
                    ValidationMessages.LessThanOrEqualMsg(ApiResources.Description, CovenantConstants.Validation.MaximumLengthDescription));
            }
            return Result.Ok(new RequestDescriptionModel { Value = description });
        }
    }
}