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

        public Guid AgencyId { get; set; }
        public Agency Agency { get; set; }

        public Guid WsibGroupId { get; set; }
        public WsibGroup WsibGroup { get; set; }
    }
}