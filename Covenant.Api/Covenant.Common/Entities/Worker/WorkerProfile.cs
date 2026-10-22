using Covenant.Common.Constants;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Resources;
using Covenant.Common.Utils;
using Covenant.Common.Utils.Extensions;
using System.Text;

namespace Covenant.Common.Entities.Worker;

public class WorkerProfile :
    ISinInformation<CovenantFile>,
    IWorkerBasicInformation<Gender>,
    IEmergencyInformation,
    IWorkerDocumentsInformation<IdentificationType, CovenantFile>,
    IWorkerContactInformation<Location, City>,
    IWorkerProfileOtherInformation<Lift>,
    IWorkerRequirementsInformation
{
    private const int SocialInsuranceMinLength = 9;
    private const int SocialInsuranceMaxLength = 15;

    public WorkerProfile()
    {
    }

    public WorkerProfile(User user)
    {
        Worker = user ?? throw new ArgumentNullException(nameof(user));
        WorkerId = user.Id;
    }

    public WorkerProfile(User user, Guid agencyId) : this(user) => AgencyId = agencyId;

    public static WorkerProfile Register(WorkerProfileCreateModel model, Agency.Agency agency, User user)
    {
        var entity = new WorkerProfile
        {
            AgencyId = agency.Id,
            Agency = agency,
            Worker = user,
            WorkerId = user.Id,
            ApprovedToWork = false,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            SecondLastName = model.SecondLastName,
            BirthDay = model.BirthDay,
            GenderId = model.Gender?.Id == Guid.Empty ? null : model.Gender?.Id,
            HasVehicle = model.HasVehicle,
            MobileNumber = model.MobileNumber,
            Phone = model.Phone,
            PhoneExt = model.PhoneExt,
            Location = new Location
            {
                CityId = model.Location.City.Id,
                Address = model.Location.Address,
                PostalCode = model.Location.PostalCode
            },
            LiftId = model.Lift?.Id,
            IdentificationNumber1 = model.IdentificationNumber1,
            IdentificationType1Id = model.IdentificationType1?.Id,
            IdentificationNumber2 = model.IdentificationNumber2,
            IdentificationType2Id = model.IdentificationType2?.Id,
            HaveAnyHealthProblem = model.HaveAnyHealthProblem,
            HealthProblem = model.HealthProblem,
            OtherHealthProblem = model.OtherHealthProblem,
            ContactEmergencyName = model.ContactEmergencyName,
            ContactEmergencyLastName = model.ContactEmergencyLastName,
            ContactEmergencyPhone = model.ContactEmergencyPhone
        };
        entity.LocationId = entity.Location.Id;

        if (!string.IsNullOrEmpty(model.ProfileImage?.FileName))
        {
            entity.ProfileImage = new CovenantFile(model.ProfileImage.FileName, model.ProfileImage.Description);
            entity.ProfileImageId = entity.ProfileImage.Id;
        }

        if (!string.IsNullOrEmpty(model.IdentificationType1File?.FileName))
        {
            entity.IdentificationType1File = new CovenantFile(model.IdentificationType1File.FileName, model.IdentificationType1File.Description);
            entity.IdentificationType1FileId = entity.IdentificationType1File.Id;
        }

        if (!string.IsNullOrEmpty(model.IdentificationType2File?.FileName))
        {
            entity.IdentificationType2File = new CovenantFile(model.IdentificationType2File.FileName, model.IdentificationType2File.Description);
            entity.IdentificationType2FileId = entity.IdentificationType2File.Id;
        }

        if (!string.IsNullOrEmpty(model.PoliceCheckBackGround?.FileName))
        {
            entity.PoliceCheckBackGround = new CovenantFile(model.PoliceCheckBackGround.FileName, model.PoliceCheckBackGround.Description);
            entity.PoliceCheckBackGroundId = entity.PoliceCheckBackGround.Id;
        }

        if (!string.IsNullOrEmpty(model.Resume?.FileName))
        {
            entity.Resume = new CovenantFile(model.Resume.FileName, model.Resume.Description);
            entity.ResumeId = entity.Resume.Id;
        }

        entity.Availabilities = [.. model.Availabilities
            .Where(a => a.Id != Guid.Empty)
            .Select(a => new WorkerProfileAvailability { AvailabilityId = a.Id, WorkerProfile = entity })];

        entity.AvailabilityTimes = [.. model.AvailabilityTimes
            .Where(t => t.Id != Guid.Empty)
            .Select(t => new WorkerProfileAvailabilityTime { AvailabilityTimeId = t.Id, WorkerProfile = entity })];

        entity.AvailabilityDays = [.. model.AvailabilityDays
            .Where(d => d.Id != Guid.Empty)
            .Select(d => new WorkerProfileAvailabilityDay { DayId = d.Id, WorkerProfile = entity })];

        entity.LocationPreferences = [.. model.LocationPreferences
            .Where(c => c.Id != Guid.Empty)
            .Select(c => new WorkerProfileLocationPreference { CityId = c.Id, WorkerProfile = entity })];

        entity.Languages = [.. model.Languages
            .Where(l => l.Id != Guid.Empty)
            .Select(l => new WorkerProfileLanguage { LanguageId = l.Id, WorkerProfile = entity })];

        entity.Skills = [.. model.Skills.Select(s => new WorkerProfileSkill { Skill = s.Skill, WorkerProfile = entity })];

        entity.Licenses = [.. model.Licenses
            .Select(l =>
            {
                var file = new CovenantFile(l.License.FileName, l.License.Description);
                return new WorkerProfileLicense
                {
                    License = file,
                    LicenseId = file.Id,
                    Number = l.Number,
                    Issued = l.Issued,
                    Expires = l.Expires,
                    WorkerProfile = entity
                };
            })];

        entity.Certificates = [.. model.Certificates
            .Select(c =>
            {
                var file = new CovenantFile(c.FileName, c.Description);
                return new WorkerProfileCertificate
                {
                    Certificate = file,
                    CertificateId = file.Id,
                    WorkerProfile = entity
                };
            })];

        foreach (var document in model.OtherDocuments)
        {
            if (string.IsNullOrEmpty(document?.FileName)) continue;
            var file = new CovenantFile(document.FileName, document.Description);
            entity.OtherDocuments.Add(WorkerProfileOtherDocument.Create(entity.Id, file).Value);
        }

        return entity;
    }

    public void AssignTo(User worker, Guid agencyId)
    {
        Worker = worker;
        WorkerId = worker.Id;
        AgencyId = agencyId;
    }

    public Guid Id { get; internal set; } = Guid.NewGuid();
    public int NumberId { get; internal set; }
    public Guid WorkerId { get; private set; }
    public Guid AgencyId { get; internal set; }
    public Guid? ProfileImageId { get; private set; }
    public string FirstName { get; internal set; }
    public string MiddleName { get; internal set; }
    public string LastName { get; internal set; }
    public string SecondLastName { get; private set; }
    public DateTime BirthDay { get; private set; }
    public Guid? GenderId { get; private set; }
    public bool HasVehicle { get; private set; }
    public string SocialInsurance { get; internal set; }
    public string MaskedSocialInsurance => SocialInsurance.MaskSIN();
    public bool SocialInsuranceExpire { get; internal set; }
    public DateTime? DueDate { get; internal set; }
    public Guid? SocialInsuranceFileId { get; private set; }
    public string IdentificationNumber1 { get; internal set; }
    public Guid? IdentificationType1Id { get; internal set; }
    public Guid? IdentificationType1FileId { get; private set; }
    public string IdentificationNumber2 { get; private set; }
    public Guid? IdentificationType2Id { get; private set; }
    public Guid? IdentificationType2FileId { get; private set; }
    public bool HavePoliceCheckBackground { get; internal set; }
    public Guid? PoliceCheckBackGroundId { get; private set; }
    public Guid? ResumeId { get; private set; }
    public Guid LocationId { get; private set; }
    public string MobileNumber { get; private set; }
    public string Phone { get; private set; }
    public int? PhoneExt { get; private set; }
    public Guid? LiftId { get; private set; }
    public bool HaveAnyHealthProblem { get; private set; }
    public string HealthProblem { get; private set; }
    public string ContactEmergencyPhone { get; private set; }
    public string OtherHealthProblem { get; private set; }
    public string ContactEmergencyName { get; private set; }
    public string ContactEmergencyLastName { get; private set; }
    public bool ApprovedToWork { get; internal set; }
    public bool Dnu { get; private set; } = false;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public string CreatedBy { get; internal set; }
    public bool IsSubcontractor { get; internal set; }
    public string PunchCardId { get; internal set; }
    public bool IsContractor { get; internal set; } = false;
    public string ExternalId { get; private set; }
    public string WcCode { get; private set; }
    public WorkerProfileTaxCategory WorkerProfileTaxCategory { get; internal set; }
    public Gender Gender { get; private set; }
    public User Worker { get; private set; }
    public Agency.Agency Agency { get; internal set; }
    public CovenantFile ProfileImage { get; private set; }
    public CovenantFile SocialInsuranceFile { get; private set; }
    public IdentificationType IdentificationType1 { get; internal set; }
    public CovenantFile IdentificationType1File { get; internal set; }
    public CovenantFile IdentificationType2File { get; internal set; }
    public IdentificationType IdentificationType2 { get; internal set; }
    public CovenantFile PoliceCheckBackGround { get; internal set; }
    public CovenantFile Resume { get; internal set; }
    public Location Location { get; internal set; }
    public Lift Lift { get; private set; }
    public List<WorkerProfileOtherDocument> OtherDocuments { get; private set; } = [];
    public List<WorkerProfileNote> Notes { get; private set; } = [];
    public List<WorkerProfileAvailability> Availabilities { get; private set; } = [];
    public List<WorkerProfileAvailabilityTime> AvailabilityTimes { get; internal set; } = [];
    public List<WorkerProfileAvailabilityDay> AvailabilityDays { get; internal set; } = [];
    public List<WorkerProfileLocationPreference> LocationPreferences { get; internal set; } = [];
    public List<WorkerProfileLanguage> Languages { get; internal set; } = [];
    public List<WorkerProfileSkill> Skills { get; internal set; } = [];
    public List<WorkerProfileLicense> Licenses { get; internal set; } = [];
    public List<WorkerProfileCertificate> Certificates { get; internal set; } = [];
    public List<WorkerProfileJobExperience> JobExperiences { get; internal set; } = [];
    public List<Request.WorkerRequest> WorkerRequests { get; private set; } = [];

    public string FullName =>
        FirstName +
        (string.IsNullOrWhiteSpace(MiddleName) ? string.Empty : $" {MiddleName}") +
        $" {LastName}" +
        (string.IsNullOrWhiteSpace(SecondLastName) ? string.Empty : $" {SecondLastName}");

    public bool IsUsa => Location?.IsUSA ?? false;
    public bool HasProfileImage => !string.IsNullOrEmpty(ProfileImage?.FileName);
    public bool HasTaxCategory => WorkerProfileTaxCategory?.FederalCategory != null || WorkerProfileTaxCategory?.ProvincialCategory != null;
    public bool HasSocialInsuranceFile => !string.IsNullOrEmpty(SocialInsuranceFile?.FileName);
    public bool HasIdentification1File => !string.IsNullOrEmpty(IdentificationType1File?.FileName);
    public bool HasIdentification2File => !string.IsNullOrEmpty(IdentificationType2File?.FileName);

    public Result PatchContactInformation<TLocation, TCity>(IWorkerContactInformation<TLocation, TCity> contactInformation)
        where TLocation : ILocation<TCity>
        where TCity : ICatalog<Guid>
    {
        ArgumentNullException.ThrowIfNull(contactInformation);
        if (default(TLocation)?.Equals(contactInformation.Location) ?? false) return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.Location));
        var rLocation = Location.Create(contactInformation.Location.City.Id, contactInformation.Location.Address, contactInformation.Location.PostalCode);
        if (!rLocation) return Result.Fail(rLocation.Errors);
        if (Location is null)
        {
            Location = rLocation.Value;
            LocationId = Location.Id;
        }
        else Location.Update(rLocation.Value);

        if (!CommonValidators.IsValidPhoneNumber(contactInformation.MobileNumber))
            return Result.Fail(ValidationMessages.InvalidFormatMsg(ApiResources.MobileNumber));

        if (!CommonValidators.IsValidPhoneNumber(contactInformation.Phone))
            return Result.Fail(ValidationMessages.InvalidFormatMsg(ApiResources.Phone));

        MobileNumber = contactInformation.MobileNumber;
        Phone = contactInformation.Phone;
        PhoneExt = contactInformation.PhoneExt;
        return Result.Ok();
    }

    public Result PatchBasicInformation<TGender>(IWorkerBasicInformation<TGender> information)
        where TGender : ICatalog<Guid>
    {
        ArgumentNullException.ThrowIfNull(information);
        if (!information.FirstName.IsValidLength(CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength))
            return Result.Fail(ValidationMessages.LengthMsg(ApiResources.FirstName, CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength));
        if (!information.LastName.IsValidLength(CovenantConstants.Validation.LastNameMinLength, CovenantConstants.Validation.LastNameMaxLength))
            return Result.Fail(ValidationMessages.LengthMsg(ApiResources.LastName, CovenantConstants.Validation.LastNameMinLength, CovenantConstants.Validation.LastNameMaxLength));
        if (!string.IsNullOrEmpty(information.MiddleName))
        {
            if (!information.MiddleName.IsValidLength(CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength))
                return Result.Fail(ValidationMessages.LengthMsg(ApiResources.MiddleName, CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength));
        }

        if (!string.IsNullOrEmpty(information.SecondLastName))
        {
            if (!information.SecondLastName.IsValidLength(CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength))
                return Result.Fail(ValidationMessages.LengthMsg(ApiResources.SecondLastName, CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength));
        }

        //TODO write unit test
        if (information.BirthDay > DateTime.Now.Date.AddYears(-CovenantConstants.Validation.BirthdayMinimumAge)) return Result.Fail(ValidationMessages.AgeMsg(CovenantConstants.Validation.BirthdayMinimumAge));

        FirstName = information.FirstName;
        MiddleName = information.MiddleName;
        LastName = information.LastName;
        SecondLastName = information.SecondLastName;
        BirthDay = information.BirthDay;
        Guid? genderId = information.Gender?.Id;
        if (genderId == Guid.Empty) genderId = null;
        GenderId = genderId;
        HasVehicle = information.HasVehicle;
        return Result.Ok();
    }

    public Result PatchSinInformation<TFile>(ISinInformation<TFile> sinInformation) where TFile : ICovenantFile
    {
        ArgumentNullException.ThrowIfNull(sinInformation);
        if (!string.IsNullOrEmpty(sinInformation.SocialInsurance))
        {
            int length = sinInformation.SocialInsurance.Length;
            if (length < SocialInsuranceMinLength || length > SocialInsuranceMaxLength)
                return Result.Fail(ValidationMessages.LengthMsg(ApiResources.SocialInsurance, SocialInsuranceMinLength, SocialInsuranceMaxLength));
            if (sinInformation.SocialInsuranceExpire)
            {
                if (sinInformation.DueDate is null) return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.SocialInsuranceDate));
            }
        }

        SocialInsurance = sinInformation.SocialInsurance;
        SocialInsuranceExpire = sinInformation.SocialInsuranceExpire;
        DueDate = sinInformation.DueDate;
        if (string.IsNullOrEmpty(sinInformation.SocialInsuranceFile?.FileName))
        {
            SocialInsuranceFile = null;
            SocialInsuranceFileId = null;
            return Result.Ok();
        }

        return PatchSocialInsuranceDocument(sinInformation.SocialInsurance, sinInformation.SocialInsuranceFile);
    }

    public Result<bool> PatchSocialInsuranceFromIdentification(string socialInsurance, ICovenantFile socialInsuranceFile)
    {
        if (string.IsNullOrEmpty(socialInsurance)) return Result.Fail<bool>(ValidationMessages.RequiredMsg(ApiResources.SocialInsurance));
        int length = socialInsurance.Length;
        if (length < SocialInsuranceMinLength || length > SocialInsuranceMaxLength)
            return Result.Fail<bool>(ValidationMessages.LengthMsg(ApiResources.SocialInsurance, SocialInsuranceMinLength, SocialInsuranceMaxLength));
        var replaced = !string.IsNullOrEmpty(SocialInsurance) && SocialInsurance != socialInsurance;
        var patch = PatchSocialInsuranceDocument(socialInsurance, socialInsuranceFile);
        if (!patch) return Result.Fail<bool>(patch.Errors);
        return Result.Ok(replaced);
    }

    public Result PatchSocialInsuranceDocument(string socialInsurance, ICovenantFile socialInsuranceFile)
    {
        if (!string.IsNullOrEmpty(socialInsurance))
        {
            int length = socialInsurance.Length;
            if (length < SocialInsuranceMinLength || length > SocialInsuranceMaxLength)
                return Result.Fail(ValidationMessages.LengthMsg(ApiResources.SocialInsurance, SocialInsuranceMinLength, SocialInsuranceMaxLength));
            SocialInsurance = socialInsurance;
        }

        if (string.IsNullOrEmpty(socialInsuranceFile?.FileName)) return Result.Ok();
        if (SocialInsuranceFile is null)
        {
            SocialInsuranceFile = new CovenantFile(socialInsuranceFile.FileName, socialInsuranceFile.Description);
            SocialInsuranceFileId = SocialInsuranceFile.Id;
        }
        else
        {
            SocialInsuranceFile.FileName = socialInsuranceFile.FileName;
            SocialInsuranceFile.Description = socialInsuranceFile.Description;
        }

        return Result.Ok();
    }

    public void UpdateExternalId(string externalId) => ExternalId = externalId;

    public void UpdateWcCode(string wcCode) => WcCode = wcCode;

    public Result PatchProfileImage(ICovenantFile profileImage)
    {
        if (string.IsNullOrEmpty(profileImage?.FileName)) return Result.Ok();
        ProfileImage = new CovenantFile(profileImage.FileName, profileImage.Description);
        ProfileImageId = ProfileImage.Id;
        return Result.Ok();
    }

    public Result PatchDocuments<TTypeOfDocument, TFile>(IWorkerDocumentsInformation<TTypeOfDocument, TFile> documentsInformation)
        where TTypeOfDocument : ICatalog<Guid> where TFile : ICovenantFile
    {
        ArgumentNullException.ThrowIfNull(documentsInformation);
        if (string.IsNullOrEmpty(documentsInformation.IdentificationType1File?.FileName))
        {
            IdentificationType1File = null;
            IdentificationType1FileId = null;
        }
        var identification1 = PatchIdentification1(documentsInformation.IdentificationNumber1, documentsInformation.IdentificationType1?.Id, documentsInformation.IdentificationType1File);
        if (!identification1) return identification1;

        if (string.IsNullOrEmpty(documentsInformation.IdentificationType2File?.FileName))
        {
            IdentificationType2File = null;
            IdentificationType2FileId = null;
        }
        var identification2 = PatchIdentification2(documentsInformation.IdentificationNumber2, documentsInformation.IdentificationType2?.Id, documentsInformation.IdentificationType2File);
        if (!identification2) return identification2;

        if (string.IsNullOrEmpty(documentsInformation.PoliceCheckBackGround?.FileName))
        {
            PoliceCheckBackGround = null;
            PoliceCheckBackGroundId = null;
        }
        var policeCheck = PatchPoliceCheck(documentsInformation.PoliceCheckBackGround);
        if (!policeCheck) return policeCheck;
        HavePoliceCheckBackground = documentsInformation.HavePoliceCheckBackground;

        return PatchResume(documentsInformation.Resume);
    }

    public Result PatchIdentification1(string identificationNumber, Guid? identificationTypeId, ICovenantFile file)
    {
        IdentificationNumber1 = identificationNumber;
        IdentificationType1Id = identificationTypeId;
        if (string.IsNullOrEmpty(file?.FileName)) return Result.Ok();
        if (IdentificationType1File is null)
        {
            IdentificationType1File = new CovenantFile();
            IdentificationType1File.Update(file);
            IdentificationType1FileId = IdentificationType1File.Id;
        }
        else
        {
            IdentificationType1File.Update(file);
        }
        return Result.Ok();
    }

    public Result PatchIdentification2(string identificationNumber, Guid? identificationTypeId, ICovenantFile file)
    {
        IdentificationNumber2 = identificationNumber;
        IdentificationType2Id = identificationTypeId;
        if (string.IsNullOrEmpty(file?.FileName)) return Result.Ok();
        if (IdentificationType2File is null)
        {
            IdentificationType2File = new CovenantFile();
            IdentificationType2File.Update(file);
            IdentificationType2FileId = IdentificationType2File.Id;
        }
        else
        {
            IdentificationType2File.Update(file);
        }
        return Result.Ok();
    }

    public Result PatchPoliceCheck(ICovenantFile file)
    {
        if (string.IsNullOrEmpty(file?.FileName)) return Result.Ok();
        if (PoliceCheckBackGround is null)
        {
            PoliceCheckBackGround = new CovenantFile();
            PoliceCheckBackGround.Update(file);
            PoliceCheckBackGroundId = PoliceCheckBackGround.Id;
        }
        else
        {
            PoliceCheckBackGround.Update(file);
        }
        HavePoliceCheckBackground = true;
        return Result.Ok();
    }

    public void RemoveResume()
    {
        Resume = null;
        ResumeId = null;
    }

    public Result PatchResume(ICovenantFile resume)
    {
        if (string.IsNullOrEmpty(resume?.FileName)) return Result.Ok();
        if (Resume is null)
        {
            Resume = new CovenantFile();
            Resume.Update(resume);
            ResumeId = Resume.Id;
        }
        else
        {
            Resume.Update(resume);
        }
        return Result.Ok();
    }

    public Result PatchEmergencyInformation(IEmergencyInformation emergencyInformation)
    {
        ArgumentNullException.ThrowIfNull(emergencyInformation);
        if (!string.IsNullOrEmpty(emergencyInformation.ContactEmergencyName))
        {
            if (!emergencyInformation.ContactEmergencyName.IsValidLength(CovenantConstants.Validation.ContactEmergencyMinLength, CovenantConstants.Validation.ContactEmergencyMaxLength))
                return Result.Fail(ValidationMessages.LengthMsg(ApiResources.ContactEmergencyName, CovenantConstants.Validation.ContactEmergencyMinLength, CovenantConstants.Validation.ContactEmergencyMaxLength));
        }

        if (!string.IsNullOrEmpty(emergencyInformation.ContactEmergencyLastName))
        {
            if (!emergencyInformation.ContactEmergencyLastName.IsValidLength(CovenantConstants.Validation.ContactEmergencyMinLength, CovenantConstants.Validation.ContactEmergencyMaxLength))
                return Result.Fail(ValidationMessages.LengthMsg(ApiResources.ContactEmergencyLastName, CovenantConstants.Validation.ContactEmergencyMinLength, CovenantConstants.Validation.ContactEmergencyMaxLength));
        }

        if (!CommonValidators.IsValidPhoneNumber(emergencyInformation.ContactEmergencyPhone))
            return Result.Fail(ValidationMessages.InvalidFormatMsg(ApiResources.ContactEmergencyPhone));

        if (!string.IsNullOrEmpty(emergencyInformation.HealthProblem))
        {
            int length = emergencyInformation.HealthProblem.Length;
            if (length > CovenantConstants.Validation.HealthProblemMaxLength) return Result.Fail(ValidationMessages.LengthMaxMsg(ApiResources.HealthProblem, CovenantConstants.Validation.HealthProblemMaxLength));
        }

        if (!string.IsNullOrEmpty(emergencyInformation.OtherHealthProblem))
        {
            int length = emergencyInformation.OtherHealthProblem.Length;
            if (length > CovenantConstants.Validation.HealthProblemMaxLength) return Result.Fail(ValidationMessages.LengthMaxMsg(ApiResources.OtherHealthProblem, CovenantConstants.Validation.HealthProblemMaxLength));
        }

        ContactEmergencyName = emergencyInformation.ContactEmergencyName;
        ContactEmergencyLastName = emergencyInformation.ContactEmergencyLastName;
        ContactEmergencyPhone = emergencyInformation.ContactEmergencyPhone;
        HaveAnyHealthProblem = emergencyInformation.HaveAnyHealthProblem;
        HealthProblem = emergencyInformation.HealthProblem;
        OtherHealthProblem = emergencyInformation.OtherHealthProblem;
        return Result.Ok();
    }

    public Result PatchAvailabilities(IReadOnlyCollection<ICatalog<Guid>> availabilities)
    {
        IfIsNotInRemove(Availabilities, a => availabilities.Any(i => i.Id == a.AvailabilityId));
        foreach (var availability in availabilities)
        {
            if (availability.Id == Guid.Empty) continue;
            if (Availabilities.Any(a => a.AvailabilityId == availability.Id)) continue;
            Availabilities.Add(new WorkerProfileAvailability { AvailabilityId = availability.Id, WorkerProfile = this });
        }

        return Result.Ok();
    }

    public Result PatchAvailabilityTimes(IReadOnlyCollection<ICatalog<Guid>> availabilityTimes)
    {
        IfIsNotInRemove(AvailabilityTimes, t => availabilityTimes.Any(c => c.Id == t.AvailabilityTimeId));
        foreach (var time in availabilityTimes)
        {
            if (time.Id == Guid.Empty) continue;
            if (AvailabilityTimes.Any(a => a.AvailabilityTimeId == time.Id)) continue;
            AvailabilityTimes.Add(new WorkerProfileAvailabilityTime { AvailabilityTimeId = time.Id, WorkerProfile = this });
        }

        return Result.Ok();
    }

    public Result PatchAvailabilityDays(IReadOnlyCollection<ICatalog<Guid>> days)
    {
        IfIsNotInRemove(AvailabilityDays, day => days.Any(i => i.Id == day.DayId));
        foreach (var day in days)
        {
            if (day.Id == Guid.Empty) continue;
            if (AvailabilityDays.Any(a => a.DayId == day.Id)) continue;
            AvailabilityDays.Add(new WorkerProfileAvailabilityDay { DayId = day.Id, WorkerProfile = this });
        }

        return Result.Ok();
    }

    public Result PatchLocationPreferences(IReadOnlyCollection<ICatalog<Guid>> cities)
    {
        IfIsNotInRemove(LocationPreferences, p => cities.Any(i => i.Id == p.CityId));
        foreach (var city in cities)
        {
            if (city.Id == Guid.Empty) continue;
            if (LocationPreferences.Any(l => l.CityId == city.Id)) continue;
            LocationPreferences.Add(new WorkerProfileLocationPreference { CityId = city.Id, WorkerProfile = this });
        }

        return Result.Ok();
    }

    public Result PatchLanguages(IReadOnlyCollection<ICatalog<Guid>> languages)
    {
        IfIsNotInRemove(Languages, l => languages.Any(ca => ca.Id == l.LanguageId));
        foreach (var language in languages)
        {
            if (language.Id == Guid.Empty) continue;
            if (Languages.Any(l => l.LanguageId == language.Id)) continue;
            Languages.Add(new WorkerProfileLanguage { LanguageId = language.Id, WorkerProfile = this });
        }

        return Result.Ok();
    }

    public Result PatchSkills(IReadOnlyCollection<string> skills)
    {
        Skills.Clear();
        foreach (string skill in skills)
        {
            Skills.Add(new WorkerProfileSkill { Skill = skill, WorkerProfile = this });
        }
        return Result.Ok();
    }

    public Result PatchLicenses<TFile>(IReadOnlyCollection<IWorkerProfileLicense<TFile>> licenses)
        where TFile : ICovenantFile
    {
        IfIsNotInRemove(Licenses, l => licenses.Any(m => m.License.FileName == l.License.FileName));
        foreach (IWorkerProfileLicense<TFile> i in licenses)
        {
            if (string.IsNullOrEmpty(i.License?.FileName)) return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.LicenseFile));
            WorkerProfileLicense license = Licenses.FirstOrDefault(f => f.License.FileName == i.License.FileName);
            if (license is null)
            {
                var file = new CovenantFile(i.License.FileName, i.License.Description);
                Licenses.Add(new WorkerProfileLicense
                {
                    License = file,
                    Expires = i.Expires,
                    Issued = i.Issued,
                    Number = i.Number,
                    LicenseId = file.Id,
                    WorkerProfile = this
                });
                continue;
            }

            license.Number = i.Number;
            license.Expires = i.Expires;
            license.Issued = i.Issued;
        }

        return Result.Ok();
    }

    public Result PatchCertificates(IReadOnlyCollection<ICovenantFile> certificates)
    {
        IfIsNotInRemove(Certificates, c => certificates.Any(m => m.FileName == c.Certificate.FileName));
        foreach (ICovenantFile i in certificates)
        {
            if (string.IsNullOrEmpty(i.FileName)) return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.Certificates));
            if (Certificates.Any(c => c.Certificate.FileName == i.FileName)) continue;
            var file = new CovenantFile(i.FileName, i.Description);
            Certificates.Add(new WorkerProfileCertificate { Certificate = file, CertificateId = file.Id, WorkerProfile = this });
        }

        return Result.Ok();
    }

    public Result PatchOtherInformation<TLift>(IWorkerProfileOtherInformation<TLift> otherInformation)
        where TLift : ICatalog<Guid>
    {
        LiftId = otherInformation.Lift?.Id;
        return Result.Ok();
    }

    public Result<WorkerProfileJobExperience> AddJobExperience(IWorkerProfileJobExperience jobExperienceModel)
    {
        Result<WorkerProfileJobExperience> result = WorkerProfileJobExperience.Create(jobExperienceModel.Company,
            jobExperienceModel.Supervisor, jobExperienceModel.Duties,
            jobExperienceModel.StartDate, jobExperienceModel.EndDate, jobExperienceModel.IsCurrentJobPosition);
        if (!result) return result;
        WorkerProfileJobExperience jobExperience = result.Value;
        jobExperience.WorkerProfile = this;
        jobExperience.WorkerProfileId = this.Id;
        JobExperiences.Add(result.Value);
        return result;
    }

    public Result UpdateJobExperience(Guid id, IWorkerProfileJobExperience jobExperienceModel)
    {
        Result<WorkerProfileJobExperience> result = WorkerProfileJobExperience.Create(jobExperienceModel.Company,
            jobExperienceModel.Supervisor, jobExperienceModel.Duties, jobExperienceModel.StartDate,
            jobExperienceModel.EndDate, jobExperienceModel.IsCurrentJobPosition);
        if (!result) return result;
        WorkerProfileJobExperience current = JobExperiences.FirstOrDefault(e => e.Id == id);
        current?.Update(result.Value);
        return Result.Ok();
    }

    public Result AddOtherDocuments(IReadOnlyCollection<ICovenantFile> documents)
    {
        IfIsNotInRemove(OtherDocuments, d => documents.Any(c => c.FileName == d.Document.FileName));
        foreach (ICovenantFile document in documents)
        {
            if (OtherDocuments.Any(d => d.Document.FileName == document.FileName)) continue;
            var rDoc = CovenantFile.Create(document);
            if (!rDoc) return rDoc;
            var result = WorkerProfileOtherDocument.Create(Id, rDoc.Value);
            if (!result) return result;
            OtherDocuments.Add(result.Value);
        }
        return Result.Ok();
    }

    public Result DeleteJobExperience(Guid id)
    {
        WorkerProfileJobExperience toDelete = JobExperiences.FirstOrDefault(c => c.Id == id);
        if (toDelete is null) return Result.Fail();
        JobExperiences.Remove(toDelete);
        return Result.Ok();
    }

    public Result CanBeBook(IEnumerable<WorkerRequirement> missingRequirements)
    {
        if (IsSubcontractor || IsContractor) return Result.Ok();
        if (!ApprovedToWork) return Result.Fail("Worker is not approved to work");
        return ToResult(missingRequirements.Where(r => r.IsSocialInsuranceIssue()));
    }

    public Result UpdateApprovedToWork(IEnumerable<WorkerRequirement> missingRequirements)
    {
        if (ApprovedToWork)
        {
            ApprovedToWork = false;
            return Result.Ok();
        }

        Result requirements = ToResult(missingRequirements);
        if (!requirements) return requirements;
        ApprovedToWork = true;
        return Result.Ok();
    }

    private static Result ToResult(IEnumerable<WorkerRequirement> missing)
    {
        var errors = missing.Select(r => (ResultError)r.ToMessage()).ToArray();
        return errors.Length == 0 ? Result.Ok() : Result.Fail(errors);
    }

    private static void IfIsNotInRemove<T>(List<T> list, Func<T, bool> filter)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            T elementAt = list.ElementAt(i);
            if (filter(elementAt)) continue;
            list.Remove(elementAt);
        }
    }

    public void UpdateDnu() => Dnu = !Dnu;

    public Result UpdateContractor(IEnumerable<WorkerRequirement> missingDocuments)
    {
        if (IsContractor)
        {
            Result hasRequiredDocuments = ToResult(missingDocuments);
            if (!hasRequiredDocuments) return hasRequiredDocuments;
            IsContractor = false;
            return Result.Ok();
        }

        IsContractor = true;
        return Result.Ok();
    }

    public Result UpdateSubcontractor(IEnumerable<WorkerRequirement> missingDocuments, bool? isSubcontractor = null)
    {
        if (!isSubcontractor.HasValue) isSubcontractor = !IsSubcontractor;
        if (isSubcontractor.Value)
        {
            IsSubcontractor = true;
            return Result.Ok();
        }

        Result hasRequiredDocuments = ToResult(missingDocuments);
        if (!hasRequiredDocuments) return hasRequiredDocuments;
        IsSubcontractor = false;
        return Result.Ok();
    }
}