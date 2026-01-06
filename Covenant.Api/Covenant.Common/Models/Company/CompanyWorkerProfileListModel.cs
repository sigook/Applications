namespace Covenant.Common.Models.Company
{
    public class CompanyWorkerProfileListModel
    {
        public Guid Id { get; set; }
        public int NumberId { get; set; }
        public string Name { get; set; }
        public Guid WorkerId { get; set; }
        public string ProfileImage { get; set; }
        public string LastName { get; set; }
        public IEnumerable<string> Availabilities { get; set; }
        public IEnumerable<string> Skills { get; set; }
        public Guid AgencyId { get; set; }
        public string AgencyFullName { get; set; }
        public bool ApprovedToWork { get; set; }
        public bool AgencyRequiredPaymentMethod { get; set; }
        public bool CompanyHasPaymentMethod { get; set; }
        public bool CompanyActive { get; set; }
        public bool IsCurrentlyWorking { get; set; }
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
