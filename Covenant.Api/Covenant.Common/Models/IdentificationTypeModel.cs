using Covenant.Common.Enums;

namespace Covenant.Common.Models;

public class IdentificationTypeModel : BaseModel<Guid>
{
    public IdentificationTypeCode Code { get; set; }
}
