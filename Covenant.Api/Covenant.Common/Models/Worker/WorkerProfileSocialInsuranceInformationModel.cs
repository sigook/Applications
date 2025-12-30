namespace Covenant.Common.Models.Worker
{
    public class WorkerProfileSocialInsuranceInformationModel
    {
        public bool ApprovedToWork { get; set; }
        public string SocialInsurance { get; set; }
        public bool SocialInsuranceExpire { get; set; }
        public DateTime? DueDate { get; set; }
    }
}