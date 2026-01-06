namespace Covenant.Common.Models.Worker
{
    public class EmergencyInformationModel : IEmergencyInformation
    {
        public bool HaveAnyHealthProblem { get; set; }
        public string HealthProblem { get; set; }
        public string ContactEmergencyPhone { get; set; }
        public string OtherHealthProblem { get; set; }
        public string ContactEmergencyName { get; set; }
        public string ContactEmergencyLastName { get; set; }
    }
}