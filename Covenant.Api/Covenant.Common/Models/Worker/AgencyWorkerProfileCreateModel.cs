using Covenant.Common.Entities.Worker;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Security;

namespace Covenant.Common.Models.Worker
{
    public class AgencyWorkerProfileCreateModel :
        IWorkerBasicInformation<BaseModel<Guid>>,
        IWorkerContactInformation<LocationModel, CityModel>,
        IJobExperienceInformation<WorkerProfileJobExperienceModel>,
        IEmailInformation,
        IWorkerProfileStatusInformation,
        ISinInformation<CovenantFileModel>,
        IEmergencyInformation,
        IWorkerDocumentsInformation<BaseModel<Guid>, CovenantFileModel>,
        IWorkerProfileOtherInformation<BaseModel<Guid>>

    {
        public CovenantFileModel ProfileImage { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public DateTime BirthDay { get; set; }
        public BaseModel<Guid> Gender { get; set; }
        public string SocialInsurance { get; set; }
        public bool SocialInsuranceExpire { get; set; }
        public DateTime? DueDate { get; set; }
        public CovenantFileModel SocialInsuranceFile { get; set; }
        public string IdentificationNumber1 { get; set; }
        public string IdentificationNumber2 { get; set; }
        public bool HavePoliceCheckBackground { get; set; }
        public CovenantFileModel IdentificationType1File { get; set; }
        public CovenantFileModel IdentificationType2File { get; set; }
        public BaseModel<Guid> IdentificationType1 { get; set; }
        public BaseModel<Guid> IdentificationType2 { get; set; }
        public CovenantFileModel PoliceCheckBackGround { get; set; }
        public string MobileNumber { get; set; }
        public string Phone { get; set; }
        public int? PhoneExt { get; set; }
        public LocationModel Location { get; set; }
        public bool HasVehicle { get; set; }
        public IEnumerable<WorkerProfileLicenseModel> Licenses { get; set; } = Array.Empty<WorkerProfileLicenseModel>();
        public IEnumerable<CovenantFileModel> Certificates { get; set; } = Array.Empty<CovenantFileModel>();
        public IEnumerable<BaseModel<Guid>> Availabilities { get; set; } = Array.Empty<BaseModel<Guid>>();
        public IEnumerable<BaseModel<Guid>> AvailabilityTimes { get; set; } = Array.Empty<BaseModel<Guid>>();
        public IEnumerable<BaseModel<Guid>> AvailabilityDays { get; set; } = Array.Empty<BaseModel<Guid>>();
        public IEnumerable<BaseModel<Guid>> LocationPreferences { get; set; } = Array.Empty<BaseModel<Guid>>();
        public BaseModel<Guid> Lift { get; set; }
        public IEnumerable<BaseModel<Guid>> Languages { get; set; } = Array.Empty<BaseModel<Guid>>();
        public IEnumerable<SkillModel> Skills { get; set; } = Array.Empty<SkillModel>();
        public CovenantFileModel Resume { get; set; }
        public bool HaveAnyHealthProblem { get; set; }
        public string HealthProblem { get; set; }
        public string OtherHealthProblem { get; set; }
        public string ContactEmergencyName { get; set; }
        public string ContactEmergencyLastName { get; set; }
        public string ContactEmergencyPhone { get; set; }
        public IEnumerable<WorkerProfileJobExperienceModel> JobExperiences { get; set; } = Array.Empty<WorkerProfileJobExperienceModel>();
        public string Email { get; set; }
        public bool ApprovedToWork { get; set; }

    }
}