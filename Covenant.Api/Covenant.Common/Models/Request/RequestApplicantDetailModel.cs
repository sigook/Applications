namespace Covenant.Common.Models.Request
{
    public class RequestApplicantDetailModel : RequestApplicantModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid? WorkerId { get; set; }
    }
}