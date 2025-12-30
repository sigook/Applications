namespace Covenant.Common.Models.Worker
{
    public class WorkerProfileBasicInfoModel
    {
        public int NumberId { get; set; }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public bool ApprovedToWork { get; set; }
        public CovenantFileModel ProfileImage { get; set; }
        public bool HasSocialInsurance { get; set; }
        public bool HasSocialInsuranceFile { get; set; }
        public bool HasIdentificationNumber1 { get; set; }
        public bool HasIdentificationType1File { get; set; }
        public bool HasIdentificationNumber2 { get; set; }
        public bool HasIdentificationType2File { get; set; }
        public bool HasResume { get; set; }
        public string PunchCardId { get; set; }
    }
}
