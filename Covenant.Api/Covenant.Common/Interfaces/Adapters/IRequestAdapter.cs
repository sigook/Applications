using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Request;

namespace Covenant.Common.Interfaces.Adapters;

public interface IRequestAdapter
{
    Result<List<RequestComplianceItem>> MapToComplianceItems(Guid requestId, IEnumerable<RequestComplianceItemModel> models);
}
