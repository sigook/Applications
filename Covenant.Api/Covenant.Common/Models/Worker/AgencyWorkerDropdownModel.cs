namespace Covenant.Common.Models.Worker
{
    public class AgencyWorkerDropdownModel
    {
        public Guid Id { get; set; }
        public string SocialInsurance { get; set; }
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
        public Guid WorkerProfileId { get; set; }
        public bool ApprovedToWork { get; set; }
        public string Value => $"{SocialInsurance} {FullName}";
    }
}