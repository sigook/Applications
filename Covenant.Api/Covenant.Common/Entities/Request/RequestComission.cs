using Covenant.Common.Entities.Agency;

namespace Covenant.Common.Entities.Request
{
    public class RequestComission
    {
        internal RequestComission()
        {
        }

        public RequestComission(Guid requestId, Guid agencyPersonnelId)
        {
            RequestId = requestId;
            AgencyPersonnelId = agencyPersonnelId;
        }

        public Guid RequestId { get; internal set; }
        public Guid AgencyPersonnelId { get; internal set; }
        public virtual Request Request { get; private set; }
        public virtual AgencyPersonnel AgencyPersonnel { get; private set; }

        public void ChangeAgencyPersonnel(Guid agencyPersonnelId) => AgencyPersonnelId = agencyPersonnelId;
    }
}
