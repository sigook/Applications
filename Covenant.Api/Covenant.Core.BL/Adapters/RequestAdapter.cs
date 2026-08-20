using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Models.Request;

namespace Covenant.Core.BL.Adapters;

public class RequestAdapter : IRequestAdapter
{
    public Result<List<RequestComplianceItem>> MapToComplianceItems(Guid requestId, IEnumerable<RequestComplianceItemModel> models)
    {
        var items = new List<RequestComplianceItem>();
        foreach (var model in models ?? [])
        {
            var item = RequestComplianceItem.Create(requestId, model.Name, model.IsMandatory, model.DocumentTarget);
            if (!item) return Result.Fail<List<RequestComplianceItem>>(item.Errors);
            items.Add(item.Value);
        }
        return Result.Ok(items);
    }
}
