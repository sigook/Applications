namespace Covenant.Common.Models.Company
{
    public class CompanyWorkerProfileDetailModel
    {
        public Guid Id { get; set; }
        public Guid WorkerId { get; set; }
        public CovenantFileModel ProfileImage { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public BaseModel<Guid> Gender { get; set; }
        public bool Dnu { get; set; }
        public bool HaveAnyHealthProblem { get; set; }
        public string HealthProblem { get; set; }
        public string OtherHealthProblem { get; set; }
        public string ContactEmergencyName { get; set; }
        public string ContactEmergencyLastName { get; set; }
        public string ContactEmergencyPhone { get; set; }
        public bool ApprovedToWork { get; set; }
        public bool AgencyRequiredPaymentMethod { get; set; }
        public bool CompanyHasPaymentMethod { get; set; }
        public bool CompanyActive { get; set; }
        public List<string> Availabilities { get; set; } = new List<string>();
        public List<string> AvailabilityTimes { get; set; } = new List<string>();
        public List<string> AvailabilityDays { get; set; } = new List<string>();
        public List<string> LocationPreferences { get; set; } = new List<string>();
        public string Lift { get; set; }
        public List<string> Languages { get; set; } = new List<string>();
        public List<string> Skills { get; set; } = new List<string>();
        public bool CanCreateRequest
        {
            get
            {
                if (!CompanyActive) return false;
                if (!AgencyRequiredPaymentMethod) return true;
                if (!CompanyHasPaymentMethod) return false;
                return true;
            }
        }
    }
}
