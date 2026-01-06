using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Candidate
{
    public class Candidate
    {
        private Candidate()
        {
        }

        public Candidate(Guid agencyId, string name)
        {
            AgencyId = agencyId;
            Name = name;
        }

        public Candidate(Guid agencyId, string name, CvnEmail email) : this(agencyId, name) => Email = email.Email;

        public Guid Id { get; set; } = Guid.NewGuid();
        public long NumberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public Guid? GenderId { get; set; }
        public Gender Gender { get; set; }
        public bool HasVehicle { get; set; }
        public string Source { get; set; }
        public string Recruiter { get; set; }
        public string ResidencyStatus { get; set; }
        public IList<CandidatePhone> PhoneNumbers { get; set; } = new List<CandidatePhone>();
        public IList<CandidateSkill> Skills { get; set; } = new List<CandidateSkill>();
        public IList<CandidateDocument> Documents { get; set; } = new List<CandidateDocument>();
        public IList<CandidateNote> Notes { get; set; } = new List<CandidateNote>();
        public IList<RequestApplicant> RequestApplicants { get; set; } = new List<RequestApplicant>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Agency.Agency Agency { get; set; }
        public Guid AgencyId { get; set; }
        public bool Dnu { get; set; } = false;

        public void AddEmail(CvnEmail email) => Email = email.Email;

        public void AddPostalCode(CvnPostalCode postalCode) => PostalCode = postalCode.PostalCode;

        public Result<CandidateSkill> AddSkill(string skill)
        {
            if (Skills.Any(c => c.Skill.Equals(skill, StringComparison.InvariantCultureIgnoreCase)))
                return Result.Fail<CandidateSkill>(ValidationMessages.AlreadyExists(nameof(skill)));
            var result = CandidateSkill.Create(Id, skill);
            if (result) Skills.Add(result.Value);
            return result;
        }

        public Result DeleteSkill(Guid id)
        {
            CandidateSkill skill = Skills.FirstOrDefault(c => c.Id == id);
            if (skill == null) return Result.Fail();
            Skills.Remove(skill);
            return Result.Ok();
        }

        public Result<CandidatePhone> AddPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return Result.Fail<CandidatePhone>(ValidationMessages.RequiredMsg(ApiResources.Phone));
            var entity = new CandidatePhone(Id, phone);
            PhoneNumbers.Add(entity);
            return Result.Ok(entity);
        }

        public Result DeletePhone(Guid id)
        {
            var entity = PhoneNumbers.FirstOrDefault(c => c.Id == id);
            if (entity is null) return Result.Fail();
            PhoneNumbers.Remove(entity);
            return Result.Ok();
        }
    }
}