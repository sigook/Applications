using Covenant.Common.Entities.Worker;

namespace Covenant.Common.Models.Worker
{
    public class SinInformationModel : ISinInformation<CovenantFileModel>
    {
        public string SocialInsurance { get; set; }
        public bool SocialInsuranceExpire { get; set; }
        public DateTime? DueDate { get; set; }
        public CovenantFileModel SocialInsuranceFile { get; set; }
    }
}