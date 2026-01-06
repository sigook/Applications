namespace Covenant.Common.Models.Candidate
{
    public class CandidateListModel
    {
        public Guid Id { get; set; }
        public Guid AgencyId { get; set; }
        public IEnumerable<BaseModel<Guid>> Requests { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string ResidencyStatus { get; set; }
        public IEnumerable<PhoneNumberModel> PhoneNumbers { get; set; }
        public IEnumerable<SkillModel> Skills { get; set; }
        public string Recruiter { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HasVehicle { get; set; }
        public bool HasDocuments { get; set; }
        public int NotesCount { get; set; }
        public bool Dnu { get; set; }
        public string Source { get; set; }
    }
}