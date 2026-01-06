namespace Covenant.Common.Models.Agency
{
    public class InvitationToApplyModel
    {
        public Guid RequestId { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string City { get; set; }
        public string Rate { get; set; }
        public Guid AgencyId { get; set; }

        public bool IsValidModel()
        {
            if (RequestId == default) return false;
            if (string.IsNullOrEmpty(JobTitle)) return false;
            if (string.IsNullOrEmpty(Description)) return false;
            if (string.IsNullOrEmpty(Requirements)) return false;
            if (string.IsNullOrEmpty(Rate)) return false;
            if (AgencyId == default) return false;
            return true;
        }

        public override string ToString() => $"RequestId={RequestId},JobTitle:{JobTitle},Description:{Description},Requirements:{Requirements},Rate={Rate},City={City},AgencyId={AgencyId}";
    }
}
