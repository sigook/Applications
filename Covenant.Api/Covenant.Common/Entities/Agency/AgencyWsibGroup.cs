namespace Covenant.Common.Entities.Agency
{
    public class AgencyWsibGroup
    {
        public AgencyWsibGroup() 
        { 
        }

        public AgencyWsibGroup(Guid agencyId, Guid wsibGroupId)
        {
            AgencyId = agencyId;
            WsibGroupId = wsibGroupId;
        }

        public Guid AgencyId { get; private set; }
        public Agency Agency { get; private set; }

        public Guid WsibGroupId { get; private set; }
        public WsibGroup WsibGroup { get; private set; }
    }
}