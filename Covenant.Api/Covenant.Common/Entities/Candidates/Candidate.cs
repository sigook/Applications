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

        public Guid Id { get; private set; } = Guid.NewGuid();
        public long NumberId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Address { get; internal set; }
        public string PostalCode { get; private set; }
        public Guid? CityId { get; internal set; }
        public City City { get; private set; }
        public Guid? GenderId { get; private set; }
        public Gender Gender { get; internal set; }
        public bool HasVehicle { get; internal set; }
        public Guid? SourceId { get; internal set; }
        public Source Source { get; private set; }
        public string Recruiter { get; internal set; }
        public string ResidencyStatus { get; internal set; }
        public IList<CandidatePhone> PhoneNumbers { get; private set; } = new List<CandidatePhone>();
        public IList<CandidateSkill> Skills { get; private set; } = new List<CandidateSkill>();
        public IList<CandidateDocument> Documents { get; internal set; } = new List<CandidateDocument>();
        public IList<CandidateNote> Notes { get; private set; } = new List<CandidateNote>();
        public IList<RequestApplicant> RequestApplicants { get; private set; } = new List<RequestApplicant>();
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public Agency.Agency Agency { get; internal set; }
        public Guid AgencyId { get; internal set; }
        public bool Dnu { get; internal set; } = false;
        public bool EmailUnsubscribed { get; private set; } = false;

        public void AddEmail(CvnEmail email) => Email = email.Email;

        public void UnsubscribeFromEmails() => EmailUnsubscribed = true;

        public Result CanConvertToWorker()
        {
            if (string.IsNullOrWhiteSpace(Email))
                return Result.Fail("The candidate must have an email to be converted to a worker");
            if (!PhoneNumbers.Any())
                return Result.Fail("The candidate must have a phone number to be converted to a worker");
            if (!CityId.HasValue)
                return Result.Fail("The candidate must have a city to be converted to a worker");
            if (Dnu)
                return Result.Fail("A candidate marked as Do Not Use cannot be converted to a worker");
            return Result.Ok();
        }

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

        public void UpdateProfile(string name, string address, Guid? cityId, string residencyStatus, Guid? genderId, bool hasVehicle, Guid? sourceId)
        {
            Name = name;
            Address = address;
            CityId = cityId;
            ResidencyStatus = residencyStatus;
            GenderId = genderId;
            HasVehicle = hasVehicle;
            SourceId = sourceId;
        }

        public void AssignRecruiter(string recruiter) => Recruiter = recruiter;

        public void UpdateDnu(bool dnu) => Dnu = dnu;
    }
}
