namespace Covenant.Common.Models.Candidate
{
    public class CandidateCreateModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public BaseModel<Guid> Gender { get; set; }
        public bool HasVehicle { get; set; }
        public string ResidencyStatus { get; set; }
        public string Source { get; set; }
        public bool Dnu { get; set; }
        public IEnumerable<PhoneNumberModel> PhoneNumbers { get; set; } = new List<PhoneNumberModel>();
        public IEnumerable<SkillModel> Skills { get; set; } = new List<SkillModel>();
        public string FileName { get; set; }
    }
}