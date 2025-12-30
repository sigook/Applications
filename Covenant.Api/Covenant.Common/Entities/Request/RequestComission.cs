using Covenant.Common.Entities.Agency;

namespace Covenant.Common.Entities.Request
{
    public class RequestComission
    {
        public Guid RequestId { get; set; }
        public Guid AgencyPersonnelId { get; set; }
        public virtual Request Request { get; set; }
        public virtual AgencyPersonnel AgencyPersonnel { get; set; }
    }
}
