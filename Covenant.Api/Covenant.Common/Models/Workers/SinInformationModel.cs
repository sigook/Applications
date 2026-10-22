using Covenant.Common.Entities.Workers;

namespace Covenant.Common.Models.Workers
{
    public class SinInformationModel : ISinInformation<CovenantFileModel>
    {
        public string SocialInsurance { get; set; }
        public bool SocialInsuranceExpire { get; set; }
        public DateTime? DueDate { get; set; }
        public CovenantFileModel SocialInsuranceFile { get; set; }
    }
}