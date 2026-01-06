namespace Covenant.Common.Models.Candidate
{
    public class CandidateDetailModel
    {
        public Guid Id { get; set; }
        public long NumberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public BaseModel<Guid> Gender { get; set; }
        public bool HasVehicle { get; set; }
        public string ResidencyStatus { get; set; }
        public bool Dnu { get; set; }
    }
}