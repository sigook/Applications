using Covenant.Common.Entities.Requests;
using Covenant.Common.Functionals;
using Covenant.Common.Models.Requests;

namespace Covenant.Common.Interfaces.Adapters;

public interface IRequestAdapter
{
    Result<List<RequestComplianceItem>> MapToComplianceItems(Guid requestId, IEnumerable<RequestComplianceItemModel> models);
}
